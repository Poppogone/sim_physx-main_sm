using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class csv_RingQuestion: MonoBehaviour
{
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

    }

    public void LeftAnswerRecord(string area, string question, string c, string answer_state, string ans_time)
    {
        if (IsCheck)
        {
            timeNow = Time.realtimeSinceStartup - timeStart;
            string[] s1 = {timeNow.ToString(), area, "left", question, c, answer_state, ans_time};
            string s2 = string.Join(",", s1);
            sw.WriteLine(s2);
        }
    }
    public void RightAnswerRecord(string area, string question, string c, string answer_state, string ans_time)
    {
        if (IsCheck)
        {
            timeNow = Time.realtimeSinceStartup - timeStart;
            string[] s1 = {timeNow.ToString(), area, "right", question, c, answer_state, ans_time};
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
        sw = new StreamWriter(@"Assets/ExperimentData_2025/"+filename.text+"_RingQuestion.csv", false, Encoding.UTF8);
        string[] s1 = { "Time", "Area", "LorR", "Question", "Character", "State", "Answer_Time"};
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);
        IsCheck = true;
    }

    public void WriteAverageAnsTime(float[,] AnsTime)
    {
        if (!IsCheck) return;

        sw.WriteLine();
        sw.WriteLine("Question,AnswerTime_ave_state0,AnswerTime_ave_state1");

        for (int i = 0; i < 27; i++)
        {
            string line = (i + 1).ToString();
            for (int s = 0; s < 2; s++)
            {
                float ave = AnsTime[i, s];
                line += ",";
                line += (ave > 0f && ave < 5.0f) ? ave.ToString("F4") : "";
            }
            sw.WriteLine(line);
        }

        Debug.Log("[INFO] 平均反応時間をCSVに追記しました（csv_RingQuestion）");
    }    
}