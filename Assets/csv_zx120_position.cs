using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class csv_zx120_position : MonoBehaviour
{
    [Header("Logging Control")]
    public bool enabledLogging = true;

    [Header("Targets")]
    [SerializeField] private Transform zx120;        // 追跡対象（車体）

    [Header("Filename (UI Text)")]
    [SerializeField] private Text filenameText;      // 必要なら start_check から参照

    private StreamWriter sw;
    private string path;
    private bool tracing = false;
    private float t0 = 0f;
    private bool IsCheck = false;                    // ファイルオープン済みガード

    // ====== 外部から呼ぶAPI ======

    /*
    // UI Text を受け取る互換版
    public void MakeFile(Text filename)
    {
        if (filename == null || string.IsNullOrWhiteSpace(filename.text))
        {
            Debug.LogWarning("[csv_zx120_position] filenameText is null/empty.");
            return;
        }
        MakeFile(filename.text);
    }
    */

    // string 版（StartLineで一度だけ呼ぶ）
    public void MakeFile(string baseName)
    {
        if (!enabledLogging) return;
        if (IsCheck) return; // 冪等：二重初期化防止

        // 保存先を統一（他ロガーと同様）
        string dir = Path.Combine(Application.dataPath, "ExperimentData_2025");
        Directory.CreateDirectory(dir);
        path = Path.Combine(dir, baseName + "_zx120_position.csv");

        // 共有違反を避けつつ即時書き出し
        var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
        sw = new StreamWriter(fs, new UTF8Encoding(false)) { AutoFlush = true };

        // ヘッダ
        sw.WriteLine("time,x,y,z");

        // 記録開始（StartCheckは不要）
        IsCheck = true;
        tracing = true;
        t0 = Time.realtimeSinceStartup;

        Debug.Log("[csv_zx120_position] Start logging: " + path);
    }

    // StopLine等で止めたいときに呼ぶ
    public void StopTrace()
    {
        tracing = false;
        CloseIfOpen();
        IsCheck = false;
        Debug.Log("[csv_zx120_position] Trace STOP & file closed");
    }

    // ====== 内部処理 ======

    private void Update()
    {
        if (!enabledLogging || !tracing || !IsCheck || sw == null || zx120 == null) return;

        float t = Time.realtimeSinceStartup - t0;
        Vector3 p = zx120.position;

        // 軽量・低GC
        sw.Write(t.ToString("F4"));
        sw.Write(",");
        sw.Write(p.x.ToString("F5"));
        sw.Write(",");
        sw.Write(p.y.ToString("F5"));
        sw.Write(",");
        sw.WriteLine(p.z.ToString("F5"));
    }

    private void OnDestroy()
    {
        CloseIfOpen();
        IsCheck = false;
    }

    private void CloseIfOpen()
    {
        if (sw != null)
        {
            try
            {
                sw.Flush();
                sw.Close();
            }
            finally
            {
                sw.Dispose();
                sw = null;
            }
        }
    }

    public void Close()
    {
        tracing = false;
        CloseIfOpen();
        IsCheck = false;
        //Debug.Log("[csv_zx120_position] Close called -> file closed");
    }
}
