using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using UnityEngine.UI;

public class headJoystick : MonoBehaviour
{
    private ROSConnection ros;
    public string setpointTopicName = "right_js_sw2";
    private Float64Msg jsInput;

    [SerializeField] private Transform target;
    [SerializeField] private Transform bodyL;
    [SerializeField] private Transform baseL;
    public float r = 8;
    public float theta = 60;
    public float phi = -135;

    float r_first;
    float theta_first;
    float phi_first;

    float x;
    float y;
    float z;

    Vector3 headPosFirst;
    Vector3 headAngFirst;

    float zoomGain = 400.0f/3.0f;
    float thetaGain = 1.0f/75.0f;
    float phiGain = 1.0f/75.0f;

    float zoomDead = 0.05f;
    float phiDead = 7.5f;
    float thetaDead = 7.5f;

    float zoom_max = 7.0f;
    float phi_max = 30.0f;
    float theta_max = 50.0f;

    float dhrx;
    float dhry;
    float dhz;

    bool IsRegistered = false;

    public GameObject yellow_camera;
    public GameObject black_camera;
    public bool IsFree = false;
    public Vector3 headPos;
    public Vector3 headAng;
    [SerializeField] private GameObject head_button;
    [SerializeField] private Image start_button;
    [SerializeField] private Image debug_button;

    [SerializeField] private GameObject op_state;
    [SerializeField] private bool viewpoint_sw = true;

    int area_now;

    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<Float64Msg>(setpointTopicName, ExecuteJointPosControl);
        jsInput = new Float64Msg();
        jsInput.data = 0.0f;

        r_first = r;
        theta_first = theta;
        phi_first = phi;        

        x = -r * Mathf.Sin(theta/180*Mathf.PI) * Mathf.Sin(phi/180*Mathf.PI);
        y = r * Mathf.Cos(theta/180*Mathf.PI);
        z = r * Mathf.Sin(theta/180*Mathf.PI) * Mathf.Cos(phi/180*Mathf.PI);
        transform.localPosition = new Vector3(x, y, z);
        transform.LookAt(target);

    }

    // Update is called once per frame
    void Update()
    {
        operation_state_check script;
        script = op_state.GetComponent<operation_state_check>();
        area_now = script.area_now;
        if (area_now > 0 && viewpoint_sw)
        {
            yellow_camera.SetActive(true);
            black_camera.SetActive(false);
            IsFree = true;
        }
        if (area_now == 0 || !viewpoint_sw)
        {
            yellow_camera.SetActive(false);
            black_camera.SetActive(true);
            IsFree = false;            
        }

        GazePoint gazePoint = TobiiAPI.GetGazePoint();
        HeadPose headPose = TobiiAPI.GetHeadPose();

        headPos = headPose.Position;
        headAng = headPose.Rotation.eulerAngles;
        headAng = AdjustAngle(headAng);

        if (gazePoint.IsRecent() && headPose.IsRecent() && IsRegistered && IsFree)
        {

            //ズーム
            dhz = headPos.z - headPosFirst.z;
            if (dhz >= zoomDead)
            {
                if (dhz >= 0.1f)
                {
                    r = r - ((zoom_max*(40.0f*(dhz - 0.15f)/3.0f)) + zoom_max)*Time.deltaTime;
                }
                else
                {
                    r = r - zoomGain*zoom_max*Mathf.Pow(dhz-zoomDead,2.0f)*Time.deltaTime;
                }
            }
            if (dhz <= -zoomDead)
            {
                if (dhz <= -0.1f)
                {
                    r = r - ((zoom_max*(40.0f*(dhz + 0.15f)/3.0f)) - zoom_max)*Time.deltaTime;
                }
                else
                {
                    r = r + zoomGain*zoom_max*Mathf.Pow(dhz+zoomDead,2.0f)*Time.deltaTime;
                }
            }
            r = Mathf.Clamp(r, 3.0f, r_first +2.0f);

            //θ
            dhrx = headAng.x - headAngFirst.x;
            if (dhrx >= thetaDead)
            {
                if (dhrx >= 15.0f)
                {
                    theta = theta - ((theta_max*(2.0f*(dhrx - 20.0f)/15.0f)) + theta_max)*Time.deltaTime;
                }
                else
                {
                    theta = theta - thetaGain*theta_max*Mathf.Pow(dhrx-thetaDead,2.0f)*Time.deltaTime;
                }
            }
            if (dhrx <= -thetaDead)
            {
                if (dhrx <= -15.0f)
                {
                    theta = theta - ((theta_max*(2.0f*(dhrx + 20.0f)/15.0f)) - theta_max)*Time.deltaTime;
                }
                else
                {
                    theta = theta + thetaGain*theta_max*Mathf.Pow(dhrx+thetaDead,2.0f)*Time.deltaTime;
                }
            }
            theta = Mathf.Clamp(theta, theta_first - 55.0f , theta_first + 29.0f); 

            //φ
            dhry = headAng.y - headAngFirst.y;
            if (dhry >= phiDead)
            {
                if (dhry >= 15.0f)
                {
                    phi = phi - ((phi_max*(2.0f*(dhry - 20.0f)/15.0f)) + phi_max)*Time.deltaTime;
                }
                else
                {
                    phi = phi - phiGain*phi_max*Mathf.Pow(dhry-phiDead,2.0f)*Time.deltaTime;
                }
            }
            if (dhry <= -phiDead)
            {
                if (dhry <= -15.0f)
                {
                    phi = phi - ((phi_max*(2.0f*(dhry + 20.0f)/15.0f)) - phi_max)*Time.deltaTime;
                }
                else
                {
                    phi = phi + phiGain*phi_max*Mathf.Pow(dhry+phiDead,2.0f)*Time.deltaTime;
                }
            }
            phi = Mathf.Clamp(phi, phi_first - 45.0f, phi_first + 90.0f);
            Debug.Log("PHIII  " + dhrx);
        }

        if (jsInput.data != 0)
        {
            r = r_first;
            theta = theta_first;
            phi = phi_first;
        }
        if(!IsFree)
        {
            r = r_first;
            theta = theta_first;
            phi = phi_first;
        }

        x = -r * Mathf.Sin(theta/180*Mathf.PI) * Mathf.Sin(phi/180*Mathf.PI);
        y = r * Mathf.Cos(theta/180*Mathf.PI);
        z = r * Mathf.Sin(theta/180*Mathf.PI) * Mathf.Cos(phi/180*Mathf.PI);
        transform.localPosition = new Vector3(x, y, z);
        transform.LookAt(target);
    }

    private Vector3 AdjustAngle(Vector3 angle)
    {

        if(angle.x > 180)
        {
            angle.x = angle.x - 360f;
        }
        if(angle.y > 180)
        {
            angle.y = angle.y - 360f;
        }
        if(angle.z > 180)
        {
            angle.z = angle.z - 360f;
        }
        return angle;
    }

    void ExecuteJointPosControl(Float64Msg msg)
    {
        jsInput = msg;
        Debug.Log("right_js_sw2: " + jsInput.data);
    }

    public void HeadSetting()
    {
        GazePoint gazePoint = TobiiAPI.GetGazePoint();
        HeadPose headPose = TobiiAPI.GetHeadPose();
        Debug.Log("head_set___1");
        if (gazePoint.IsRecent() && headPose.IsRecent())
        {
            Debug.Log("head_set___2");
            headPosFirst = headPose.Position;
            headAngFirst = headPose.Rotation.eulerAngles;
            headAngFirst = AdjustAngle(headAngFirst);
            IsRegistered = true;
            Debug.Log("head_set___3");
            head_button.SetActive(false);
            start_button.color = new Color(255.0f/255.0f, 49.0f/255.0f, 36.0f/255.0f, 213.0f/255.0f);
            debug_button.color = new Color(255.0f/255.0f, 100.0f/255.0f, 36.0f/255.0f, 213.0f/255.0f);
        }
    } 
}
