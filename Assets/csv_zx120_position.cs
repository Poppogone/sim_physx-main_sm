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
    [SerializeField] private Text filenameText;      // 既存UIテキスト（csv_operation_state.csに合わせる）

    private StreamWriter sw;
    private bool tracing = false;
    private float t0 = 0f;

    // ====== 外部から呼ぶAPI ======

    // csv_operation_state.cs と同様の命名＆保存先
    public void MakeFile(Text filename)
    {
        if (!enabledLogging) return;
        CloseIfOpen();
        var path = @"Assets/ExperimentData_2025/" + filename.text + "_zx120_position.csv";
        sw = new StreamWriter(path, false, Encoding.UTF8);
        sw.WriteLine("time,x,y,z");  // ヘッダ
    }

    // start_check.cs からワンコールで開始できるように
    public void StartCheck()
    {
        if (!enabledLogging) return;
        if (sw == null)
        {
            // filenameText が割り当て済みならそれを使ってファイル新規作成
            if (filenameText != null) MakeFile(filenameText);
            else Debug.LogWarning("[csv_zx120_position] filenameTextが未設定です。");
        }
        t0 = Time.realtimeSinceStartup;
        tracing = true;
        Debug.Log("[csv_zx120_position] Trace START");
    }

    // Finish 合図で呼ぶ
    public void StopTrace()
    {
        tracing = false;
        CloseIfOpen();
        Debug.Log("[csv_zx120_position] Trace STOP & file closed");
    }

    // ====== 内部処理 ======

    private void Update()
    {
        if (!enabledLogging || !tracing || sw == null || zx120 == null) return;

        float t = Time.realtimeSinceStartup - t0;
        Vector3 p = zx120.position;
        // 軽量・低GCの手書きフォーマット
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
    }

    private void CloseIfOpen()
    {
        if (sw != null)
        {
            sw.Flush();
            sw.Close();
            sw.Dispose();
            sw = null;
        }
    }
}
