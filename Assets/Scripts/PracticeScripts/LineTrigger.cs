using UnityEngine;


public class LineTrigger : MonoBehaviour
{
    public enum LineType { Start, Finish }
    [Header("ライン種別")]
    public LineType lineType = LineType.Start;

    private bool check = false;
    private bool sw = true;

    void Update()
    {
        if (check && sw)
        {
            if (RaceTimer.Instance == null)
            {
                Debug.LogWarning("[LineTrigger] RaceTimer.Instance が見つかりません。");
                return;
            }

            if (lineType == LineType.Start)
            {
                RaceTimer.Instance.StartTimer();
                Debug.Log("[LineTrigger] StartLine 衝突 → 計測開始");
            }
            else if (lineType == LineType.Finish)
            {
                float final = RaceTimer.Instance.StopTimer();
                Debug.Log($"[LineTrigger] FinishLine 衝突 → 計測終了: {final:F3} 秒");
            }

            sw = false;  // 一度きりにする
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // start_check.cs と同じく特定タグだけ無視する
        if (collision.gameObject.tag != "Obstacle" &&
            collision.gameObject.tag != "Ball" &&
            collision.gameObject.tag != "Stone")
        {
            check = true;
            Debug.Log("[LineTrigger] OnCollisionEnter → check=true");
        }
    }
}


