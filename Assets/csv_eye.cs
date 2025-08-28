using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Tobii.Gaming;

public class csv_eye : MonoBehaviour
{
    [Header("Logging Control")]
    public bool enabledLogging = false;

    private StreamWriter sw;
    private float timeNow;
    private float timeStart;
    bool IsCheck = false;

    // Start is called before the first frame update
    void Start()
    {
        timeStart = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        if (!enabledLogging) return;
        GazePoint gazePoint = TobiiAPI.GetGazePoint();
        timeNow = Time.realtimeSinceStartup - timeStart;

        if(IsCheck)
        {
        /*if(gazePoint.IsRecent())
        {*/
            //Vector2 gazePos = gazePoint.Viewport;    //(0~1)
            Vector2 gazePos = gazePoint.Screen;    //(like mouse pointer)
            string[] s1 = {timeNow.ToString(), "1", gazePos.x.ToString(), gazePos.y.ToString()};
            string s2 = string.Join(",", s1);
            sw.WriteLine(s2);
            Debug.Log(gazePos);
        /*}
        else
        {
            string[] s1 = {timeNow.ToString(), "0", "NN", "NN"};
            string s2 = string.Join(",", s1);
            sw.WriteLine(s2);
        }*/
        }
    }
    
    private void OnApplicationQuit()
    {
        if(IsCheck)
        {
            sw.Close();
        }
    }

    public void MakeFile(Text filename)
    {
        if (!enabledLogging) return;
        sw = new StreamWriter(@"Assets/ExperimentData_2025/"+filename.text+"_eye.csv", false, Encoding.UTF8);
        string[] s1 = { "time", "is_recent","pos_x", "pos_y" };
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);
        IsCheck = true;
    }
}
