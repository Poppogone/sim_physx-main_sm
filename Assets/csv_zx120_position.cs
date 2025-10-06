using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class csv_zx120_position : MonoBehaviour
{
    [Header("Logging Control")]
    public bool enabledLogging = true;

    [Header("Targets")]
    [SerializeField] private Transform zx120;  // 追跡対象（車体）

    [Header("Filename (UI Text)")]
    [SerializeField] private Text filenameText;

    private StreamWriter sw;
    private string path;
    private bool isLogging = false;
    private float startLocal;        // このセッションの開始時間
    private float elapsedBefore = 0; // 累積時間

    void OnDestroy()
    {
        Close();
    }

    void Update()
    {
        if (!enabledLogging || !isLogging) return;
        if (TestDataManager.I != null && TestDataManager.I.pausedForSAGAT) return;
        if (zx120 == null) return;

        float t = elapsedBefore + (Time.realtimeSinceStartup - startLocal);
        Vector3 p = zx120.position;
        sw.WriteLine($"{t:F3},{p.x:F4},{p.y:F4},{p.z:F4}");
    }

    // ===== ファイル作成（初回のみ） =====
    public void MakeFile(Text filenameUI)
    {
        if (!enabledLogging || filenameUI == null || string.IsNullOrWhiteSpace(filenameUI.text)) return;
        MakeFile(filenameUI.text.Trim());
    }

    public void MakeFile(string baseName)
    {
        if (!enabledLogging || isLogging) return;

        string dir = Path.Combine(Application.dataPath, "ExperimentData_2025");
        Directory.CreateDirectory(dir);
        path = Path.Combine(dir, $"{baseName}_zx120_position.csv");

        bool exists = File.Exists(path);
        sw = new StreamWriter(path, append: true, new UTF8Encoding(false)) { AutoFlush = true };
        if (!exists) sw.WriteLine("Time,X,Y,Z");

        startLocal = Time.realtimeSinceStartup;
        elapsedBefore = 0f;
        isLogging = true;

        TestDataManager.I.posLogPath = path;
        TestDataManager.I.posElapsed = 0f;
        Debug.Log("[csv_zx120_position] Start logging: " + path);
    }

    // ===== 一時停止 =====
    public void PauseLogging()
    {
        if (!isLogging) return;

        elapsedBefore += (Time.realtimeSinceStartup - startLocal);
        TestDataManager.I.posElapsed = elapsedBefore;

        sw.Flush();
        sw.Close();
        sw = null;
        isLogging = false;

        Debug.Log($"[csv_zx120_position] Pause at {elapsedBefore:F3}s");
    }

    // ===== 再開 =====
    public void ResumeLogging()
    {
        path = TestDataManager.I.posLogPath;
        elapsedBefore = TestDataManager.I.posElapsed;
        if (string.IsNullOrEmpty(path)) return;

        sw = new StreamWriter(path, append: true, new UTF8Encoding(false)) { AutoFlush = true };
        startLocal = Time.realtimeSinceStartup;
        isLogging = true;

        Debug.Log($"[csv_zx120_position] Resume logging from {elapsedBefore:F3}s");
    }

    // ===== クローズ =====
    public void Close()
    {
        if (!isLogging) return;
        sw.Flush();
        sw.Close();
        sw = null;
        isLogging = false;
    }
}
