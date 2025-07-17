using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class csv_target_pos_1 : MonoBehaviour
{
    private StreamWriter sw1;
    private StreamWriter sw3;
    private StreamWriter sw5;
    private StreamWriter sw7;
    private float timeNow;
    private float timeStart;
    [SerializeField] private Transform target1;
    [SerializeField] private Transform target3;
    [SerializeField] private Transform target5;
    [SerializeField] private Transform target7;
    private Vector3 pos_pre1;
    private Vector3 pos_pre3;
    private Vector3 pos_pre5;
    private Vector3 pos_pre7;
    bool IsCheck = false;

    // Start is called before the first frame update
    void Start()
    {
        timeStart = Time.realtimeSinceStartup;
        pos_pre1 = target1.position;
        pos_pre3 = target3.position;
        pos_pre5 = target5.position;
        pos_pre7 = target7.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsCheck)
        {
            timeNow = Time.realtimeSinceStartup - timeStart;
            if(target1.position != pos_pre1)
            {
                string[] s1_1 = {timeNow.ToString(), target1.position.x.ToString(), target1.position.y.ToString(), target1.position.z.ToString()};
                string s2_1 = string.Join(",", s1_1);
                sw1.WriteLine(s2_1);
                pos_pre1 = target1.position;
            }

            if(target3.position != pos_pre3)
            {
                string[] s1_3 = {timeNow.ToString(), target3.position.x.ToString(), target3.position.y.ToString(), target3.position.z.ToString()};
                string s2_3 = string.Join(",", s1_3);
                sw3.WriteLine(s2_3);
                pos_pre3 = target3.position;
            }

            if(target5.position != pos_pre5)
            {
                string[] s1_5 = {timeNow.ToString(), target5.position.x.ToString(), target5.position.y.ToString(), target5.position.z.ToString()};
                string s2_5 = string.Join(",", s1_5);
                sw5.WriteLine(s2_5);
                pos_pre5 = target5.position;
            }

            if(target7.position != pos_pre7)
            {
                string[] s1_7 = {timeNow.ToString(), target7.position.x.ToString(), target7.position.y.ToString(), target7.position.z.ToString()};
                string s2_7 = string.Join(",", s1_7);
                sw7.WriteLine(s2_7);
                pos_pre7 = target7.position;
            }
        }

    }
    
    /*private void OnApplicationQuit()
    {
        if (IsCheck)
        {
            sw1.Close();
            sw3.Close();
            sw5.Close();
            sw7.Close();
        }
    }*/

    public void Close()
    {
        if (IsCheck)
        {
            sw1.Close();
            sw3.Close();
            sw5.Close();
            sw7.Close();
        } 
    }


    public void MakeFile(Text filename)
    {
        sw1 = new StreamWriter(@"Assets/ExperimentData/"+filename.text+"_target_pos_1.csv", false, Encoding.UTF8);
        string[] s1_1 = { "time", "pos_x", "pos_y", "pos_z"};
        string s2_1 = string.Join(",", s1_1);
        sw1.WriteLine(s2_1);
        Debug.Log("csv_target_pos_1");

        sw3 = new StreamWriter(@"Assets/ExperimentData/"+filename.text+"_target_pos_3.csv", false, Encoding.UTF8);
        string[] s1_3 = { "time", "pos_x", "pos_y", "pos_z"};
        string s2_3 = string.Join(",", s1_3);
        sw3.WriteLine(s2_3);
        Debug.Log("csv_target_pos_3");

        sw5 = new StreamWriter(@"Assets/ExperimentData/"+filename.text+"_target_pos_5.csv", false, Encoding.UTF8);
        string[] s1_5 = { "time", "pos_x", "pos_y", "pos_z"};
        string s2_5 = string.Join(",", s1_5);
        sw5.WriteLine(s2_5);
        Debug.Log("csv_target_pos_5");

        sw7 = new StreamWriter(@"Assets/ExperimentData/"+filename.text+"_target_pos_7.csv", false, Encoding.UTF8);
        string[] s1_7 = { "time", "pos_x", "pos_y", "pos_z"};
        string s2_7 = string.Join(",", s1_7);
        sw7.WriteLine(s2_7);
        Debug.Log("csv_target_pos_7");

        IsCheck = true;
    }
}