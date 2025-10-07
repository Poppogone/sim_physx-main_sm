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

        // ★ 「6つすべてが0（≒しきい値以内）」ならアイドル
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

        // 1行出力（H 列が IdleTime）
        string[] row = {
            timeNow.ToString(CultureInfo.InvariantCulture),
            crawlerL.ToString(CultureInfo.InvariantCulture),
            crawlerR.ToString(CultureInfo.InvariantCulture),
            turn.ToString(CultureInfo.InvariantCulture),
            arm.ToString(CultureInfo.InvariantCulture),
            boom.ToString(CultureInfo.InvariantCulture),
            bucket.ToString(CultureInfo.InvariantCulture),
            idleAccum.ToString("F3", CultureInfo.InvariantCulture)
        };
        sw.WriteLine(string.Join(",", row));

        nextSampleTime += sampleInterval;
    }

    // ===== ここから：新規作成 / 一時停止 / 再開 =====

    public void MakeFile(string baseName)
    {
        if (!enabledLogging) return;
        if (IsCheck) return;  // 冪等ガード

        string dir = Path.Combine(Application.dataPath, "ExperimentData_2025");
        Directory.CreateDirectory(dir);

        path = Path.Combine(dir, baseName + "_ControllerLog.csv");

        var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
        sw = new StreamWriter(fs, new UTF8Encoding(false)) { AutoFlush = true };

        // ヘッダ行（K列はラベル。値は終了時に上書き）
        string[] header = {
            "Time","CrawlerL","CrawlerR","Turn","Arm","Boom","Bucket",
            "IdleTime","","","TotalIdleTime"
        };
        sw.WriteLine(string.Join(",", header));

        IsCheck = true;
        timeStart = Time.realtimeSinceStartup;
        nextSampleTime = Time.unscaledTime;
        idleAccum = 0f;
        totalIdleTime = 0f;

        PlayerPrefs.SetString(PP_KEY_PATH, path);
        PlayerPrefs.SetString(PP_KEY_ELAPSED, "0");
        PlayerPrefs.Save();

        Debug.Log("[csv_ControllerLog] Start logging (new): " + path);
    }

    public void PauseLogging()
    {
        if (!IsCheck || sw == null) return;

        float elapsed = Time.realtimeSinceStartup - timeStart;

        // 先に閉じる（Windowsの共有ロック回避）
        try { sw.Flush(); sw.Close(); } catch {} finally { sw = null; IsCheck = false; }

        // ヘッダK列へ書き戻し
        WriteTotalIdleToHeader(path, totalIdleTime);

        // 続き再開のため保存
        PlayerPrefs.SetString(PP_KEY_PATH, path);
        PlayerPrefs.SetString(PP_KEY_ELAPSED, elapsed.ToString("F6", CultureInfo.InvariantCulture));
        PlayerPrefs.Save();

        Debug.Log($"[csv_ControllerLog] Paused. Saved path & elapsed. (elapsed={elapsed:F3}s, totalIdle={totalIdleTime:F3}s)");
    }

    public void ResumeLogging()
    {
        if (!enabledLogging) return;
        if (IsCheck) return;

        string savedPath = PlayerPrefs.GetString(PP_KEY_PATH, "");
        string savedElapsedStr = PlayerPrefs.GetString(PP_KEY_ELAPSED, "");
        if (string.IsNullOrEmpty(savedPath) || !File.Exists(savedPath))
        {
            Debug.LogWarning("[csv_ControllerLog] ResumeLogging: saved file not found. Call MakeFile() for new logging.");
            return;
        }

        path = savedPath;

        float elapsedBefore = 0f;
        float.TryParse(savedElapsedStr, NumberStyles.Float, CultureInfo.InvariantCulture, out elapsedBefore);

        // ヘッダK列（TotalIdleTime）を読み取り、累積合計を復元（数値のみ/ラベル付きの両対応）
        totalIdleTime = ReadTotalIdleFromHeader(path);

        var fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read);
        sw = new StreamWriter(fs, new UTF8Encoding(false)) { AutoFlush = true };

        timeStart = Time.realtimeSinceStartup - elapsedBefore;
        nextSampleTime = Time.unscaledTime;
        idleAccum = 0f;

        IsCheck = true;

        Debug.Log($"[csv_ControllerLog] Resumed append: {path} (elapsedBefore={elapsedBefore:F3}s, totalIdle={totalIdleTime:F3}s)");
    }

    public void Close()
    {
        // 先にストリームを閉じる（競合対策）
        if (sw != null)
        {
            try { sw.Flush(); sw.Close(); } catch {} finally { sw = null; }
        }
        bool wasLogging = IsCheck;
        IsCheck = false;

        // ヘッダK列に書き戻し（視認性のため "TotalIdleTime=xxx" 形式）
        if (!string.IsNullOrEmpty(path))
        {
            WriteTotalIdleToHeader(path, totalIdleTime);
            Debug.Log("[csv_ControllerLog] Closed. TotalIdleTime(K) = " + totalIdleTime.ToString("F3", CultureInfo.InvariantCulture));
        }
        else if (wasLogging)
        {
            Debug.LogWarning("[csv_ControllerLog] Closed but path was empty.");
        }
    }

    // ===== ヘルパ =====

    // ヘッダ1行目のK列に "TotalIdleTime=xxx" として書き戻す（Excelで見やすく、読み戻しも容易）
    private void WriteTotalIdleToHeader(string filePath, float totalIdle)
    {
        try
        {
            var lines = File.ReadAllLines(filePath, Encoding.UTF8).ToList();
            if (lines.Count > 0)
            {
                var cols = lines[0].Split(',');
                if (cols.Length < 11) Array.Resize(ref cols, 11); // K列まで拡張

                cols[10] = $"TotalIdleTime={totalIdle.ToString("F3", CultureInfo.InvariantCulture)}"; // K列

                lines[0] = string.Join(",", cols);
                File.WriteAllLines(filePath, lines, Encoding.UTF8);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("[csv_ControllerLog] Failed to write TotalIdle to header: " + e.Message);
        }
    }

    // ヘッダK列から合計アイドルを読む（"3.940" でも "TotalIdleTime=3.940" でもOK）
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
                    string k = cols[10];
                    if (string.IsNullOrWhiteSpace(k)) return 0f;

                    // "TotalIdleTime=3.940" → 右辺だけ抜き出し
                    int eq = k.IndexOf('=');
                    if (eq >= 0 && eq < k.Length - 1) k = k.Substring(eq + 1);

                    float val;
                    if (float.TryParse(k, NumberStyles.Float, CultureInfo.InvariantCulture, out val))
                        return val;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("[csv_ControllerLog] Failed to read TotalIdle from header: " + e.Message);
        }
        return 0f;
    }

    void OnApplicationQuit() { Close(); }
    void OnDestroy()         { Close(); }
}
