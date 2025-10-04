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
        if (check && sw)
        {
            var st = csv_writer.GetComponent<csv_start>();
            if (st != null) st.StartCheck();

            var ring = pop_1.GetComponent<Ring_AnswerRecord>();
            if (ring != null) ring.StartCheck();

            var ringQA = csv_writer.GetComponent<csv_RingQuestion>();
            if (ringQA != null && ringQA.enabledLogging && filenameText != null)
            {
                ringQA.MakeFile(filenameText);   // RingQuestion CSV開始
            }

            var ctrl = csv_writer.GetComponent<csv_ControllerLog>();
            if (ctrl != null && ctrl.enabledLogging && filenameText != null)
            {
                ctrl.MakeFile(filenameText.text);   // ControllerLog CSV開始（string版推奨）
            }

            var pos = csv_writer.GetComponent<csv_zx120_position>();
            if (pos != null && pos.enabledLogging && filenameText != null)
            {
                pos.MakeFile(filenameText.text);
            }
            
            var op = csv_writer.GetComponent<csv_operation_state>();
            if (op != null && op.enabledLogging && filenameText != null)
            {
                op.MakeFile(filenameText.text);
            }

            sw = false; // 二重実行防止
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
