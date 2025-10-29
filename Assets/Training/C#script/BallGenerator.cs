using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGenerator : MonoBehaviour
{
    public GameObject ballPrefab;
    public int n = 1800;               // 総ボール数
    public float radius = 0.05f;       // ボール半径
    public float depth = 5.0f;         // 散布範囲の奥行き
    public float width = 1.5f;         // 散布範囲の幅

    [Header("生成中心(2点) ※Yは無視してXZのみ使用")]
    public Vector3 center1 = new Vector3(22f, 0f, 1.5f);
    //public Vector3 center2 = new Vector3(25f, 0f, 3.5f);

    void Start()
    {
        int n1 = n / 2;     // 1箇所目に落とす数
        int n2 = n - n1;    // 2箇所目に落とす数（端数はこちらへ）

        SpawnAt(center1, n1);
        //SpawnAt(center2, n2);
    }

    // 指定中心の周囲にボールを散布生成
    void SpawnAt(Vector3 center, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject ball = Instantiate(ballPrefab);

            // 散布範囲（ランダム）
            float x = Random.Range(-width / 2.0f + radius, width / 2.0f - radius);
            float y = Random.Range(radius - 0.25f, 4.0f);
            float z = Random.Range(-depth / 2.0f + radius, depth / 2.0f - radius);

            // 実際の位置を決定
            ball.transform.position = new Vector3(center.x + x, y, center.z + z);
        }
    }
}
