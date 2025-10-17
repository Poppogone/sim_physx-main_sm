using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class operation_state_check : MonoBehaviour
{
    [SerializeField] private Transform zx120;

    public int area_now = 0;
    public int area_now_past = 0;
    public static int areaNow = 0;

    GameObject csv_writer;

    // area1の範囲（X:20以上28未満, Z:3以上8未満）
    private float[] area1_x = new float[] { 17.0f, 25.0f };
    private float[] area1_z = new float[] { 3.0f, 8.0f };

    void Start()
    {
        csv_writer = GameObject.Find("CSV writer");
    }

    void Update()
    {
        // デフォルトはarea0
        area_now = 0;

        // area1の範囲に入ったらarea1に設定
        if (zx120.position.x >= area1_x[0] && zx120.position.x < area1_x[1])
        {
            if (zx120.position.z >= area1_z[0] && zx120.position.z < area1_z[1])
            {
                area_now = 1;
            }
        }

        areaNow = area_now;

        // 状態が変化した時のみログを記録
        if (area_now != area_now_past)
        {
            csv_writer.GetComponent<csv_operation_state>().OperationState();
        }

        area_now_past = area_now;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // Area1：赤
        DrawAreaGizmo(new Vector2(area1_x[0], area1_z[0]), new Vector2(area1_x[1], area1_z[1]), Color.red);
    }

    void DrawAreaGizmo(Vector2 bottomLeft, Vector2 topRight, Color color)
    {
        Gizmos.color = color;
        Vector3 center = new Vector3((bottomLeft.x + topRight.x) / 2f, 0.1f, (bottomLeft.y + topRight.y) / 2f);
        Vector3 size = new Vector3(Mathf.Abs(topRight.x - bottomLeft.x), 0.2f, Mathf.Abs(topRight.y - bottomLeft.y));
        Gizmos.DrawWireCube(center, size);
    }

    public void RecordSAGATEvent(string eventType)
        {
            if (csv_writer == null) csv_writer = GameObject.Find("CSV writer");
            if (csv_writer == null) return;

            var csv = csv_writer.GetComponent<csv_operation_state>();
            if (csv == null) return;

            csv.WriteCustomEvent(eventType);
            Debug.Log($"[operation_state_check] Recorded {eventType} at time={Time.realtimeSinceStartup:F2}");
        }
}
