using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class csv_target_pos_2 : MonoBehaviour
{
    private StreamWriter sw2;
    private StreamWriter sw4;
    private StreamWriter sw6;
    private StreamWriter sw8;
    private float timeNow;
    private float timeStart;
    [SerializeField] private Transform target2;
    [SerializeField] private Transform target4;
    [SerializeField] private Transform target6;
    [SerializeField] private Transform target8;
    private Vector3 pos_pre2;
    private Vector3 pos_pre4;
    private Vector3 pos_pre6;
    private Vector3 pos_pre8;
    bool IsCheck = false;

    // Start is called before the first frame update
    void Start()
    {
        timeStart = Time.realtimeSinceStartup;
        pos_pre2 = target2.position;
        pos_pre4 = target4.position;
        pos_pre6 = target6.position;
        pos_pre8 = target8.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsCheck)
        {
            timeNow = Time.realtimeSinceStartup - timeStart;
            if(target2.position != pos_pre2)
            {
                string[] s1_2 = {timeNow.ToString(), target2.position.x.ToString(), target2.position.y.ToString(), target2.position.z.ToString()};
                string s2_2 = string.Join(",", s1_2);
                sw2.WriteLine(s2_2);
                pos_pre2 = target2.position;
            }

            if(target4.position != pos_pre4)
            {
                string[] s1_4 = {timeNow.ToString(), target4.position.x.ToString(), target4.position.y.ToString(), target4.position.z.ToString()};
                string s2_4 = string.Join(",", s1_4);
                sw4.WriteLine(s2_4);
                pos_pre4 = target4.position;
            }

            if(target6.position != pos_pre6)
            {
                string[] s1_6 = {timeNow.ToString(), target6.position.x.ToString(), target6.position.y.ToString(), target6.position.z.ToString()};
                string s2_6 = string.Join(",", s1_6);
                sw6.WriteLine(s2_6);
                pos_pre6 = target6.position;
            }

            if(target8.position != pos_pre8)
            {
                string[] s1_8 = {timeNow.ToString(), target8.position.x.ToString(), target8.position.y.ToString(), target8.position.z.ToString()};
                string s2_8 = string.Join(",", s1_8);
                sw8.WriteLine(s2_8);
                pos_pre8 = target8.position;
            }
        }

    }
    
    /*private void OnApplicationQuit()
    {
        if (IsCheck)
        {
            sw2.Close();
            sw4.Close();
            sw6.Close();
            sw8.Close();
        }
    }*/

    public void Close()
    {
        if (IsCheck)
        {
            sw2.Close();
            sw4.Close();
            sw6.Close();
            sw8.Close();
        } 
    }


    public void MakeFile(Text filename)
    {
        sw2 = new StreamWriter(@"Assets/ExperimentData_2025/"+filename.text+"_target_pos_2.csv", false, Encoding.UTF8);
        string[] s1_2 = { "time", "pos_x", "pos_y", "pos_z"};
        string s2_2 = string.Join(",", s1_2);
        sw2.WriteLine(s2_2);
        Debug.Log("csv_target_pos_2");

        sw4 = new StreamWriter(@"Assets/ExperimentData_2025/"+filename.text+"_target_pos_4.csv", false, Encoding.UTF8);
        string[] s1_4 = { "time", "pos_x", "pos_y", "pos_z"};
        string s2_4 = string.Join(",", s1_4);
        sw4.WriteLine(s2_4);
        Debug.Log("csv_target_pos_4");

        sw6 = new StreamWriter(@"Assets/ExperimentData_2025/"+filename.text+"_target_pos_6.csv", false, Encoding.UTF8);
        string[] s1_6 = { "time", "pos_x", "pos_y", "pos_z"};
        string s2_6 = string.Join(",", s1_6);
        sw6.WriteLine(s2_6);
        Debug.Log("csv_target_pos_6");

        sw8 = new StreamWriter(@"Assets/ExperimentData_2025/"+filename.text+"_target_pos_8.csv", false, Encoding.UTF8);
        string[] s1_8 = { "time", "pos_x", "pos_y", "pos_z"};
        string s2_8 = string.Join(",", s1_8);
        sw8.WriteLine(s2_8);
        Debug.Log("csv_target_pos_8");
        
        IsCheck = true;
    }
}