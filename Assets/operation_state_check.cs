using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using RosMessageTypes.Geometry;
using UnityEngine.UI;

public class operation_state_check : MonoBehaviour
{
    //private ROSConnection ros;
    //public string TwistTopicName = "zx120/tracks/cmd_vel";

    //private TwistMsg twist;

    // 把持・配置１
    private float[] area1_x = new float[]{-13.5f, -10.0f};
    private float[] area1_z = new float[]{10.5f, 12.5f};
    // 把持・配置２
    private float[] area2_x = new float[]{-1.5f, 2.0f};
    private float[] area2_z = new float[]{6.5f, 12.0f};
    // 掘削・積込１
    private float[] area3_x = new float[]{-2.5f, 2.0f};
    private float[] area3_z = new float[]{-6.0f, -3.0f};
    // リセットエリア
    private float[] area4_x = new float[]{-27.0f, -21.0f};
    private float[] area4_z = new float[]{8.5f, 12.0f};
    // 把持・配置３
    private float[] area5_x = new float[]{-17.5f, -13.0f};
    private float[] area5_z = new float[]{-7.5f, -5.0f};
    // 把持・配置４
    private float[] area6_x = new float[]{-26.0f, -23.5f};
    private float[] area6_z = new float[]{-3.0f, 1.5f};
    // 把持・配置５
    private float[] area7_x = new float[]{-26.5f, -23.5f};
    private float[] area7_z = new float[]{28.0f, 32.0f};
    // 把持・配置６
    private float[] area8_x = new float[]{-35.0f, -30.0f};
    private float[] area8_z = new float[]{37.5f, 40.5f};
    // 掘削・積込２
    private float[] area9_x = new float[]{-50.0f, -45.0f};
    private float[] area9_z = new float[]{33.5f, 38.0f};
    // 把持・配置７
    private float[] area10_x = new float[]{-49.5f, -47.0f};
    private float[] area10_z = new float[]{22.0f, 27.5f};
    //　把持・配置８
    private float[] area11_x = new float[]{-40.0f, -35.5f};
    private float[] area11_z = new float[]{9.5f, 11.5f};

    [SerializeField] private Transform zx120;

    private bool area_0_check = true;
    
    public int area_now = 0;
    public int area_now_past = 0;
    public static int areaNow = 0;

    GameObject csv_writer;

    // Start is called before the first frame update
    void Start()
    {
        //ros = ROSConnection.GetOrCreateInstance();
        //twist = new TwistMsg();
        //ros.Subscribe<TwistMsg>(TwistTopicName, ExecuteTwist);
        csv_writer = GameObject.Find("CSV writer");
    }

    // Update is called once per frame
    void Update()
    {
        area_0_check = true;

        if(area1_x[0] < zx120.position.x && zx120.position.x < area1_x[1])
        {
            if(area1_z[0] < zx120.position.z && zx120.position.z < area1_z[1])
            {
                //area1
                area_now = 1;
                area_0_check = false;
            }
        }
        if(area2_x[0] < zx120.position.x && zx120.position.x < area2_x[1])
        {
            if(area2_z[0] < zx120.position.z && zx120.position.z < area2_z[1])
            {
                //area2
                area_now = 2;
                area_0_check = false;
            }
        }
        if(area3_x[0] < zx120.position.x && zx120.position.x < area3_x[1])
        {
            if(area3_z[0] < zx120.position.z && zx120.position.z < area3_z[1])
            {
                //area3
                area_now = 3;
                area_0_check = false;
            }
        }
        if(area4_x[0] < zx120.position.x && zx120.position.x < area4_x[1])
        {
            if(area4_z[0] < zx120.position.z && zx120.position.z < area4_z[1])
            {
                //area4
                area_now = 4;
                area_0_check = false;
            }
        }
        if(area5_x[0] < zx120.position.x && zx120.position.x < area5_x[1])
        {
            if(area5_z[0] < zx120.position.z && zx120.position.z < area5_z[1])
            {
                //area5
                area_now = 5;
                area_0_check = false;
            }
        }
        if(area6_x[0] < zx120.position.x && zx120.position.x < area6_x[1])
        {
            if(area6_z[0] < zx120.position.z && zx120.position.z < area6_z[1])
            {
                //area6
                area_now = 6;
                area_0_check = false;
            }
        }
        if(area7_x[0] < zx120.position.x && zx120.position.x < area7_x[1])
        {
            if(area7_z[0] < zx120.position.z && zx120.position.z < area7_z[1])
            {
                //area7
                area_now = 7;
                area_0_check = false;
            }
        }
        if(area8_x[0] < zx120.position.x && zx120.position.x < area8_x[1])
        {
            if(area8_z[0] < zx120.position.z && zx120.position.z < area8_z[1])
            {
                //area8
                area_now = 8;
                area_0_check = false;
            }
        }
        if(area9_x[0] < zx120.position.x && zx120.position.x < area9_x[1])
        {
            if(area9_z[0] < zx120.position.z && zx120.position.z < area9_z[1])
            {
                //area9
                area_now = 9;
                area_0_check = false;
            }
        }
        if(area10_x[0] < zx120.position.x && zx120.position.x < area10_x[1])
        {
            if(area10_z[0] < zx120.position.z && zx120.position.z < area10_z[1])
            {
                //area10
                area_now = 10;
                area_0_check = false;
            }
        }
        if(area11_x[0] < zx120.position.x && zx120.position.x < area11_x[1])
        {
            if(area11_z[0] < zx120.position.z && zx120.position.z < area11_z[1])
            {
                //area11
                area_now = 11;
                area_0_check = false;
            }
        }

        if(area_0_check)
        {
            area_now = 0;  // 走行状態
        }
        Debug.Log("abcdef___" + area_now.ToString()); 

        areaNow = area_now;

        if(area_now != area_now_past)
        {
            csv_writer.GetComponent<csv_operation_state>().OperationState();
        }

        area_now_past = area_now;   
    }

    /*void ExecuteTwist(TwistMsg msg)
    {
        twist = msg;
    }*/
}
