using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Tobii.Gaming;

public class csv_EyeTrackOnly : MonoBehaviour
{
    [Header("Logging Control")]
    public bool enabledLogging = true;      // 記録を行うか

    [Header("視線マーカー (UI Image の RectTransform)")]
    [SerializeField] private RectTransform e;   // 視線点を表示するUI Image

    private StreamWriter sw;
    private string path;
    private bool isLogging = false;
    private float t0;
    private Vector2 filteredPos;

    void Update()
    {
        if (!enabledLogging || !isLogging) return;

        float t = Time.realtimeSinceStartup - t0;
        GazePoint gp = TobiiAPI.GetGazePoint();

        if (gp.IsRecent())
        {
            Vector2 gazePos = gp.Screen;
            filteredPos = Vector2.Lerp(filteredPos, gazePos, 0.5f);

            if (e != null)
                e.position = new Vector3(filteredPos.x, filteredPos.y, 0);

            sw.WriteLine($"{t:F6},{gazePos.x:F3},{gazePos.y:F3},{filteredPos.x:F3},{filteredPos.y:F3}");
        }
        else
        {
            sw.WriteLine($"{t:F6},NN,NN,NN,NN");
        }
    }

    void OnDestroy()
    {
        StopAndSave();
    }

    // ======= MakeFile（Startボタンから呼ぶ）=======
    public void MakeFile(Text filenameUI)
    {
        if (filenameUI == null || string.IsNullOrWhiteSpace(filenameUI.text)) return;
        MakeFile(filenameUI.text.Trim());
    }

    public void MakeFile(string baseName)
    {
        if (!enabledLogging || isLogging) return;

        // 保存先ディレクトリ
        string dir = Path.Combine(Application.dataPath, "ExperimentData_2025", "EyeTrack_2025");
        Directory.CreateDirectory(dir);

        // ファイルパス決定
        path = Path.Combine(dir, $"{baseName}_EyeTrackOnly.csv");

        // CSV書き込み準備
        sw = new StreamWriter(path, append: false, new UTF8Encoding(false)) { AutoFlush = true };
        sw.WriteLine("time,pos_x,pos_y,corrected_pos_x,corrected_pos_y");

        // 記録開始
        t0 = Time.realtimeSinceStartup;
        filteredPos = Vector2.zero;
        isLogging = true;

        Debug.Log($"[csv_EyeTrackOnly] Start logging: {path}");
    }

    // ======= 記録停止 =======
    public void StopAndSave()
    {
        if (sw != null)
        {
            try { sw.Flush(); } catch { }
            try { sw.Close(); } catch { }
            sw = null;
        }
        isLogging = false;
        Debug.Log("[csv_EyeTrackOnly] Logging stopped.");
    }
}
