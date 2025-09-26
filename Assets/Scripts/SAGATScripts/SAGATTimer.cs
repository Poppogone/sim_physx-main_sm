using UnityEngine;
using UnityEngine.UI;

public class SAGATTimer : MonoBehaviour
{
    public Button startButton;        // Startボタン
    public float limitSeconds = 180f; // 制限時間（秒数、デフォルト3分）
    public Text timerText;            // 残り時間を表示するUI Text

    private float startTime = 0f;
    private bool isRunning = false;
    private SAGATSaver saver;

    void Start()
    {
        saver = FindObjectOfType<SAGATSaver>();
        if (startButton != null)
            startButton.onClick.AddListener(StartTimer);

        if (timerText != null)
            timerText.text = FormatTime(limitSeconds); // 初期表示
    }

    void Update()
    {
        if (isRunning && saver != null && !saver.IsAlreadySaved())
        {
            float elapsed = Time.time - startTime;
            float remaining = Mathf.Max(0, limitSeconds - elapsed);

            // 残り時間を表示
            if (timerText != null)
                timerText.text = FormatTime(remaining);

            if (elapsed >= limitSeconds)
            {
                Debug.Log("[SAGATTimer] 制限時間に到達、自動保存を実行します");
                saver.SaveAndEvaluate();
                isRunning = false;

                // 残り時間を0に固定表示
                if (timerText != null)
                    timerText.text = "00:00";
            }
        }
    }

    void StartTimer()
    {
        startTime = Time.time;
        isRunning = true;
        Debug.Log("[SAGATTimer] Startボタン押下、タイマー開始");
    }

    // 秒数を mm:ss 形式に変換
    private string FormatTime(float seconds)
    {
        int min = Mathf.FloorToInt(seconds / 60f);
        int sec = Mathf.FloorToInt(seconds % 60f);
        return string.Format("{0:00}:{1:00}", min, sec);
    }
}
