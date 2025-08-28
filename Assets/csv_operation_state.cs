using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;
using UnityEngine.UI;

public class csv_operation_state : MonoBehaviour
{
    [Header("Logging Control")]
    public bool enabledLogging = false;

    private StreamWriter sw;
    private float timeNow;
    private float timeStart;
    bool IsCheck = false;
    bool check = false;

    // Start is called before the first frame update
    void Start()
    {
        timeStart = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        if (!enabledLogging) return;
        if(IsCheck)
        {
            if(check)
            {
                string[] s1 = {timeNow.ToString(), operation_state_check.areaNow.ToString()};
                string s2 = string.Join(",", s1);
                sw.WriteLine(s2);
                check = false;
            }
        }
    }

    public void OperationState()
    {
        if (!enabledLogging) return;
        check = true;
    }

    public void MakeFile(Text filename)
    {
        if (!enabledLogging) return;
        sw = new StreamWriter(@"Assets/ExperimentData_2025/"+filename.text+"_operation_state.csv", false, Encoding.UTF8);
        string[] s1 = { "time","operation_state"};
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);
        IsCheck = true;
    }
}
