using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;
using UnityEngine.UI;

public class csv_camera : MonoBehaviour
{
    private StreamWriter sw;
    private float timeNow;
    private float timeStart;

    [SerializeField] private GameObject drone;
    string states_camera;

    string R;
    string Phi;

    string Theta;

    bool IsCheck = false;

    // Start is called before the first frame update
    void Start()
    {
        timeStart = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsCheck && drone != null)
        {
            timeNow = Time.realtimeSinceStartup - timeStart;

            if (drone.GetComponent<headFollowing>().enabled)
            {
                headFollowing script;
                script = drone.GetComponent<headFollowing>();
                states_camera = script.IsFree.ToString();
                R = script.r.ToString();
                Phi = script.phi.ToString();
                Theta = script.theta.ToString();
                Debug.Log("camera__1");
            }

            else if (drone.GetComponent<headJoystick>().enabled)
            {
                headJoystick script;
                script = drone.GetComponent<headJoystick>();
                states_camera = script.IsFree.ToString();
                R = script.r.ToString();
                Phi = script.phi.ToString();
                Theta = script.theta.ToString();
                Debug.Log("camera__2");
            }

            else if (drone.GetComponent<JoyStickController>().enabled)
            {
                JoyStickController script;
                script = drone.GetComponent<JoyStickController>();
                states_camera = script.IsFree.ToString();
                R = script.dir.ToString();
                Phi = script.pan.ToString();
                Theta = script.tilt.ToString();
                Debug.Log("camera__3");
            }

            else if (drone.GetComponent<SwipeController>().enabled)
            {
                SwipeController script;
                script = drone.GetComponent<SwipeController>();
                states_camera = script.IsFree.ToString();
                R = script.dir.ToString();
                Phi = script.pan.ToString();
                Theta = script.tilt.ToString();
                Debug.Log("camera__4");
            }


            string[] s1 = {timeNow.ToString(), states_camera, R, Phi, Theta};
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
        sw = new StreamWriter(@"Assets/ExperimentData/"+filename.text+"_camera.csv", false, Encoding.UTF8);
        string[] s1 = { "time","is_free", "r","phi", "theta" };
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);
        IsCheck = true;
    }
}

