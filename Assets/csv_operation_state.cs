using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class csv_operation_state : MonoBehaviour
{
    [Header("Logging Control")]
    public bool enabledLogging = true;

    private StreamWriter sw;
    private string path;

    // 旧IsCheckの意味 = 「ファイル開いてる？」フラグ
    private bool isLogging = false;

    // 旧pending維持：状態変化があったら次のUpdateで1行だけ書く
    private bool pending = false;

    // 時間管理：セッション開始時刻＋累積（SAGAT前までの合計）
    private float startLocal;      // このセッションの開始時刻（Realtime）
    private float elapsedBefore;   // 累積（Pause時に足し込む）

    void Start()
    {
        // 旧仕様ではMakeFileで取り直すのでここは触らない
        startLocal = Time.realtimeSinceStartup;
        elapsedBefore = 0f;
    }

    void Update()
    {
        if (!enabledLogging || !isLogging || sw == null) return;

        // SAGAT中は完全停止（ pending フラグは保持されたまま ）
        if (TestDataManager.I != null && TestDataManager.I.pausedForSAGAT) return;

        if (pending)
        {
            float t = elapsedBefore + (Time.realtimeSinceStartup - startLocal);
            int state = operation_state_check.areaNow;

            sw.Write(t.ToString("F3"));
            sw.Write(",");
            sw.WriteLine(state.ToString());

            pending = false;
        }
    }

    // エリア変化時に呼ぶ（1行追記“予約”トリガ）
    public void OperationState()
    {
        if (!enabledLogging || !isLogging) return;
        // SAGAT中は書かないが、フラグを立てておけば復帰後の1フレームで書ける
        pending = true;
    }

    // 互換：UI Text 版（必要なら）
    public void MakeFile(Text filename)
    {
        if (filename == null || string.IsNullOrWhiteSpace(filename.text))
        {
            Debug.LogWarning("[csv_operation_state] filenameText is null/empty.");
            return;
        }
        MakeFile(filename.text);
    }

    // 推奨：string 版（StartLineで一度だけ呼ぶ）
    public void MakeFile(string baseName)
    {
        if (!enabledLogging) return;
        if (isLogging) return; // 冪等

        string dir = Path.Combine(Application.dataPath, "ExperimentData_2025");
        Directory.CreateDirectory(dir);
        path = Path.Combine(dir, baseName + "_operation_state.csv");

        // 新規/既存に関わらず「追記モード」で開く
        bool exists = File.Exists(path);
        sw = new StreamWriter(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read),
                              new UTF8Encoding(false)) { AutoFlush = true };

        if (!exists)
        {
            // ヘッダ
            sw.WriteLine("time,operation_state");
            // 旧仕様どおり：StartLine瞬間を基準t=0でstate0を即1行出力
            sw.Write("0.000,");
            sw.WriteLine("0");
        }

        // 時間基準：新規開始なので累積0から
        elapsedBefore = 0f;
        startLocal = Time.realtimeSinceStartup;
        isLogging = true;

        // パスと累積をTDMに保存（復帰用）
        if (TestDataManager.I != null)
        {
            TestDataManager.I.opstateLogPath = path;
            TestDataManager.I.opstateElapsed = 0f;
        }

        Debug.Log("[csv_operation_state] Start logging (append mode). path=" + path);
    }

    // ====== SAGAT前：一時停止 ======
    public void PauseLogging()
    {
        if (!isLogging || sw == null) return;

        // ここまでの経過を累積へ
        elapsedBefore += (Time.realtimeSinceStartup - startLocal);

        // 復帰用に保存
        if (TestDataManager.I != null)
            TestDataManager.I.opstateElapsed = elapsedBefore;

        // ファイルを閉じる（SAGAT中は書かない）
        try { sw.Flush(); sw.Close(); } finally { sw = null; }
        isLogging = false;
    }

    // ====== SAGAT後：続きから再開（同じCSVに追記） ======
    public void ResumeLogging()
    {
        if (TestDataManager.I == null) return;

        path = TestDataManager.I.opstateLogPath;
        elapsedBefore = TestDataManager.I.opstateElapsed;

        if (string.IsNullOrEmpty(path)) { Debug.LogWarning("[csv_operation_state] Resume failed: no path."); return; }

        sw = new StreamWriter(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read),
                              new UTF8Encoding(false)) { AutoFlush = true };

        startLocal = Time.realtimeSinceStartup;
        isLogging = true;

        // pending が true のままなら、復帰直後の Update で1行だけ出力される
        Debug.Log($"[csv_operation_state] Resume. elapsedBefore={elapsedBefore:F3}s");
    }

    public void Close()
    {
        if (sw != null)
        {
            try { sw.Flush(); sw.Close(); } finally { sw.Dispose(); sw = null; }
        }
        isLogging = false;
        pending = false;
    }

    void OnApplicationQuit() { Close(); }
    void OnDestroy()         { Close(); }
}
