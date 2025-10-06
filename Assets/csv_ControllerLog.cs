using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Globalization;
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

    private float timeStart;        // このセッションの開始時刻
    private float nextSampleTime;

    // 連続アイドル秒（行ごとに H 列へ）
    private float idleAccum = 0f;
    // アイドル合計（最後に1行目の K 列へ）
    private float totalIdleTime = 0f;

    // ---- 追記再開のためのキー（PlayerPrefs）----
    private const string PP_KEY_PATH    = "CTL_ControllerLog_Path";
    private const string PP_KEY_ELAPSED = "CTL_ControllerLog_Elapsed";

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

        // zx120Controller から6値取得（測定処理はそのまま）
        float crawlerL = zx120Controller.leftWheel;
        float crawlerR = zx120Controller.rightWheel;
        float boom     = zx120Controller.boomDirection;
        float turn     = zx120Controller.swingDirection;
        float arm      = zx120Controller.armDirection;
        float bucket   = zx120Controller.bucketDirection;

        // ★ 「6つすべてが0（≒しきい値以内）」ならアイドル（ロジックはそのまま）
        bool allZero =
            Mathf.Abs(crawlerL) <= epsilonZero &&
            Mathf.Abs(crawlerR) <= epsilonZero &&
            Mathf.Abs(boom)     <= epsilonZero &&
            Mathf.Abs(turn)     <= epsilonZero &&
            Mathf.Abs(arm)      <= epsilonZero &&
            Mathf.Abs(bucket)   <= epsilonZero;

        if (allZero)
        {
            idleAccum     += sampleInterval;   // 連続アイドル
            totalIdleTime += sampleInterval;   // 合計アイドル
        }
        else
        {
            idleAccum = 0f; // 連続リセット（合計はリセットしない）
        }

        // 1行出力（H 列が IdleTime）／列順・内容はそのまま
        string[] row = {
            timeNow.ToString(CultureInfo.InvariantCulture),
            crawlerL.ToString(CultureInfo.InvariantCulture),
            crawlerR.ToString(CultureInfo.InvariantCulture),
            turn.ToString(CultureInfo.InvariantCulture),
            arm.ToString(CultureInfo.InvariantCulture),
            boom.ToString(CultureInfo.InvariantCulture),
            bucket.ToString(CultureInfo.InvariantCulture),
            idleAccum.ToString("F3", CultureInfo.InvariantCulture)   // ← H列
        };
        sw.WriteLine(string.Join(",", row));

        nextSampleTime += sampleInterval;
    }

    // ===== ここから：新規作成 / 一時停止 / 再開 の追加 =====

    /// <summary>
    /// 新規にCSVを作成して記録を開始（従来どおり）
    /// ※ 再開したい場合は ResumeLogging() を呼んでください
    /// </summary>
    public void MakeFile(string baseName)
    {
        if (!enabledLogging) return;
        if (IsCheck) return;  // 冪等ガード

        // 保存先ディレクトリ（従来どおり）
        string dir = Path.Combine(Application.dataPath, "ExperimentData_2025");
        Directory.CreateDirectory(dir);

        path = Path.Combine(dir, baseName + "_ControllerLog.csv");

        // FileShare.Read で開く + AutoFlush = true（従来どおり）
        var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
        sw = new StreamWriter(fs, new UTF8Encoding(false)) { AutoFlush = true };

        // ヘッダ行（従来どおり）
        string[] header = {
            "Time","CrawlerL","CrawlerR","Turn","Arm","Boom","Bucket",
            "IdleTime","","","TotalIdleTime" // K列に合計を書き戻す仕様
        };
        sw.WriteLine(string.Join(",", header));

        // 初期化（従来どおり）
        IsCheck = true;
        timeStart = Time.realtimeSinceStartup;
        nextSampleTime = Time.unscaledTime;
        idleAccum = 0f;
        totalIdleTime = 0f;

        // 新規開始なので、再開用の情報を保存
        PlayerPrefs.SetString(PP_KEY_PATH, path);
        PlayerPrefs.SetString(PP_KEY_ELAPSED, "0");
        PlayerPrefs.Save();

        Debug.Log("[csv_ControllerLog] Start logging (new): " + path);
    }

    /// <summary>
    /// 記録を一時停止（SAGATへ遷移など）。ヘッダK列へ合計アイドルを書き戻し、パスと経過秒を保存。
    /// </summary>
    public void PauseLogging()
    {
        if (!IsCheck || sw == null) return;

        // ここまでの経過秒（Time列の継続用）
        float elapsed = Time.realtimeSinceStartup - timeStart;

        // K列へ totalIdleTime を書き戻す
        WriteTotalIdleToHeader(path, totalIdleTime);

        // パスと経過秒を保存（PlayerPrefs）
        PlayerPrefs.SetString(PP_KEY_PATH, path);
        PlayerPrefs.SetString(PP_KEY_ELAPSED, elapsed.ToString("F6", CultureInfo.InvariantCulture));
        PlayerPrefs.Save();

        // クローズ
        sw.Close();
        sw = null;
        IsCheck = false;

        Debug.Log($"[csv_ControllerLog] Paused. Saved path & elapsed. (elapsed={elapsed:F3}s, totalIdle={totalIdleTime:F3}s)");
    }

    /// <summary>
    /// 直前の続きから記録を再開（同じCSVに追記）。Time列を連続させる。
    /// </summary>
    public void ResumeLogging()
    {
        if (!enabledLogging) return;
        if (IsCheck) return; // すでに開いているなら何もしない

        string savedPath = PlayerPrefs.GetString(PP_KEY_PATH, "");
        string savedElapsedStr = PlayerPrefs.GetString(PP_KEY_ELAPSED, "");
        if (string.IsNullOrEmpty(savedPath) || !File.Exists(savedPath))
        {
            Debug.LogWarning("[csv_ControllerLog] ResumeLogging: saved file not found. Call MakeFile() for new logging.");
            return;
        }

        path = savedPath;

        // 直前までの経過秒を取得（Time列連続のため）
        float elapsedBefore = 0f;
        float.TryParse(savedElapsedStr, NumberStyles.Float, CultureInfo.InvariantCulture, out elapsedBefore);

        // ヘッダK列（TotalIdleTime）を読み取り、累積合計を復元
        totalIdleTime = ReadTotalIdleFromHeader(path);

        // 追記で開く（読み取り共有・AutoFlush）
        var fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read);
        sw = new StreamWriter(fs, new UTF8Encoding(false)) { AutoFlush = true };

        // timeStart を「今−直前累積」にして、timeNow=(now-timeStart)が連続するようにする
        timeStart = Time.realtimeSinceStartup - elapsedBefore;

        // サンプリングタイマは今から（バックログ実行を避ける）
        nextSampleTime = Time.unscaledTime;

        // 再開時は連続アイドルカウンタは 0 から
        idleAccum = 0f;

        IsCheck = true;

        Debug.Log($"[csv_ControllerLog] Resumed append: {path} (elapsedBefore={elapsedBefore:F3}s, totalIdle={totalIdleTime:F3}s)");
    }

    /// <summary>
    /// 完了クローズ（従来の Close）。K列へ合計アイドルを書き戻して閉じる。
    /// </summary>
    public void Close()
    {
        if (IsCheck && sw != null)
        {
            // K列に合計アイドルを書き戻す
            WriteTotalIdleToHeader(path, totalIdleTime);

            sw.Close();
            sw = null;
            IsCheck = false;

            Debug.Log("[csv_ControllerLog] Closed. TotalIdleTime(K) = " + totalIdleTime.ToString("F3", CultureInfo.InvariantCulture));
        }
    }

    // ===== ヘルパ =====

    private void WriteTotalIdleToHeader(string filePath, float totalIdle)
    {
        try
        {
            var lines = File.ReadAllLines(filePath, Encoding.UTF8).ToList();
            if (lines.Count > 0)
            {
                var cols = lines[0].Split(',');
                if (cols.Length < 11) Array.Resize(ref cols, 11); // K列まで拡張
                cols[10] = totalIdle.ToString("F3", CultureInfo.InvariantCulture); // K列へ
                lines[0] = string.Join(",", cols);
                File.WriteAllLines(filePath, lines, Encoding.UTF8);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("[csv_ControllerLog] Failed to write TotalIdle to header: " + e.Message);
        }
    }

    private float ReadTotalIdleFromHeader(string filePath)
    {
        try
        {
            using (var sr = new StreamReader(filePath, Encoding.UTF8))
            {
                string header = sr.ReadLine(); // 1行目
                if (string.IsNullOrEmpty(header)) return 0f;

                var cols = header.Split(',');
                if (cols.Length >= 11)
                {
                    float k;
                    if (float.TryParse(cols[10], NumberStyles.Float, CultureInfo.InvariantCulture, out k))
                        return k;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("[csv_ControllerLog] Failed to read TotalIdle from header: " + e.Message);
        }
        return 0f;
    }
}
