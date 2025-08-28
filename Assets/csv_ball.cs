using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class csv_ball : MonoBehaviour
{
    [Header("Logging Control")]
    public bool enabledLogging = false;   // ← Inspector から ON/OFF 可能

    private StreamWriter sw;
    private float timeNow;
    private float timeStart;

    int pickball;
    int getball;
    int getball2;
    bool check;
    bool IsCheck = false;

    // Start is called before the first frame update
    void Start()
    {
        timeStart = Time.realtimeSinceStartup;
        pickball = 0;
        getball = 0;
        getball2 = 0;
        check = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!enabledLogging) return;
        if (IsCheck)
        {
            timeNow = Time.realtimeSinceStartup - timeStart;
            if(check)
            {
                string[] s1 = {timeNow.ToString(), pickball.ToString(), getball.ToString(), getball2.ToString()};
                string s2 = string.Join(",", s1);
                sw.WriteLine(s2);
                check = false;
            }
        }
    }
    
    public void CountPickBall()
    {
        if (!enabledLogging) return;
        pickball++;
        check = true;
    }
    public void CountGetBall()
    {
        if (!enabledLogging) return;
        getball++;
        check = true;
    }
    public void CountGetBall2()
    {
        if (!enabledLogging) return;
        getball2++;
        check = true;
    }
    
    /*private void OnApplicationQuit()
    {
        if (IsCheck)
        {
            sw.Close();
        }
    }*/

    public void Close()
    {
        if (IsCheck)
        {
            sw.Close();
        } 
    }

    public void MakeFile(Text filename)
    {
        if (!enabledLogging) return;    // チェックが入っていない場合はファイルが作られない
        sw = new StreamWriter(@"Assets/ExperimentData_2025/"+filename.text+"_ball_count.csv", false, Encoding.UTF8);
        string[] s1 = { "time", "pick", "get1", "get2"};
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);
        IsCheck = true;
    }
}