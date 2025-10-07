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
    private bool isLogging = false;
    private bool pending = false;

    private float startRealtime; // 実験開始のリアル時間基準（SAGAT含む）

    void Start()
    {
        startRealtime = Time.realtimeSinceStartup;
    }

    void Update()
    {
        if (!enabledLogging || !isLogging || sw == null) return;

        // SAGAT中でも時間は進める（※出力は制御する）
        if (pending)
        {
            float t = Time.realtimeSinceStartup - startRealtime;
            int state = operation_state_check.areaNow;

            sw.WriteLine($"{t:F3},{state}");
            pending = false;
        }
    }

    // ==== 任意イベント書き込み（SAGAT_ENTER / EXITなど） ====
    public void WriteCustomEvent(string eventName)
    {
        if (!enabledLogging || !isLogging || sw == null) return;

        float t = Time.realtimeSinceStartup - startRealtime;
        sw.WriteLine($"{t:F3},{eventName}");
        sw.Flush();
    }

    // ==== area変化時に呼ばれる ====
    public void OperationState()
    {
        if (!enabledLogging || !isLogging) return;
        pending = true;
    }

    // ==== CSVファイル生成 ====
    public void MakeFile(string baseName)
    {
        if (!enabledLogging || isLogging) return;

        string dir = Path.Combine(Application.dataPath, "ExperimentData_2025");
        Directory.CreateDirectory(dir);
        path = Path.Combine(dir, baseName + "_operation_state.csv");

        bool exists = File.Exists(path);
        sw = new StreamWriter(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read),
                              new UTF8Encoding(false)) { AutoFlush = true };

        if (!exists)
        {
            sw.WriteLine("time,operation_state");
            sw.WriteLine("0.000,0");
        }

        startRealtime = Time.realtimeSinceStartup;
        isLogging = true;

        Debug.Log("[csv_operation_state] Start logging (realtime mode). path=" + path);
    }

    // ==== SAGAT中も閉じずに継続 ====
    public void PauseLogging()  { /* 何もしない */ }
    public void ResumeLogging() { /* 何もしない */ }

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
