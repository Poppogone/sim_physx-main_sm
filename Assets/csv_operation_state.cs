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
    private bool IsCheck = false;   // ファイルオープン済みガード
    private bool pending = false;   // 「今書いて」の合図
    private float timeStart;        // 計測基準（StartLineで取り直す）

    void Start()
    {
        timeStart = Time.realtimeSinceStartup; // 初期値（MakeFileで取り直す）
    }

    void Update()
    {
        if (!enabledLogging || !IsCheck || sw == null) return;

        if (pending)
        {
            float t = Time.realtimeSinceStartup - timeStart;
            int state = operation_state_check.areaNow;

            sw.Write(t.ToString("F3"));
            sw.Write(",");
            sw.WriteLine(state.ToString());

            pending = false;
        }
    }

    // エリア変化時に呼ぶ（1行追記トリガ）
    public void OperationState()
    {
        if (!enabledLogging || !IsCheck) return;
        pending = true;
    }

    // 互換：UI Text 版
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
        if (IsCheck) return; // 冪等

        string dir = Path.Combine(Application.dataPath, "ExperimentData_2025");
        Directory.CreateDirectory(dir);
        path = Path.Combine(dir, baseName + "_operation_state.csv");

        var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
        sw = new StreamWriter(fs, new UTF8Encoding(false)) { AutoFlush = true };

        // ヘッダ
        sw.WriteLine("time,operation_state");

        // ★ StartLine通過“その瞬間”を基準にする
        timeStart = Time.realtimeSinceStartup;

        // ★ 最初の state0 を「StartLineの瞬間」で記録（t=0.000）
        sw.Write("0.000,");
        sw.WriteLine("0");

        IsCheck = true;

        Debug.Log("[csv_operation_state] Start logging (w/ initial state0 at t=0): " + path);
    }

    public void Close()
    {
        if (sw != null)
        {
            try { sw.Flush(); sw.Close(); } finally { sw.Dispose(); sw = null; }
        }
        IsCheck = false;
        pending = false;
    }

    void OnApplicationQuit() { Close(); }
    void OnDestroy()         { Close(); }
}
