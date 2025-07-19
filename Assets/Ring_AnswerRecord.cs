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

        StartCheck();
    }

    void Update()
    {
        if (start_sw && question_sw && !in_question)
        {
            question_sw = false;
            Question();
        }
    }

    void Question()
    {
        var script = op_state.GetComponent<operation_state_check>();
        area_now = script.area_now;

        if (area_now == 0 || area_now == 3)
            ope_state = 0;
        else if (area_now == 1 || area_now == 2)
            ope_state = 1;
        else
        {
            Debug.Log("[INFO] 未定義エリア。出題スキップ");
            Invoke("PrepareNextQuestion", 1.0f);
            return;
        }

        List<int> list1 = (ope_state == 0) ? question_0_list_1 : question_1_list_1;
        List<int> list2 = (ope_state == 0) ? question_0_list_2 : question_1_list_2;
        List<string> charList = (ope_state == 0) ? charactrer_0_list : charactrer_1_list;

        if (list1.Count == 0 && list2.Count == 0)
        {
            Debug.Log($"[INFO] 全問出題済み (ope_state={ope_state})");
            Invoke("PrepareNextQuestion", 1.0f);
            return;
        }

        selected_question = list1.Count == 0 ? GetAndRemoveRandom(list2) : GetAndRemoveRandom(list1);
        if (selected_question == -1)
        {
            Invoke("PrepareNextQuestion", 1.0f);
            return;
        }

        selected_pos = text_pos_list[selected_question - 1];

        string charSet = charList[selected_question - 1];
        if (string.IsNullOrEmpty(charSet))
        {
            Debug.LogWarning($"[WARN] charList[{selected_question - 1}] が空。スキップ");
            Invoke("PrepareNextQuestion", 0.5f);
            return;
        }

        selected_c_num = UnityEngine.Random.Range(0, charSet.Length);
        c_now = charSet.Substring(selected_c_num, 1);
        charList[selected_question - 1] = charSet.Remove(selected_c_num, 1);

        if (c_now == "N")
        {
            N_image.GetComponent<RectTransform>().anchoredPosition = selected_pos;
            N_image.SetActive(true);
        }
        else
        {
            H_image.GetComponent<RectTransform>().anchoredPosition = selected_pos;
            H_image.SetActive(true);
        }

        in_question = true;
        timeQuestion = Time.realtimeSinceStartup;
        Invoke("Disappearance", 2.0f);

        Debug.Log($"[DEBUG] 出題: {selected_question} Char:{c_now}");
    }

    int GetAndRemoveRandom(List<int> list)
    {
        if (list.Count == 0)
        {
            Debug.LogWarning("[WARN] 空リストから選択しようとしました");
            return -1;
        }
        int index = UnityEngine.Random.Range(0, list.Count);
        int val = list[index];
        list.RemoveAt(index);
        return val;
    }

    void Disappearance()
    {
        timeAnswer = Time.realtimeSinceStartup - timeQuestion;
        csv_writer.GetComponent<csv_RingQuestion>().RightAnswerRecord(ope_state.ToString(), selected_question.ToString(), c_now, "ignored", timeAnswer.ToString());
        RecordAnsTime(4.0f);
        HideImageAndScheduleNext();
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
        csv_writer.GetComponent<csv_RingQuestion>().LeftAnswerRecord(ope_state.ToString(), selected_question.ToString(), c_now, correct ? "correct" : "mistake", timeAnswer.ToString());
        CancelInvoke("Disappearance");
        RecordAnsTime(correct ? timeAnswer : 4.0f);
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
        csv_writer.GetComponent<csv_RingQuestion>().RightAnswerRecord(ope_state.ToString(), selected_question.ToString(), c_now, correct ? "correct" : "mistake", timeAnswer.ToString());
        CancelInvoke("Disappearance");
        RecordAnsTime(correct ? timeAnswer : 4.0f);
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
}
