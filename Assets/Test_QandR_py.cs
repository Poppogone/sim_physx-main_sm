using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_QandR_py : MonoBehaviour
{
    bool start_sw = false;
    bool question_sw = true;
    bool in_question = false;

    [SerializeField] private List<int> question_0_list_1 = new List<int>();
    [SerializeField] private List<int> question_0_list_2 = new List<int>();
    [SerializeField] private List<string> charactrer_0_list = new List<string>();

    [SerializeField] private List<int> question_1_list_1 = new List<int>();
    [SerializeField] private List<int> question_1_list_2 = new List<int>();
    [SerializeField] private List<string> charactrer_1_list = new List<string>();

    List<Vector2> text_pos_list = new List<Vector2>();

    int selected_question;
    Vector2 selected_pos;
    int selected_c_num;

    [SerializeField] private GameObject N_image;
    [SerializeField] private GameObject H_image;
    [SerializeField] private GameObject op_state;

    string c_now;
    int area_now;
    int ope_state;

    void Start()
    {
        // 出題位置設定（元のまま）
        text_pos_list.AddRange(new Vector2[] {
            new Vector2 (350,960), new Vector2 (520,960), new Vector2 (690,960),
            new Vector2 (350,830), new Vector2 (520,830), new Vector2 (690,830),
            new Vector2 (350,700), new Vector2 (520,700), new Vector2 (690,700),
            new Vector2 (1190,960), new Vector2 (1360,960), new Vector2 (1530,960),
            new Vector2 (1190,830), new Vector2 (1360,830), new Vector2 (1530,830),
            new Vector2 (1190,700), new Vector2 (1360,700), new Vector2 (1530,700),
            new Vector2 (770,420), new Vector2 (940,420), new Vector2 (1110,420),
            new Vector2 (770,290), new Vector2 (940,290), new Vector2 (1110,290),
            new Vector2 (770,160), new Vector2 (940,160), new Vector2 (1110,160)
        });

        StartCheck();
    }

    void Update()
    {
        if (start_sw && question_sw && !in_question)
        {
            question_sw = false;
            StartCoroutine(QuestionAfterDelay());
        }
    }

    IEnumerator QuestionAfterDelay()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(1.0f, 2.0f));
        if (!in_question)
        {
            Question();
        }
        else
        {
            question_sw = true;
        }
    }

    void Question()
    {
        op_state.TryGetComponent(out operation_state_check script);
        area_now = script != null ? script.area_now : 0;
        ope_state = (area_now == 1 || area_now == 2) ? 1 : 0;

        // 出題前に画像を確実に消す
        N_image.SetActive(false);
        H_image.SetActive(false);

        // 出題ロジック
        if (ope_state == 0)
        {
            selected_question = SelectFromList(question_0_list_1, question_0_list_2);
            selected_pos = text_pos_list[selected_question - 1];
            selected_c_num = Random.Range(0, charactrer_0_list[selected_question - 1].Length);
            ShowImage(charactrer_0_list, selected_question, selected_c_num, selected_pos, isState1: false);
        }
        else
        {
            selected_question = SelectFromList(question_1_list_1, question_1_list_2);
            selected_pos = text_pos_list[selected_question - 1];
            selected_c_num = Random.Range(0, charactrer_1_list[selected_question - 1].Length);
            ShowImage(charactrer_1_list, selected_question, selected_c_num, selected_pos, isState1: true);
        }

        in_question = true;
        Invoke("Disappearance", 2f);
    }

    int SelectFromList(List<int> list1, List<int> list2)
    {
        if (list1.Count == 0 && list2.Count == 0)
        {
            Debug.Log("[INFO] 全問題終了");
            return 1; // 仮で戻す（出題を止めるならここで制御可能）
        }

        int val;
        if (list1.Count == 0 && list2.Count > 0)
        {
            val = list2[Random.Range(0, list2.Count)];
            list2.Remove(val);
        }
        else
        {
            val = list1[Random.Range(0, list1.Count)];
            list1.Remove(val);
        }
        return val;
    }

    void ShowImage(List<string> charList, int q, int c_num, Vector2 pos, bool isState1)
    {
        string c = charList[q - 1].Substring(c_num, 1);
        charList[q - 1] = charList[q - 1].Remove(c_num, 1);
        c_now = c;

        if (c == "N")
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

    void Disappearance()
    {
        H_image.SetActive(false);
        N_image.SetActive(false);
        question_sw = true;
        in_question = false;
    }

    public void StartCheck()
    {
        start_sw = true;
    }
}
