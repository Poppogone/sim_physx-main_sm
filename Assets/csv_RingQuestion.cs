using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class csv_RingQuestion: MonoBehaviour
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

    }

    public void LeftAnswerRecord(string area, string question, string c, string answer_state, string ans_time)
    {
        if (!enabledLogging) return;
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
        if (!enabledLogging) return;
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
        if (!enabledLogging) return;
        if (IsCheck) return; // 二重オープン防止（保険）

        var dir = @"Assets/ExperimentData_2025";
        System.IO.Directory.CreateDirectory(dir);

        var path = System.IO.Path.Combine(dir, filename.text + "_RingQuestion.csv");

        // ★ポイント：FileStream + FileShare.Read で開く（Unityの読み取りと共存）
        var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
        sw = new StreamWriter(fs, Encoding.UTF8) { AutoFlush = true };

        string[] header = { "Time", "Area", "LorR", "Question", "Character", "State", "Answer_Time" };
        sw.WriteLine(string.Join(",", header));
        IsCheck = true;

        //Debug.Log("[csv_RingQuestion] MakeFile started: " + path);
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

    // --- SAGAT一時停止・復帰用ダミー ---
    public void PauseLogging() { /* いまは何もしない */ }
    public void ResumeLogging() { /* いまは何もしない */ }

}