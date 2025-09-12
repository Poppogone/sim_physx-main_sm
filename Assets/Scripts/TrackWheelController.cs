using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class TrackWheelController : MonoBehaviour
{
    public string leftTrackTopic = "track_left";
    public string rightTrackTopic = "track_right";
    [Tooltip("|cmd|=1 の片側最大速度 [m/s]")]
    public float maxLinearSpeed = 1.0f;
    [Tooltip("左右クローラ間距離 [m]")]
    public float wheelBase = 2.0f;

    private float leftVel = 0f;
    private float rightVel = 0f;

    // 内部で位置とヨー角を保持
    private Vector3 posWorld;
    private float yawRad;

    void Start()
    {
        var ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<Float64Msg>(leftTrackTopic, msg => leftVel = (float)msg.data);
        ros.Subscribe<Float64Msg>(rightTrackTopic, msg => rightVel = (float)msg.data);

        // 初期値を現在のTransformから取得
        posWorld = transform.position;
        yawRad   = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        // 左右トラックの速度 [m/s]
        float vL = leftVel * maxLinearSpeed;
        float vR = rightVel * maxLinearSpeed;

        // 差動駆動モデル
        float v = (vL + vR) / 2f;                  // 前進速度
        float omega = (vR - vL) / wheelBase;       // 角速度 [rad/s]

        // 積分して位置・姿勢を更新
        yawRad += omega * dt;
        Vector3 forward = new Vector3(Mathf.Sin(yawRad), 0f, Mathf.Cos(yawRad));
        posWorld += forward * (v * dt);

        // === 最後に Transform に反映 ===
        Quaternion rot = Quaternion.Euler(0f, yawRad * Mathf.Rad2Deg, 0f);
        transform.SetPositionAndRotation(posWorld, rot);
        
        // base_link の位置も強制的に同期
        var baseLink = transform.Find("base_link");
        if (baseLink != null)
        {
            baseLink.SetPositionAndRotation(posWorld, rot);
        }
    }

}
