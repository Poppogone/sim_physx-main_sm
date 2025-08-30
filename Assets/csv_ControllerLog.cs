using System;
using System.IO;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class csv_ControllerLog : MonoBehaviour
{
    [Header("Logging Control")]
    public bool enabledLogging = true;

    [Tooltip("サンプリング周期(秒) 例: 0.02=50Hz")]
    public float sampleInterval = 0.02f;

    [Tooltip("ゼロ判定のしきい値（この絶対値以下を0とみなす）")]
    public float epsilonZero = 0.02f;

    private StreamWriter sw;
    private string path;
    private bool IsCheck = false;

    private float timeStart;
    private float nextSampleTime;

    // 連続アイドル秒（行ごとに H 列へ）
    private float idleAccum = 0f;
    // アイドル合計（最後に1行目の K 列へ）
    private float totalIdleTime = 0f;

    void Start()
    {
        timeStart = Time.realtimeSinceStartup;
        nextSampleTime = Time.unscaledTime;
    }

    void Update()
    {
        if (!enabledLogging) return;
        if (!IsCheck) return;
        if (Time.unscaledTime < nextSampleTime) return;

        float timeNow = Time.realtimeSinceStartup - timeStart;

        // zx120Controller から6値取得
        float crawlerL  = zx120Controller.leftWheel;
        float crawlerR  = zx120Controller.rightWheel;
        float boom   = zx120Controller.boomDirection;
        float turn   = zx120Controller.swingDirection;
        float arm    = zx120Controller.armDirection;
        float bucket = zx120Controller.bucketDirection;

        // ★ 「6つすべてが0（≒しきい値以内）」ならアイドル
        bool allZero =
            Mathf.Abs(crawlerL)   <= epsilonZero &&
            Mathf.Abs(crawlerR)  <= epsilonZero &&
            Mathf.Abs(boom)   <= epsilonZero &&
            Mathf.Abs(turn)  <= epsilonZero &&
            Mathf.Abs(arm)    <= epsilonZero &&
            Mathf.Abs(bucket) <= epsilonZero;

        if (allZero)
        {
            idleAccum     += sampleInterval;   // 連続アイドル
            totalIdleTime += sampleInterval;   // 合計アイドル
        }
        else
        {
            idleAccum = 0f; // 連続リセット（合計はリセットしない）
        }

        // 1行出力（H 列が IdleTime）
        string[] row = {
            timeNow.ToString(),
            crawlerL.ToString(),
            crawlerR.ToString(),
            turn.ToString(),
            arm.ToString(),
            boom.ToString(),
            bucket.ToString(),
            idleAccum.ToString("F3")   // ← H列
        };
        sw.WriteLine(string.Join(",", row));

        nextSampleTime += sampleInterval;
    }

    public void MakeFile(Text filename)
    {
        if (!enabledLogging) return;

#if UNITY_EDITOR
        string dir = Path.Combine(Application.dataPath, "ExperimentData_2025");
#else
        string dir = Path.Combine(Application.persistentDataPath, "ExperimentData_2025");
#endif
        Directory.CreateDirectory(dir);

        path = Path.Combine(dir, filename.text + "_ControllerLog.csv");
        sw = new StreamWriter(path, false, Encoding.UTF8);

        // A〜K列までのヘッダを作る（K=11列目に TotalIdleTime の見出し）
        // 列: A Time, B CrawlerL, C CrawlerR, D Turn, E Arm, F Boom, G Bucket, H IdleTime, I (空), J (空), K TotalIdleTime
        string[] header = {
            "Time","CrawlerL","CrawlerR","Turn","Arm","Boom","Bucket",
            "IdleTime","","","TotalIdleTime"
        };
        sw.WriteLine(string.Join(",", header));
        sw.Flush();

        IsCheck = true;
        timeStart = Time.realtimeSinceStartup;
        nextSampleTime = Time.unscaledTime;

        // 初期化
        idleAccum = 0f;
        totalIdleTime = 0f;

        Debug.Log("[csv_ControllerLog] Start logging: " + path);
    }

    public void Close()
    {
        if (IsCheck && sw != null)
        {
            sw.Close();
            sw = null;
            IsCheck = false;

            // 1行目の K 列（インデックス10）へ合計アイドル秒を書き込む
            var lines = File.ReadAllLines(path, Encoding.UTF8).ToList();
            if (lines.Count > 0)
            {
                var cols = lines[0].Split(',');
                if (cols.Length < 11)
                {
                    Array.Resize(ref cols, 11); // K列まで拡張
                }
                cols[10] = totalIdleTime.ToString("F3"); // K列へ代入
                lines[0] = string.Join(",", cols);
                File.WriteAllLines(path, lines, Encoding.UTF8);
            }

            Debug.Log("[csv_ControllerLog] Closed. TotalIdleTime(K) = " + totalIdleTime.ToString("F3"));
        }
    }
}
