using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class GetRightJsHor : MonoBehaviour
{
    private ROSConnection ros;
    public string setpointTopicName = "right_js_hor";
    public double initTargetPos;
    private Float64Msg targetPos;
    private double hor_key = 0;
    public GameObject cam;

    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        targetPos = new Float64Msg();

        targetPos.data = initTargetPos;
        ros.Subscribe<Float64Msg>(setpointTopicName, ExecuteJointPosControl);
    }

    // Update is called once per frame
    void Update()
    {
        hor_key = targetPos.data;

        if(hor_key == 1) this.cam.GetComponent<JoyStickController>().Move_left();
        if (hor_key == 0) this.cam.GetComponent<JoyStickController>().Move_horstop();
        if (hor_key == -1) this.cam.GetComponent<JoyStickController>().Move_right();

    }

    void ExecuteJointPosControl(Float64Msg msg)
    {
        targetPos = msg;
        //Debug.Log("right_js_hor: " + targetPos.data);
    }
}
