using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class foot_sw : MonoBehaviour
{
    GameObject pop_1;
    bool left_sw = true;
    bool right_sw = true;
    public AudioClip sound;

    // Start is called before the first frame update
    void Start()
    {
        pop_1 = GameObject.Find("CharacterPop_1");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftArrow))   //KeyCode.KeypadMinus
        {
            if(left_sw)
            {
                left_sw = false;
                pop_1.GetComponent<Question_and_Record>().LeftAnswer();
                pop_1.GetComponent<AudioSource>().PlayOneShot(sound);
            }

        }
        else
        {
            left_sw = true;
        }

        if(Input.GetKey(KeyCode.RightArrow))   //KeyCode.KeypadPlus
        {
            if(right_sw)
            {
                right_sw = false;
                pop_1.GetComponent<Question_and_Record>().RightAnswer();
                pop_1.GetComponent<AudioSource>().PlayOneShot(sound);
            }

        }
        else
        {
            right_sw = true;
        }
    }
}