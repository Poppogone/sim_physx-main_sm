using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ring_AnswerRecord : MonoBehaviour
{
    GameObject csv_writer;

    bool start_sw = false;
    bool question_sw = true;
    bool in_question = false;
    bool is_csv_written = false;

    [SerializeField, PersistentAmongPlayMode] private List<int> question_0_list_1 = new List<int>();
    [SerializeField, PersistentAmongPlayMode] private List<int> question_0_list_2 = new List<int>();
    [SerializeField, PersistentAmongPlayMode] private List<string> charactrer_0_list = new List<string>();

    [SerializeField, PersistentAmongPlayMode] private List<int> question_1_list_1 = new List<int>();
    [SerializeField, PersistentAmongPlayMode] private List<int> question_1_list_2 = new List<int>();
    [SerializeField, PersistentAmongPlayMode] private List<string> charactrer_1_list = new List<string>();

    List<Vector2> text_pos_list = new List<Vector2>();

    int selected_question;
    Vector2 selected_pos;
    int selected_c_num;

    [SerializeField] private GameObject N_image;
    [SerializeField] private GameObject H_image;
    [SerializeField] private GameObject op_state;

    private float timeAnswer;
    private float timeQuestion;

    int area_now;
    string c_now;
    int ope_state;

    float[,] all_ans_time = new float[27, 4];
    public float[,] AnsTime = new float[27, 2];

    void Start()
    {
        for (int i = 0; i < 27; i++)
            for (int j = 0; j < 4; j++)
                all_ans_time[i, j] = 5.0f;

        csv_writer = GameObject.Find("CSV writer");

        question_0_list_1.Clear();
        question_0_list_2.Clear();
        charactrer_0_list.Clear();
        for (int i = 0; i < 27; i++)
        {
            question_0_list_1.Add(i + 1);
            question_0_list_2.Add(i + 1);
            charactrer_0_list.Add("HN");
        }

        question_1_list_1.Clear();
        question_1_list_2.Clear();
        charactrer_1_list.Clear();
        for (int i = 0; i < 27; i++)
        {
            question_1_list_1.Add(i + 1);
            question_1_list_2.Add(i + 1);
            charactrer_1_list.Add("HN");
        }

        text_pos_list.AddRange(new Vector2[] {
            new Vector2(350,960), new Vector2(520,960), new Vector2(690,960),
            new Vector2(350,830), new Vector2(520,830), new Vector2(690,830),
            new Vector2(350,700), new Vector2(520,700), new Vector2(690,700),
            new Vector2(1190,960), new Vector2(1360,960), new Vector2(1530,960),
            new Vector2(1190,830), new Vector2(1360,830), new Vector2(1530,830),
            new Vector2(1190,700), new Vector2(1360,700), new Vector2(1530,700),
            new Vector2(770,420), new Vector2(940,420), new Vector2(1110,420),
            new Vector2(770,290), new Vector2(940,290), new Vector2(1110,290),
            new Vector2(770,160), new Vector2(940,160), new Vector2(1110,160)
        });

        //StartCheck();
    }

    void Update()
    {
        // ★最小追加：SAGAT中は完全停止（ToSAGATScene→TestDataManagerが制御）
        if (TestDataManager.I != null && TestDataManager.I.pausedForSAGAT) return;

        if (start_sw && question_sw && !in_question)
        {
            question_sw = false;
            Question();
        }
    }

    ////////////////////////////////////////
    /// 
    
    
    void Question()
    {
        var script = op_state.GetComponent<operation_state_check>();
        area_now = script.area_now;

        if (area_now == 0 || area_now == 3)
            ope_state = 0;
        else if (area_now == 1 || area_now == 2)
            ope_state = 1;
        else
            return;

        // 全ての出題が終了したら平均時間を書き出し
        if (!is_csv_written &&
            question_0_list_1.Count == 0 && question_0_list_2.Count == 0 &&
            question_1_list_1.Count == 0 && question_1_list_2.Count == 0)
        {
            Debug.Log("[INFO] 全問題出題済み。平均反応時間をCSVに出力します。");
            csv_writer.GetComponent<csv_RingQuestion>().WriteAverageAnsTime(AnsTime);
            is_csv_written = true;
            return;
        }

        // ope_state 0 の出題処理
        if (ope_state == 0)
        {
            if (question_0_list_1.Count == 0 && question_0_list_2.Count == 0)
            {
                // 今のフェーズにはもう問題がない
                question_sw = true;
                return;
            }

            if (question_0_list_1.Count == 0 && question_0_list_2.Count > 0)
            {
                selected_question = question_0_list_2[UnityEngine.Random.Range(0, question_0_list_2.Count)];
                question_0_list_2.Remove(selected_question);
            }
            else
            {
                selected_question = question_0_list_1[UnityEngine.Random.Range(0, question_0_list_1.Count)];
                question_0_list_1.Remove(selected_question);
            }

            selected_pos = text_pos_list[selected_question - 1];
            selected_c_num = UnityEngine.Random.Range(0, charactrer_0_list[selected_question - 1].Length);
            c_now = charactrer_0_list[selected_question - 1].Substring(selected_c_num, 1);

            ShowImage(c_now, selected_pos);
            charactrer_0_list[selected_question - 1] = charactrer_0_list[selected_question - 1].Remove(selected_c_num, 1);

            in_question = true;
            timeQuestion = Time.realtimeSinceStartup;
            Invoke("Disappearance", 2);
            return;
        }

        // ope_state 1 の出題処理
        if (ope_state == 1)
        {
            if (question_1_list_1.Count == 0 && question_1_list_2.Count == 0)
            {
                question_sw = true;
                return;
            }

            if (question_1_list_1.Count == 0 && question_1_list_2.Count > 0)
            {
                selected_question = question_1_list_2[UnityEngine.Random.Range(0, question_1_list_2.Count)];
                question_1_list_2.Remove(selected_question);
            }
            else
            {
                selected_question = question_1_list_1[UnityEngine.Random.Range(0, question_1_list_1.Count)];
                question_1_list_1.Remove(selected_question);
            }

            selected_pos = text_pos_list[selected_question - 1];
            selected_c_num = UnityEngine.Random.Range(0, charactrer_1_list[selected_question - 1].Length);
            c_now = charactrer_1_list[selected_question - 1].Substring(selected_c_num, 1);

            ShowImage(c_now, selected_pos);
            charactrer_1_list[selected_question - 1] = charactrer_1_list[selected_question - 1].Remove(selected_c_num, 1);

            in_question = true;
            timeQuestion = Time.realtimeSinceStartup;
            Invoke("Disappearance", 2);
            return;
        }
    }
    
    
    ////////////////////////////////////////
    /*
    void Question()
    {
        ope_state = 1; // ← 固定

        if (!is_csv_written && question_1_list_1.Count == 0 && question_1_list_2.Count == 0)
        {
            FinishAndWriteAverage();                 // 平均表を追記
            csv_writer.GetComponent<csv_RingQuestion>()?.Close(); // 任意：確実にファイルを閉じる
            is_csv_written = true;
            return;
        }

        if (question_1_list_1.Count == 0 && question_1_list_2.Count > 0)
        {
            selected_question = question_1_list_2[UnityEngine.Random.Range(0, question_1_list_2.Count)];
            question_1_list_2.Remove(selected_question);
        }
        else
        {
            selected_question = question_1_list_1[UnityEngine.Random.Range(0, question_1_list_1.Count)];
            question_1_list_1.Remove(selected_question);
        }

        selected_pos = text_pos_list[selected_question - 1];
        selected_c_num = UnityEngine.Random.Range(0, charactrer_1_list[selected_question - 1].Length);

        if (charactrer_1_list[selected_question - 1].Substring(selected_c_num, 1) == "N")
        {
            N_image.GetComponent<RectTransform>().anchoredPosition = selected_pos;
            N_image.SetActive(true);
            c_now = "N";
        }
        else
        {
            H_image.GetComponent<RectTransform>().anchoredPosition = selected_pos;
            H_image.SetActive(true);
            c_now = "H";
        }

        in_question = true;
        timeQuestion = Time.realtimeSinceStartup;
        Invoke("Disappearance", 2);
        charactrer_1_list[selected_question - 1] =
            charactrer_1_list[selected_question - 1].Remove(selected_c_num, 1);
    }
    */
    ////////////////////////////////////////


    // 表示用共通関数
    void ShowImage(string character, Vector2 pos)
    {
        if (character == "N")
        {
            N_image.GetComponent<RectTransform>().anchoredPosition = pos;
            N_image.SetActive(true);
        }
        else
        {
            H_image.GetComponent<RectTransform>().anchoredPosition = pos;
            H_image.SetActive(true);
        }
    }

    int GetAndRemoveRandom(List<int> list)
    {
        if (list.Count == 0)
        {
            //Debug.LogWarning("[WARN] 空リストから選択しようとしました");
            return -1;
        }
        int index = UnityEngine.Random.Range(0, list.Count);
        int val = list[index];
        list.RemoveAt(index);
        return val;
    }

    void Disappearance()
    {
        float ignoredTime = 4.0f;

        csv_writer.GetComponent<csv_RingQuestion>().RightAnswerRecord(
            ope_state.ToString(),
            selected_question.ToString(),
            c_now,
            "ignored",
            ignoredTime.ToString("F4")
        );

        RecordAnsTime(ignoredTime);
        HideImageAndScheduleNext();

        H_image.SetActive(false);
        N_image.SetActive(false);
        //question_sw = true;
        //in_question = false;
    }

    public void LeftAnswer()
    {
        if (!in_question)
        {
            csv_writer.GetComponent<csv_RingQuestion>().LeftAnswerRecord("9999", "9999", "xx", "irrelevant_answer", "9999");
            return;
        }

        timeAnswer = Time.realtimeSinceStartup - timeQuestion;
        bool correct = c_now == "H";
        float logTime = correct ? timeAnswer : 4.0f;

        csv_writer.GetComponent<csv_RingQuestion>().LeftAnswerRecord(
            ope_state.ToString(),
            selected_question.ToString(),
            c_now,
            correct ? "correct" : "mistake",
            logTime.ToString("F4")
        );

        CancelInvoke("Disappearance");
        RecordAnsTime(logTime);
        HideImageAndScheduleNext();
    }

    public void RightAnswer()
    {
        if (!in_question)
        {
            csv_writer.GetComponent<csv_RingQuestion>().RightAnswerRecord("9999", "9999", "xxx", "irrelevant_answer", "9999");
            return;
        }

        timeAnswer = Time.realtimeSinceStartup - timeQuestion;
        bool correct = c_now == "N";
        float logTime = correct ? timeAnswer : 4.0f;

        csv_writer.GetComponent<csv_RingQuestion>().RightAnswerRecord(
            ope_state.ToString(),
            selected_question.ToString(),
            c_now,
            correct ? "correct" : "mistake",
            logTime.ToString("F4")
        );

        CancelInvoke("Disappearance");
        RecordAnsTime(logTime);
        HideImageAndScheduleNext();
    }

    void RecordAnsTime(float val)
    {
        if (all_ans_time[selected_question - 1, ope_state * 2] == 5.0f)
        {
            all_ans_time[selected_question - 1, ope_state * 2] = val;
        }
        else if (all_ans_time[selected_question - 1, ope_state * 2] < 5.0f)
        {
            all_ans_time[selected_question - 1, ope_state * 2 + 1] = val;
            AnsTime[selected_question - 1, ope_state] =
                (all_ans_time[selected_question - 1, ope_state * 2] +
                 all_ans_time[selected_question - 1, ope_state * 2 + 1]) / 2;
        }
    }

    void HideImageAndScheduleNext()
    {
        H_image.SetActive(false);
        N_image.SetActive(false);
        in_question = false;
        float wait_sec = UnityEngine.Random.Range(2.0f, 5.0f);
        Invoke("PrepareNextQuestion", wait_sec);
    }

    void PrepareNextQuestion()
    {
        question_sw = true;
    }

    public void StartCheck()
    {
        start_sw = true;
    }

    // ===== 最小追加：SAGAT一時停止/再開のためのAPI =====
    public void PauseQuestions()
    {
        // 進行中の出題を即キャンセルして非表示
        CancelInvoke("Disappearance");
        if (H_image) H_image.SetActive(false);
        if (N_image) N_image.SetActive(false);
        in_question = false;

        // 再開指示が来るまで次の出題を止める
        question_sw = false;
    }

    public void ResumeQuestions()
    {
        // 次の出題を許可
        question_sw = true;
    }
    public void FinishAndWriteAverage()
    {
        var csv = csv_writer.GetComponent<csv_RingQuestion>();
        if (csv != null) csv.WriteAverageAnsTime(AnsTime);
    }

}
