using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class target_transform : MonoBehaviour
{
    private Vector3 pos_pre_1;
    private Vector3 pos_pre_2;
    private Vector3 pos_pre_3;
    private Vector3 pos_pre_4;
    private Vector3 pos_pre_5;
    private Vector3 pos_pre_6;
    private Vector3 pos_pre_7;
    private Vector3 pos_pre_8;

    private Vector3 rot_pre_1;
    private Vector3 rot_pre_2;
    private Vector3 rot_pre_3;
    private Vector3 rot_pre_4;
    private Vector3 rot_pre_5;
    private Vector3 rot_pre_6;
    private Vector3 rot_pre_7;
    private Vector3 rot_pre_8;

    private bool transform_sw_1;
    private bool transform_sw_2;

    [SerializeField] private Transform target1;
    [SerializeField] private Transform target2;
    [SerializeField] private Transform target3;
    [SerializeField] private Transform target4;
    [SerializeField] private Transform target5;
    [SerializeField] private Transform target6;
    [SerializeField] private Transform target7;
    [SerializeField] private Transform target8;

    [SerializeField] private GameObject op_state;

    int area_now;
    
    // Start is called before the first frame update
    void Start()
    {
        //targetの初期位置
        pos_pre_1 = target1.position;
        pos_pre_2 = target2.position;
        pos_pre_3 = target3.position;
        pos_pre_4 = target4.position;
        pos_pre_5 = target5.position;
        pos_pre_6 = target6.position;
        pos_pre_7 = target7.position;
        pos_pre_8 = target8.position;

        rot_pre_1 = target1.eulerAngles;
        rot_pre_2 = target2.eulerAngles;
        rot_pre_3 = target3.eulerAngles;
        rot_pre_4 = target4.eulerAngles;
        rot_pre_5 = target5.eulerAngles;
        rot_pre_6 = target6.eulerAngles;
        rot_pre_7 = target7.eulerAngles;
        rot_pre_8 = target8.eulerAngles;

        transform_sw_1 = false;
        transform_sw_2 = false;
    }

    // Update is called once per frame
    void Update()
    {
        operation_state_check script;
        script = op_state.GetComponent<operation_state_check>();
        area_now = script.area_now;
        

        //area7後にarea9に入ると，target1～4を初期化
        if(area_now == 7)
        {
            transform_sw_1 = true;
        }

        if(transform_sw_1)
        {
            if(area_now == 9)
            {
                target1.transform.position = pos_pre_1;
                target1.transform.eulerAngles = rot_pre_1;
                target2.transform.position = pos_pre_2;
                target2.transform.eulerAngles = rot_pre_2;
                target3.transform.position = pos_pre_3;
                target3.transform.eulerAngles = rot_pre_3;
                target4.transform.position = pos_pre_4;
                target4.transform.eulerAngles = rot_pre_4;
             
                transform_sw_1 = false;
            }
        }

        //area1後にarea3に入ると，target5～8を初期化
        if(area_now == 1)
        {
            transform_sw_1 = true;
        }

        if(transform_sw_1)
        {
            if(area_now == 3)
            {
                target5.transform.position = pos_pre_5;
                target5.transform.eulerAngles = rot_pre_5;
                target6.transform.position = pos_pre_6;
                target6.transform.eulerAngles = rot_pre_6;
                target7.transform.position = pos_pre_7;
                target7.transform.eulerAngles = rot_pre_7;
                target8.transform.position = pos_pre_8;
                target8.transform.eulerAngles = rot_pre_8;
             
                transform_sw_1 = false;
            }
        }

    }
}
