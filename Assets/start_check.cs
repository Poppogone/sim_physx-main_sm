using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class start_check : MonoBehaviour
{
    GameObject csv_writer;
    GameObject pop_1;
    bool check;
    bool sw;
    // Start is called before the first frame update

    [Header("Filename (UI Text)")]
    public Text filenameText; 

    void Start()
    {
        csv_writer = GameObject.Find("CSV writer");
        pop_1 = GameObject.Find("CharacterPop_1");
        check = false;
        sw = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(check && sw)
        {
            csv_writer.GetComponent<csv_start>().StartCheck();
            pop_1.GetComponent<Ring_AnswerRecord>().StartCheck();

            /*
            // ControllerLogのログ開始（enabledLogging は true にしておく）
            var ctrl = csv_writer.GetComponent<csv_ControllerLog>();
            if (ctrl != null && ctrl.enabledLogging && filenameText != null)
            {
                ctrl.MakeFile(filenameText);
                Debug.Log("[start_check] csv_ControllerLog.MakeFile called");
            }
            */
            sw = false;
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("aaaa" + collision.gameObject.tag);
        if(collision.gameObject.tag != "Obstacle" && collision.gameObject.tag != "Ball" && collision.gameObject.tag != "Stone")
        {
            check = true;
        }
    }
}
