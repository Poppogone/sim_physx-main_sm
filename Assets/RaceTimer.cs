using UnityEngine;
using UnityEngine.UI;
// TextMeshPro を使うならコメントアウト解除
// using TMPro;

public class RaceTimer : MonoBehaviour
{
    public static RaceTimer Instance { get; private set; }

    [Header("UI")]
    [Tooltip("UI Text (Text または TextMeshProUGUI のどちらか片方だけ割り当て)")]
    public Text timeText;                 // 旧 Text
    // public TextMeshProUGUI tmpTimeText; // TMP を使う場合はこちら

    [Header("Settings")]
    public bool autoStart = false;        // 再生と同時に開始するなら ON
    public bool useUnscaledTime = false;  // ポーズ中も進めたいなら ON（Time.timeScale の影響なし）

    bool running;
    bool finished;
    bool paused;
    float startTime;      // 計測開始した「ゲーム時間/非スケール時間」
    float pausedAt;       // 一時停止した時刻
    float pausedOffset;   // 一時停止の累計（計測から除外）

    float ElapsedNow =>
        (useUnscaledTime ? Time.unscaledTime : Time.time) - startTime - pausedOffset;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        SetText("0:00.000");
        if (autoStart) StartTimer();
    }

    void Update()
    {
        if (!running || paused) return;

        float t = Mathf.Max(0f, ElapsedNow);
        SetText(Format(t));
    }

    //=== Public API ===//
    public void StartTimer()
    {
        if (running) return;

        float now = useUnscaledTime ? Time.unscaledTime : Time.time;
        startTime = now;
        pausedOffset = 0f;
        paused = false;
        finished = false;
        running = true;

        SetText("0:00.000");
        // Debug.Log("[RaceTimer] Start");
    }

    public float StopTimer()
    {
        if (!running) return 0f;
        running = false;
        finished = true;

        float t = Mathf.Max(0f, ElapsedNow);
        SetText(Format(t));
        // Debug.Log($"[RaceTimer] Finish {Format(t)}");
        return t;
    }

    public void ResetTimer()
    {
        running = false;
        finished = false;
        paused = false;
        startTime = 0f;
        pausedAt = 0f;
        pausedOffset = 0f;
        SetText("0:00.000");
        // Debug.Log("[RaceTimer] Reset");
    }

    public void PauseTimer()
    {
        if (!running || paused) return;
        paused = true;
        pausedAt = useUnscaledTime ? Time.unscaledTime : Time.time;
        // Debug.Log("[RaceTimer] Pause");
    }

    public void ResumeTimer()
    {
        if (!running || !paused) return;
        float now = useUnscaledTime ? Time.unscaledTime : Time.time;
        pausedOffset += now - pausedAt; // 停止していた分を加算して経過から除外
        paused = false;
        // Debug.Log("[RaceTimer] Resume");
    }

    //=== Helpers ===//
    string Format(float t)
    {
        int m = Mathf.FloorToInt(t / 60f);
        float s = t % 60f;
        return $"{m}:{s:00.000}";
    }

    void SetText(string s)
    {
        if (timeText) timeText.text = s;
        // if (tmpTimeText) tmpTimeText.text = s; // TMP 使用時
    }
}
