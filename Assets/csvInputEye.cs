using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Tobii.Gaming;

public class csvInputEye : MonoBehaviour
{
    private StreamWriter sw;
    private float timeNow;
    private float timeStart;

    Vector2 filteredPos;
    bool IsCheck = false;
    bool IsFileMade = false;
    
    // Start is called before the first frame update
    void Start()
    {
        timeStart = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        GazePoint gazePoint = TobiiAPI.GetGazePoint();

        timeNow = Time.realtimeSinceStartup - timeStart;

        /*if(!IsFileMade)
        {
            MakeFile(FilenameLoad.filename);
            IsFileMade = true;
        }*/

        if(IsCheck)
        {
            /*if(gazePoint.IsRecent())
            {*/
                Vector2 gazePos = gazePoint.Screen;
                filteredPos = Vector2.Lerp(filteredPos, gazePos, 0.5f);
                string[] s1 = {timeNow.ToString(), "1", gazePos.x.ToString(), gazePos.y.ToString(), filteredPos.x.ToString(), filteredPos.y.ToString()};
                string s2 = string.Join(",", s1);
                sw.WriteLine(s2);
            /*}
            else
            {
                string[] s1 = {timeNow.ToString(), "0", "NN", "NN", "NN", "NN"};
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

    public void MakeFile(string filename)
    {
        sw = new StreamWriter(@"Assets/ExperimentData/"+filename+"_inputeye.csv", false, Encoding.UTF8);
        string[] s1 = { "time", "is_recent", "pos_x", "pos_y", "filtered_pos_x", "filtered_pos_y"};
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);
        IsCheck = true;
    }
}
