using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;
using UnityEngine.UI;

public class csv_input_eye : MonoBehaviour
{
    [Header("Logging Control")]
    public bool enabledLogging = false;

    private StreamWriter sw;
    private float timeNow;
    private float timeStart;

    [SerializeField] private Transform base_link;

    private float kenki_x;
    private float kenki_y;
    private float kenki_z;
    private float kenki_ry;

    [SerializeField] private ArticulationBody swing_joint;
    [SerializeField] private ArticulationBody boom_joint;
    [SerializeField] private ArticulationBody arm_joint;
    [SerializeField] private ArticulationBody bucket_joint;

    private float swing_deg;
    private float boom_deg;
    private float arm_deg;
    private float bucket_deg;

    [SerializeField] private GameObject drone;
    string states_camera;
    Vector3 headP;
    Vector3 headA;
    Vector2 filteredPos;
    bool IsCheck = false;

    [SerializeField] private RectTransform e;

    // Start is called before the first frame update
    void Start()
    {
        timeStart = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        if (!enabledLogging) return;
        if (IsCheck)
        {
            GazePoint gazePoint = TobiiAPI.GetGazePoint();
            Debug.Log("eeeeeeee"+gazePoint);

            timeNow = Time.realtimeSinceStartup - timeStart;

            kenki_x = base_link.position.x;
            kenki_y = base_link.position.y;
            kenki_z = base_link.position.z;
            kenki_ry = base_link.rotation.y;

            swing_deg = swing_joint.jointPosition[0];
            boom_deg = boom_joint.jointPosition[0];
            arm_deg = arm_joint.jointPosition[0];
            bucket_deg = bucket_joint.jointPosition[0];

            if (drone.GetComponent<headFollowing>().enabled)
            {
                headFollowing script;
                script = drone.GetComponent<headFollowing>();
                states_camera = script.IsFree.ToString();
                headP = script.headPos;
                headA = script.headAng;
            }

            else if (drone.GetComponent<headJoystick>().enabled)
            {
                headJoystick script;
                script = drone.GetComponent<headJoystick>();
                states_camera = script.IsFree.ToString();
                headP = script.headPos;
                headA = script.headAng;
            }

            else if (drone.GetComponent<JoyStickController>().enabled)
            {
                JoyStickController script;
                script = drone.GetComponent<JoyStickController>();
                states_camera = script.IsFree.ToString();
                headP = script.headPos;
                headA = script.headAng;
            }


            if(gazePoint.IsRecent())
            {
                //Vector2 gazePos = gazePoint.Viewport;    //(0~1)
                Vector2 gazePos = gazePoint.Screen;    //(like mouse pointer)
                filteredPos = Vector2.Lerp(filteredPos, gazePos, 0.5f);
                e.position = new Vector3(filteredPos.x, filteredPos.y, 0);
                Debug.Log("AAAAA"+e.position);
                string[] s1 = {timeNow.ToString(),kenki_x.ToString(),kenki_y.ToString(),kenki_z.ToString(),kenki_ry.ToString(),swing_deg.ToString(),boom_deg.ToString(),arm_deg.ToString(),bucket_deg.ToString(),headP.x.ToString(),headP.y.ToString(),headP.z.ToString(),headA.x.ToString(),headA.y.ToString(),headA.z.ToString(), states_camera, "1", gazePos.x.ToString(), gazePos.y.ToString(), filteredPos.x.ToString(), filteredPos.y.ToString()};
                string s2 = string.Join(",", s1);
                sw.WriteLine(s2);
            }
            else
            {
                string[] s1 = {timeNow.ToString(),kenki_x.ToString(),kenki_y.ToString(),kenki_z.ToString(),kenki_ry.ToString(),swing_deg.ToString(),boom_deg.ToString(),arm_deg.ToString(),bucket_deg.ToString(),headP.x.ToString(),headP.y.ToString(),headP.z.ToString(),headA.x.ToString(),headA.y.ToString(),headA.z.ToString(), states_camera,"0", "NN", "NN", "NN", "NN"};
                string s2 = string.Join(",", s1);
                sw.WriteLine(s2);
            }
        }
    }
    
    private void OnApplicationQuit()
    {
        if (IsCheck)
        {
            sw.Close();
        }
    }

    public void MakeFile(Text filename)
    {
        if (!enabledLogging) return;
        sw = new StreamWriter(@"Assets/ExperimentData_2025/"+filename.text+"_inputeye.csv", false, Encoding.UTF8);
        string[] s1 = { "time","kenki_pos_x","kenki_pos_y","kenki_pos_z","kenki_ori_y","swing","boom","arm","bucket","h_pos_x","h_pos_y","h_pos_z","h_ang_x","h_ang_y","h_ang_z","is_free", "is_recent","pos_x", "pos_y","filtered_pos_x", "filtered_pos_y" };
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);
        IsCheck = true;
    }
}

