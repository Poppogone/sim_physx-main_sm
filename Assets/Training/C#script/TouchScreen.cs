using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using UnityEngine.UI;

public class TouchScreen : MonoBehaviour
{
    Vector2 currentPos;
    Vector2 prePos;
    Vector2 dif;
    Vector2 difAvg;
    Vector2 speed;

    Vector3 cameraPos;
    public int positionCount = 5;
    private List<Vector2> difList;
    public float moveRate = 0.02f;
    public float speedRate = 0.0001f;
    public float zoomRate = 50f;
    public float panRate = 0.02f;
    public float tiltRate = 0.02f;
    public float panSpeedRate = 0.02f;
    public float tiltSpeedRate = 0.02f;
    public float attenuationRate = 0.98f;

    public float max = 20.0f;

    float dist0 = 0f;
    float dist1 = 0f;
    float scale = 0f;
    float oldDist = 0f;//�O���2�_�Ԃ̋���
    float minRate = 0.7f;
    float maxRate = 3f;

    private Camera cam;
    float m_FieldOfView = 60;

    public float pan = 45;
    public float tilt = 60;
    public float dir = 8;
    public float rot = 10;
    public bool isDrone;
    public GameObject bodyLink;
    public GameObject zx120;
    public GameObject baseLink;

    float pan_first;
    float tilt_first;
    float dir_first;

    public bool IsFree = false;

    [SerializeField] private GameObject op_state;
    [SerializeField] private bool viewpoint_sw = true;

    int area_now;

    private ROSConnection ros;
    public string setpointTopicName = "right_js_sw2";
    private Float64Msg jsInput;

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

    public GameObject centerPoint;
    Vector3 poolPos;

    float x;
    float y;
    float z;

    void Swipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            cameraPos = transform.position;
            speed.x = 0;
            speed.y = 0;
            currentPos = Input.mousePosition;
            prePos = currentPos;

        }
        else if (Input.GetMouseButton(0))
        {
            currentPos = Input.mousePosition;
            dif = currentPos - prePos;

            if (dif.magnitude > max)
            {
                prePos = currentPos;
                return;
            }

            difList.Add(dif);

            if (difList.Count > positionCount)
            {
                difList.RemoveAt(0); // ���X�g���w��̐��𒴂����ꍇ�A�Â��ʒu���폜
            }

            difAvg = CalculateAveragePosition();

            speed.x = difAvg.x / Time.deltaTime * speedRate;
            speed.y = difAvg.y / Time.deltaTime * speedRate;

            pan -= dif.x * panRate;
            tilt += dif.y * tiltRate;

            prePos = currentPos;
            Debug.Log("x: " + currentPos.x + "y: " + currentPos.y);
        }
        else
        {

            pan -= speed.x * panSpeedRate;
            tilt += speed.y * tiltSpeedRate;
            transform.Translate(-speed.x, -speed.y, 0);
            speed.x *= attenuationRate;
            speed.y *= attenuationRate;
            //Debug.Log("good");
        }
        //Debug.Log("x:" + dif.x + "y:" + dif.y + "camerax:" + cameraPos.x + "cameray" + cameraPos.y + "speedx:" + speed.x + "speed.y" + speed.y);

    }

    private Vector2 CalculateAveragePosition()
    {
        Vector2 sum = Vector2.zero;

        foreach (Vector2 position in difList)
        {
            sum += position;
        }

        return sum / difList.Count;
    }

    void PinchInOut()
    {
        if (Input.touchCount >= 2)
        {
            Touch t1 = Input.GetTouch(0);
            Touch t2 = Input.GetTouch(1);
            if (t2.phase == TouchPhase.Began)
            {
                dist0 = Vector2.Distance(t1.position, t2.position);
                oldDist = dist0;
            }
            else if (t1.phase == TouchPhase.Moved || t2.phase == TouchPhase.Moved)
            {
                dist1 = Vector2.Distance(t1.position, t2.position);
                if (dist0 < 0.001f || dist1 < 0.001f)
                {
                    return;
                }
                else
                {
                    dir -= (dist1 - oldDist) / zoomRate;
                    oldDist = dist1;

                }


                //m_FieldOfView -= Time.deltaTime * 5;
                //cam.fieldOfView = m_FieldOfView;
                //transform.localScale = new Vector3(scale, scale, scale);
            }
        }
    }

    void ExecuteJointPosControl(Float64Msg msg)
    {
        jsInput = msg;
        Debug.Log("right_js_sw2: " + jsInput.data);
    }

    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<Float64Msg>(setpointTopicName, ExecuteJointPosControl);
        jsInput = new Float64Msg();
        jsInput.data = 0.0f;

        pan_first = pan;
        tilt_first = tilt;
        dir_first = dir;

        cameraPos = transform.position;
        difList = new List<Vector2>();

        cam = GetComponent<Camera>();
        cam.fieldOfView = m_FieldOfView;

        poolPos = centerPoint.transform.position;

        float pan_rad;
        if (isDrone == true)
        {
            pan_rad = (180+pan - bodyLink.transform.localEulerAngles.y - zx120.transform.localEulerAngles.y - baseLink.transform.localEulerAngles.y) / 180 * Mathf.PI;
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

        operation_state_check script;
        script = op_state.GetComponent<operation_state_check>();
        area_now = script.area_now;

        if (area_now > 0 && viewpoint_sw)
        {
            IsFree = true;
        }
        if (area_now == 0 || !viewpoint_sw)
        {
            IsFree = false;            
        }

        if(IsFree){
           Swipe();
           PinchInOut(); 
        }

        if(!IsFree){
            pan = pan_first;
            tilt = tilt_first;
            dir = dir_first;
        }
        if(jsInput.data !=0){
            pan = pan_first;
            tilt = tilt_first;
            dir = dir_first;
        }

        dir = Mathf.Clamp(dir, 3, dir_first +2.0f);
        pan = Mathf.Clamp(pan,pan_first-45.0f,pan_first+90.0f);
        tilt = Mathf.Clamp(tilt,tilt_first - 55.0f,tilt_first + 29.0f);
        
        poolPos = centerPoint.transform.position;
        float pan_rad = (180 + pan - bodyLink.transform.localEulerAngles.y - zx120.transform.localEulerAngles.y - baseLink.transform.localEulerAngles.y) / 180 * Mathf.PI;
        float tilt_rad = tilt / 180 * Mathf.PI;

        x = -dir * Mathf.Sin(tilt_rad) * Mathf.Sin(pan_rad) + poolPos.x;
        y = dir * Mathf.Cos(tilt_rad) + poolPos.y;
        z = dir * Mathf.Sin(tilt_rad) * Mathf.Cos(pan_rad) + poolPos.z;

        this.transform.position = new Vector3(x, y, z);
        this.transform.LookAt(centerPoint.transform);

    }

    
}
