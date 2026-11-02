using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float pan = 45;
    public float tilt = 45;
    public float dir = 10;
    public float rot = 10;
    public bool isDrone;
    public GameObject bodyLink;
    public GameObject zx120;
    public GameObject baseLink;

    [Min(0f)]
    [Tooltip("機体への追従遅延（シミュレーション秒）。0で即時追従。開始直後や値を増やした直後は、履歴がたまるまで最も古い機体位置・向きを使用します。")]
    public float followDelaySeconds = 2f;

    private struct FollowSample
    {
        public double time;
        public Vector3 centerPosition;
        public float machineYaw;
    }

    private readonly Queue<FollowSample> followHistory = new Queue<FollowSample>();
    private FollowSample previousFollowSample;
    private double lastFollowSampleTime;
    private bool hasFollowSample;


    int flag_up = 0;
    int flag_down = 0;
    int flag_right = 0;
    int flag_left = 0;
    int flag_upright = 0;
    int flag_upleft = 0;
    int flag_downright = 0;
    int flag_downleft = 0;
    int flag_zoomin = 0;
    int flag_zoomout = 0;

    private Camera cam;
    float m_FieldOfView = 40;


    public GameObject centerPoint;
    Vector3 poolPos;

    public void Move_up()
    {
        flag_up = 1;
    }

    public void Move_down()
    {
        flag_down = 1;
    }

    public void Move_right()
    {
        flag_right = 1;
    }

    public void Move_left()
    {
        flag_left = 1;
    }

    public void Move_upright()
    {
        flag_upright = 1;
    }

    public void Move_upleft()
    {
        flag_upleft = 1;
    }

    public void Move_downright()
    {
        flag_downright = 1;
    }

    public void Move_downleft()
    {
        flag_downleft = 1;
    }

    public void Zoom_in()
    {
        flag_zoomin = 1;
    }

    public void Zoom_out()
    {
        flag_zoomout = 1;
    }




    public void Move_stop()
    {
        flag_up = 0;
        flag_down = 0;
        flag_right = 0;
        flag_left = 0;
        flag_upright = 0;
        flag_upleft = 0;
        flag_downright = 0;
        flag_downleft = 0;
        flag_zoomin = 0;
        flag_zoomout = 0;
    }

    public void Move_horstop()
    {
        flag_right = 0;
        flag_left = 0;
    }

    public void Move_verstop()
    {
        flag_up = 0;
        flag_down = 0;
    }

    public void Zoom_stop()
    {
        flag_zoomin = 0;
        flag_zoomout = 0;
    }

    float x;
    float y;
    float z;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = m_FieldOfView;
        // centerPoint = GameObject.Find("BallPoolPos");
        poolPos = centerPoint.transform.position;


        float pan_rad;
        if (isDrone == true)
        {
            pan_rad = (pan - bodyLink.transform.localEulerAngles.y - zx120.transform.localEulerAngles.y - baseLink.transform.localEulerAngles.y) / 180 * Mathf.PI;
        }
        else
        {
            pan_rad = pan / 180 * Mathf.PI;
        }
        float tilt_rad = tilt / 180 * Mathf.PI;

        x = -dir * Mathf.Sin(tilt_rad) * Mathf.Sin(pan_rad) + poolPos.x;
        y = dir * Mathf.Cos(tilt_rad) + poolPos.y;
        z = dir * Mathf.Sin(tilt_rad) * Mathf.Cos(pan_rad) + poolPos.z;

        this.transform.position = new Vector3(x, y, z);
        this.transform.LookAt(centerPoint.transform);

    }

    // Update is called once per frame
    void Update()
    {

        if (isDrone == false)
        {
            if (flag_right == 1)
            {
                transform.Rotate(Vector3.up * rot * Time.deltaTime, Space.World);
            }

            if (flag_left == 1)
            {
                transform.Rotate(Vector3.down * rot * Time.deltaTime, Space.World);
            }

            if (flag_up == 1)
            {
                transform.Rotate(Vector3.left * rot * Time.deltaTime);
            }

            if (flag_down == 1)
            {
                transform.Rotate(Vector3.right * rot * Time.deltaTime);
            }

            if (flag_upright == 1)
            {
                transform.Rotate(Vector3.up * rot * Time.deltaTime, Space.World);
                transform.Rotate(Vector3.left * rot * Time.deltaTime);
            }

            if (flag_downright == 1)
            {
                transform.Rotate(Vector3.up * rot * Time.deltaTime, Space.World);
                transform.Rotate(Vector3.right * rot * Time.deltaTime);
            }

            if (flag_upleft == 1)
            {
                transform.Rotate(Vector3.left * rot * Time.deltaTime);
                transform.Rotate(Vector3.down * rot * Time.deltaTime, Space.World);
            }

            if (flag_downleft == 1)
            {
                transform.Rotate(Vector3.right * rot * Time.deltaTime);
                transform.Rotate(Vector3.down * rot * Time.deltaTime, Space.World);
            }

            if (flag_zoomin == 1)
            {
                m_FieldOfView -= Time.deltaTime * 5;
                cam.fieldOfView = m_FieldOfView;
            }

            if (flag_zoomout == 1)
            {
                m_FieldOfView += Time.deltaTime * 5;
                cam.fieldOfView = m_FieldOfView;
            }

        }
        else
        {
            if (flag_right == 1)
            {
                pan += Time.deltaTime * rot;
            }

            if (flag_left == 1)
            {
                pan -= Time.deltaTime * rot;
            }

            if (flag_up == 1)
            {
                tilt -= Time.deltaTime * rot;
            }

            if (flag_down == 1)
            {
                tilt += Time.deltaTime * rot;
            }

            if (flag_upright == 1)
            {
                pan += Time.deltaTime * rot;
                tilt -= Time.deltaTime * rot;
            }

            if (flag_downright == 1)
            {
                pan += Time.deltaTime * rot;
                tilt += Time.deltaTime * rot;
            }

            if (flag_upleft == 1)
            {
                pan -= Time.deltaTime * rot;
                tilt -= Time.deltaTime * rot;
            }

            if (flag_downleft == 1)
            {
                pan -= Time.deltaTime * rot;
                tilt += Time.deltaTime * rot;
            }

            if (flag_zoomin == 1)
            {
                dir -= Time.deltaTime * 2;
            }

            if (flag_zoomout == 1)
            {
                dir += Time.deltaTime * 2;
            }

            // Apply the delayed world-space pose in LateUpdate after the machine updates.



        }

    }

    void OnEnable()
    {
        followHistory.Clear();
        hasFollowSample = false;
    }

    void OnDisable()
    {
        followHistory.Clear();
        hasFollowSample = false;
    }

    void LateUpdate()
    {
        if (!isDrone)
        {
            followHistory.Clear();
            hasFollowSample = false;
            return;
        }

        double now = Time.timeAsDouble;
        FollowSample current = new FollowSample
        {
            time = now,
            centerPosition = centerPoint.transform.position,
            machineYaw = bodyLink.transform.localEulerAngles.y
                + zx120.transform.localEulerAngles.y + baseLink.transform.localEulerAngles.y
        };

        if (!hasFollowSample)
        {
            previousFollowSample = current;
            lastFollowSampleTime = now;
            hasFollowSample = true;
        }
        else if (now > lastFollowSampleTime)
        {
            // Do not accumulate duplicate samples while the simulation is paused.
            followHistory.Enqueue(current);
            lastFollowSampleTime = now;
        }

        double followTime = now - Mathf.Max(0f, followDelaySeconds);
        while (followHistory.Count > 0 && followHistory.Peek().time <= followTime)
        {
            // Retain one sample before the requested time for interpolation.
            previousFollowSample = followHistory.Dequeue();
        }

        FollowSample delayed = previousFollowSample;
        if (followHistory.Count > 0)
        {
            FollowSample next = followHistory.Peek();
            float t = Mathf.Clamp01((float)((followTime - delayed.time) / (next.time - delayed.time)));
            delayed.centerPosition = Vector3.Lerp(delayed.centerPosition, next.centerPosition, t);
            delayed.machineYaw = Mathf.LerpAngle(delayed.machineYaw, next.machineYaw, t);
        }

        // Keep the existing distance, pan and tilt controls; delay only machine motion.
        // Use the historical look-at point too, so rotation does not follow immediately.
        poolPos = delayed.centerPosition;
        float pan_rad = (pan - delayed.machineYaw) / 180 * Mathf.PI;
        float tilt_rad = tilt / 180 * Mathf.PI;

        x = dir * Mathf.Sin(tilt_rad) * Mathf.Sin(pan_rad) + poolPos.x;
        y = dir * Mathf.Cos(tilt_rad) + poolPos.y;
        z = -dir * Mathf.Sin(tilt_rad) * Mathf.Cos(pan_rad) + poolPos.z;

        // Override inherited parent motion in world space before the camera renders.
        transform.position = new Vector3(x, y, z);
        transform.LookAt(poolPos);
    }

}
