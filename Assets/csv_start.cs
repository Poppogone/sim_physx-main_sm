using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class csv_start : MonoBehaviour
{
    private StreamWriter sw;
    private StreamWriter sw_op;
    private float timeNow;
    private float timeStart;
    bool IsCheck = false;
    bool IsCheck_op = false;
    [SerializeField] private GameObject op_state;
    private int area_now;
    private int area_pre;


    // Start is called before the first frame update
    void Start()
    {
        timeStart = Time.realtimeSinceStartup;
        operation_state_check script;
        script = op_state.GetComponent<operation_state_check>();
        area_pre = script.area_now;
        //Debug.Log(area_pre+"aaaa");
    }

    // Update is called once per frame
    void Update()
    {   
        if (IsCheck_op)
        {
            timeNow = Time.realtimeSinceStartup - timeStart;
            operation_state_check script;
            script = op_state.GetComponent<operation_state_check>();
            area_now = script.area_now;
            //Debug.Log(area_now+"bbbb");
            if(area_now != area_pre)
            {
                string[] s1_op = {timeNow.ToString(), area_now.ToString()};
                string s2_op = string.Join(",", s1_op);
                sw_op.WriteLine(s2_op);
                area_pre = area_now;
            }
        }
    }
    
    public void StartCheck()
    {
        if (IsCheck)
        {
            timeNow = Time.realtimeSinceStartup - timeStart;
            string[] s1 = {timeNow.ToString()};
            string s2 = string.Join(",", s1);
            sw.WriteLine(s2);
        }
    }
    
    /*private void OnApplicationQuit()
    {
        if (IsCheck)
        {
            sw.Close();
        }
        if (IsCheck_op)
        {
            sw_op.Close();
        }
    }*/

    public void Close()
    {
        if (IsCheck)
        {
            sw.Close();
        }
        if (IsCheck_op)
        {
            sw_op.Close();
        }
    }

    public void MakeFile(Text filename)
    {
        sw = new StreamWriter(@"Assets/ExperimentData_2025/"+filename.text+"_start.csv", false, Encoding.UTF8);
        string[] s1 = {"time"};
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);
        IsCheck = true;

        sw_op = new StreamWriter(@"Assets/ExperimentData_2025/"+filename.text+"_operation_state.csv", false, Encoding.UTF8);
        string[] s1_op = {"time", "area"};
        string s2_op = string.Join(",", s1_op);
        sw_op.WriteLine(s2_op);
        IsCheck_op = true;
    }
}