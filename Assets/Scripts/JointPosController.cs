using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class JointPosController : MonoBehaviour
{
    private ROSConnection ros;
    public string setpointTopicName = "joint_name/setpoint";
    public double initTargetPos;
    private ArticulationBody joint;
    private Float64Msg targetPos;

    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        joint = this.GetComponent<ArticulationBody>();
        targetPos = new Float64Msg();

        if (joint)
        {
            var drive = joint.xDrive;

            //初期位置を固定しないならコメントアウト↓
            targetPos.data = initTargetPos;
            drive.target = (float)(targetPos.data * Mathf.Rad2Deg);
            joint.xDrive = drive;
            //↑ここまでコメントアウト

            //xDriveを個別に設定しているため省略
        }
        else
        {
            //Debug.Log("No ArticulationBody are found");
        }
        ros.Subscribe<Float64Msg>(setpointTopicName, ExecuteJointPosControl);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Joint Target Position:" + targetPos.data);
        var drive = joint.xDrive;
        drive.target = (float)(targetPos.data * Mathf.Rad2Deg);
        joint.xDrive = drive;
    }

    void ExecuteJointPosControl(Float64Msg msg)
    {
        targetPos = msg;
        //Debug.Log("Joint Target Position:" + targetPos.data);
    }
}
