using UnityEngine;
using UnityEngine.UI;   // UI Text 用
// using TMPro;         // TextMeshPro を使うならコメントアウト外す

public class RaceTimer : MonoBehaviour
{
    public static RaceTimer Instance { get; private set; }

    public Text timerText;   // 普通のUI Text
    // public TMP_Text timerText; // TextMeshProならこちらに切り替え

    private float startTime;
    private bool running = false;
    private float finalTime = 0f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // 初期表示を 0.00 にする
        if (timerText != null)
            timerText.text = "0.00";
    }

    void Update()
    {
        if (running && timerText != null)
        {
            float elapsed = Time.time - startTime;
            // 分は不要なので「秒.コンマ秒」のみ
            timerText.text = string.Format("{0:0.00}", elapsed);
        }
    }

    public void StartTimer()
    {
        startTime = Time.time;
        running = true;
        finalTime = 0f;
    }

    public float StopTimer()
    {
        running = false;
        finalTime = Time.time - startTime;
        if (timerText != null)
            timerText.text = string.Format("{0:0.00}", finalTime); // 終了時の結果を残す
        return finalTime;
    }

    public void ResetTimer()
    {
        running = false;
        finalTime = 0f;
        if (timerText != null)
            timerText.text = "0.00";
    }
}
