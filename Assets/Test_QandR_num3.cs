using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Test_QandR_num3 : MonoBehaviour
{
    GameObject csv_writer;
    // bool start_sw = false;
    bool start_sw = false;
    bool question_sw = true;    // 出題可能な状態かどうか
    bool in_question = false;   // 出題中かどうか

    [SerializeField, PersistentAmongPlayMode] private bool list_0_reset_sw;
    [SerializeField, PersistentAmongPlayMode] private List<int> question_0_list_1 = new List<int>();
    [SerializeField, PersistentAmongPlayMode] private List<int> question_0_list_2 = new List<int>();
    [SerializeField, PersistentAmongPlayMode] private List<string> charactrer_0_list = new List<string>();

    [SerializeField, PersistentAmongPlayMode] private bool list_1_reset_sw;
    [SerializeField, PersistentAmongPlayMode] private List<int> question_1_list_1 = new List<int>();
    [SerializeField, PersistentAmongPlayMode] private List<int> question_1_list_2 = new List<int>();
    [SerializeField, PersistentAmongPlayMode] private List<string> charactrer_1_list = new List<string>();


    List<Vector2> text_pos_list = new List<Vector2>();

    int selected_question;

    Vector2 selected_pos;
    int selected_c_num;

    [SerializeField] private GameObject text;
    [SerializeField] private GameObject N_image;
    [SerializeField] private GameObject H_image;

    private float timeAnswer;
    private float timeQuestion;

    [SerializeField] private GameObject op_state;

    int area_now;
    string c_now;

    int ope_state;

    float[,] all_ans_time = new float[27, 4];  // 回答時間を各エリア，各回答数毎に記録（エリア０の１回目，２回目，エリア１の１回目，２回目，… の順）
    public float[,] AnsTime = new float[27, 2];  // エリアごとの回答時間の平均（to_heatmap.cs へ）

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 27; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                all_ans_time[i, j] = 5.0f;  // 初期値5.0に設定
                //Debug.Log("2a");
            }
        }

        csv_writer = GameObject.Find("CSV writer");

        if(list_0_reset_sw)
        {
            question_0_list_1 = new List<int>();
            question_0_list_2 = new List<int>();
            charactrer_0_list = new List<string>();
            for(int i = 0; i < 27; i++)
            {
                question_0_list_1.Add(i+1);
                question_0_list_2.Add(i+1);
                charactrer_0_list.Add("HN");
            }
            list_0_reset_sw = false;
        }
        
        if(list_1_reset_sw)
        {
            question_1_list_1 = new List<int>();
            question_1_list_2 = new List<int>();
            charactrer_1_list = new List<string>();
            for(int i = 0; i < 27; i++)
            {
                question_1_list_1.Add(i+1);
                question_1_list_2.Add(i+1);
                charactrer_1_list.Add("HN");
            }
            list_1_reset_sw = false;
        }


        // 出題位置設定
        text_pos_list.AddRange(new Vector2[] {new Vector2 (350.0f, 960.0f), new Vector2 (520.0f, 960.0f), new Vector2 (690.0f, 960.0f),
                                              new Vector2 (350.0f, 830.0f), new Vector2 (520.0f, 830.0f), new Vector2 (690.0f, 830.0f),
                                              new Vector2 (350.0f, 700.0f), new Vector2 (520.0f, 700.0f), new Vector2 (690.0f, 700.0f),
                                              new Vector2 (1190.0f, 960.0f), new Vector2 (1360.0f, 960.0f), new Vector2 (1530.0f, 960.0f),
                                              new Vector2 (1190.0f, 830.0f), new Vector2 (1360.0f, 830.0f), new Vector2 (1530.0f, 830.0f),
                                              new Vector2 (1190.0f, 700.0f), new Vector2 (1360.0f, 700.0f), new Vector2 (1530.0f, 700.0f),
                                              new Vector2 (770.0f, 420.0f), new Vector2 (940.0f, 420.0f), new Vector2 (1110.0f, 420.0f),
                                              new Vector2 (770.0f, 290.0f), new Vector2 (940.0f, 290.0f), new Vector2 (1110.0f, 290.0f),
                                              new Vector2 (770.0f, 160.0f), new Vector2 (940.0f, 160.0f), new Vector2 (1110.0f, 160.0f)
                                              });
        // charactrer_list.AddRange(new List<string>[] {new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>()});
    
    StartCheck(); // 出題開始のフラグをオンにする

    }

    // Update is called once per frame
    void Update()
    {
        if(question_0_list_1.Count == 0 && question_0_list_2.Count == 0)
        {
            question_0_list_1 = new List<int>();
            question_0_list_2 = new List<int>();
            charactrer_0_list = new List<string>();
            for(int i = 0; i < 27; i++)
            {
                question_0_list_1.Add(i+1);
                question_0_list_2.Add(i+1);
                charactrer_0_list.Add("HN");
            }
        }
        
        if(question_1_list_1.Count == 0 && question_1_list_2.Count == 0)
        {
            question_1_list_1 = new List<int>();
            question_1_list_2 = new List<int>();
            charactrer_1_list = new List<string>();
            for(int i = 0; i < 27; i++)
            {
                question_1_list_1.Add(i+1);
                question_1_list_2.Add(i+1);
                charactrer_1_list.Add("HN");
            }
        }


        if(start_sw)
        {
            if(question_sw && !in_question) // "出題可能" かつ "出題中ではない"
            {
                question_sw = false;
                Invoke("Question", UnityEngine.Random.Range(1.0f, 2.0f)); // 出題間隔設定(2, 5)
            }
        }

    }

    void Question()
    {
        operation_state_check script;
        script = op_state.GetComponent<operation_state_check>();
        area_now = script.area_now;

        if(area_now == 0 || area_now == 3)  // 走行
        {
            ope_state = 0;
        }
        else if(area_now == 1 || area_now == 2)  // 掘削
        {
            ope_state = 1;
        }


        if(ope_state == 0)
        {
            if(question_0_list_1.Count == 0 && question_0_list_2.Count > 0)
            {
                selected_question = question_0_list_2[UnityEngine.Random.Range(0,question_0_list_2.Count)];
                question_0_list_2.Remove(selected_question);
            }
            else
            {
                selected_question = question_0_list_1[UnityEngine.Random.Range(0,question_0_list_1.Count)];
                question_0_list_1.Remove(selected_question);
            }

            selected_pos = text_pos_list[selected_question-1];
            selected_c_num = UnityEngine.Random.Range(0,charactrer_0_list[selected_question-1].Length);

            if(charactrer_0_list[selected_question-1].Substring(selected_c_num,1) == "N")
            {
                // 右
                N_image.GetComponent<RectTransform>().anchoredPosition = selected_pos;
                N_image.SetActive(true);
                c_now = "N";
            }
            else
            {
                // 左
                H_image.GetComponent<RectTransform>().anchoredPosition = selected_pos;
                H_image.SetActive(true);
                c_now = "H";
            }

            in_question = true;
            timeQuestion = Time.realtimeSinceStartup;
            Invoke("Disappearance", 2);
            charactrer_0_list[selected_question-1] = charactrer_0_list[selected_question-1].Remove(selected_c_num,1);
            Debug.Log("abcde___" + charactrer_0_list[selected_question-1].Length.ToString());
            Debug.Log("abcde出題しました");
        }


        if(ope_state == 1)
        {
            if(question_1_list_1.Count == 0 && question_1_list_2.Count > 0)
            {
                selected_question = question_1_list_2[UnityEngine.Random.Range(0,question_1_list_2.Count)];
                question_1_list_2.Remove(selected_question);
            }
            else
            {
                selected_question = question_1_list_1[UnityEngine.Random.Range(0,question_1_list_1.Count)];
                question_1_list_1.Remove(selected_question);
            }

            selected_pos = text_pos_list[selected_question-1];
            selected_c_num = UnityEngine.Random.Range(0,charactrer_1_list[selected_question-1].Length);

            if(charactrer_1_list[selected_question-1].Substring(selected_c_num,1) == "N")
            {
                // 右
                N_image.GetComponent<RectTransform>().anchoredPosition = selected_pos;
                N_image.SetActive(true);
                c_now = "N";
            }
            else
            {
                // 左
                H_image.GetComponent<RectTransform>().anchoredPosition = selected_pos;
                H_image.SetActive(true);
                c_now = "H";
            }

            in_question = true;
            timeQuestion = Time.realtimeSinceStartup;
            Invoke("Disappearance", 2);
            charactrer_1_list[selected_question-1] = charactrer_1_list[selected_question-1].Remove(selected_c_num,1);
            Debug.Log("abcde___" + charactrer_1_list[selected_question-1].Length.ToString());
            Debug.Log("abcde出題しました");
        }

        


    }

    void Disappearance()
    {
        timeAnswer = Time.realtimeSinceStartup - timeQuestion;
        csv_writer.GetComponent<csv_RingQuestion>().RightAnswerRecord(ope_state.ToString(), selected_question.ToString(), c_now, "ignored", timeAnswer.ToString());
        if(all_ans_time[selected_question - 1, ope_state*2] == 5.0f)
        {
            all_ans_time[selected_question - 1, ope_state*2] = 4.0f;
        }
        else if(all_ans_time[selected_question - 1, ope_state*2] < 5.0f)
        {
            all_ans_time[selected_question - 1, ope_state*2 + 1] = 4.0f;
            AnsTime[selected_question - 1, ope_state] = (all_ans_time[selected_question - 1, ope_state*2] + all_ans_time[selected_question - 1, ope_state*2 + 1]) / 2; 
        }
        H_image.SetActive(false);
        N_image.SetActive(false);
        Debug.Log("abcde消失させました");
        question_sw = true;
        in_question = false;
    }

    public void LeftAnswer()
    {
        if(!in_question)
        {
            // 勝手に押した
            csv_writer.GetComponent<csv_RingQuestion>().LeftAnswerRecord("9999","9999","xx", "irrelevant_answer", "9999");
        }
        else
        {
            if(c_now == "H")
            {
                // 正答
                timeAnswer = Time.realtimeSinceStartup - timeQuestion;
                csv_writer.GetComponent<csv_RingQuestion>().LeftAnswerRecord(ope_state.ToString(), selected_question.ToString(), "H", "correct", timeAnswer.ToString());
                if(all_ans_time[selected_question - 1, ope_state*2] == 5.0f)
                {
                    all_ans_time[selected_question - 1, ope_state*2] = timeAnswer;
                }
                else if(all_ans_time[selected_question - 1, ope_state*2] < 5.0f)
                {
                    all_ans_time[selected_question - 1, ope_state*2 + 1] = timeAnswer;
                    AnsTime[selected_question - 1, ope_state] = (all_ans_time[selected_question - 1, ope_state*2] + all_ans_time[selected_question - 1, ope_state*2 + 1]) / 2; 
                }
                CancelInvoke("Disappearance");
                Debug.Log("abcde__関数の取消");
                H_image.SetActive(false);
                N_image.SetActive(false);
                Debug.Log("abcde消失させました");
                question_sw = true;
                in_question = false;
            }
            else if(c_now == "N")
            {
                // 文字の見間違え
                timeAnswer = Time.realtimeSinceStartup - timeQuestion;
                csv_writer.GetComponent<csv_RingQuestion>().LeftAnswerRecord(ope_state.ToString(), selected_question.ToString(), "N","mistake", timeAnswer.ToString());
                if(all_ans_time[selected_question - 1, ope_state*2] == 5.0f)
                {
                    all_ans_time[selected_question - 1, ope_state*2] = 4.0f;
                }
                else if(all_ans_time[selected_question - 1, ope_state*2] < 5.0f)
                {
                    all_ans_time[selected_question - 1, ope_state*2 + 1] = 4.0f;
                    AnsTime[selected_question - 1, ope_state] = (all_ans_time[selected_question - 1, ope_state*2] + all_ans_time[selected_question - 1, ope_state*2 + 1]) / 2; 
                }
                CancelInvoke("Disappearance");
                Debug.Log("abcde__関数の取消");
                H_image.SetActive(false);
                N_image.SetActive(false);
                Debug.Log("abcde消失させました");
                question_sw = true;
                in_question = false;
            }
        }
    }

    public void RightAnswer()
    {
        if(!in_question)
        {
            // 勝手に押した
            csv_writer.GetComponent<csv_RingQuestion>().RightAnswerRecord("9999", "9999", "xxx", "irrelevant_answer", "9999");
        }
        else
        {
            if(c_now == "N")
            {
                // 正答
                timeAnswer = Time.realtimeSinceStartup - timeQuestion;
                csv_writer.GetComponent<csv_RingQuestion>().RightAnswerRecord(ope_state.ToString(), selected_question.ToString(), "N", "correct", timeAnswer.ToString());
                if(all_ans_time[selected_question - 1, ope_state*2] == 5.0f)
                {
                    all_ans_time[selected_question - 1, ope_state*2] = timeAnswer;
                }
                else if(all_ans_time[selected_question - 1, ope_state*2] < 5.0f)
                {
                    all_ans_time[selected_question - 1, ope_state*2 + 1] = timeAnswer;
                    AnsTime[selected_question - 1, ope_state] = (all_ans_time[selected_question - 1, ope_state*2] + all_ans_time[selected_question - 1, ope_state*2 + 1]) / 2; 
                }
                CancelInvoke("Disappearance");
                Debug.Log("abcde__関数の取消");
                H_image.SetActive(false);
                N_image.SetActive(false);
                Debug.Log("abcde消失させました");
                question_sw = true;
                in_question = false;
            }
            else if(c_now == "H")
            {
                // 文字の見間違え
                timeAnswer = Time.realtimeSinceStartup - timeQuestion;
                csv_writer.GetComponent<csv_RingQuestion>().RightAnswerRecord(ope_state.ToString(), selected_question.ToString(), "H", "mistake", timeAnswer.ToString());
                if(all_ans_time[selected_question - 1, ope_state*2] == 5.0f)
                {
                    all_ans_time[selected_question - 1, ope_state*2] = 4.0f;
                }
                else if(all_ans_time[selected_question - 1, ope_state*2] < 5.0f)
                {
                    all_ans_time[selected_question - 1, ope_state*2 + 1] = 4.0f;
                    AnsTime[selected_question - 1, ope_state] = (all_ans_time[selected_question - 1, ope_state*2] + all_ans_time[selected_question - 1, ope_state*2 + 1]) / 2; 
                }
                CancelInvoke("Disappearance");
                Debug.Log("abcde__関数の取消");
                H_image.SetActive(false);
                N_image.SetActive(false);
                Debug.Log("abcde消失させました");
                question_sw = true;
                in_question = false;
            }
        }
    }

    public void StartCheck()
    {
        start_sw = true;
    }

}
