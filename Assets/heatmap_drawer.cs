using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class heatmap_drawer : MonoBehaviour
{
    question_and_record_1 script;
    private float[,] AnswerTime = new float[27, 5];
    public bool hm_sw_0;
    public bool hm_sw_1;
    public bool hm_sw_2;
    public bool hm_sw_3;
    public bool hm_sw_4;
    float red;
    float green;
    float blue;

    [SerializeField] public List<Image> hmImage = new List<Image>();
    [SerializeField] public List<Text> hmText = new List<Text>();

    public List<Color> hmColor0 = new List<Color>();
    public List<Color> hmColor1 = new List<Color>();
    public List<Color> hmColor2 = new List<Color>();
    public List<Color> hmColor3 = new List<Color>();
    public List<Color> hmColor4 = new List<Color>();
    //public List<Color> hmColor5 = new List<Color>();
    public List<string> hmNum0 = new List<string>();
    public List<string> hmNum1 = new List<string>();
    public List<string> hmNum2 = new List<string>();
    public List<string> hmNum3 = new List<string>();
    public List<string> hmNum4 = new List<string>();
    //public List<string> hmNum5 = new List<string>();
    //public List<float> Ans0 = new List<float>();
    
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 27; i++)
        {
            hmImage[i].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            hmText[i].text = "2.0";
        }
    }

    // Update is called once per frame
    void Update()
    {
        // to_heatmap.cs から ansTime を引用
        AnswerTime = to_heatmap.ansTime;
        string[,] AnswerTime_str = new string[27, 4];
        
        hm_sw_0 = true;
        hm_sw_1 = true;
        hm_sw_2 = true;
        hm_sw_3 = true;
        //hm_sw_4 = true;

        for(int j = 0; j < 4; j++)
        {
            for(int i = 0; i < 27; i++)
            {
                if(AnswerTime[i, j] > 4.0f)  // 回答時間が未記録の場合
                {
                    if(j == 0)
                    {
                        hm_sw_0 = false; 
                    }
                    else if(j == 1)
                    {
                        hm_sw_1 = false; 
                    }
                    else if(j == 2)
                    {
                        hm_sw_2 = false; 
                    }
                    else if(j == 3)
                    {
                        hm_sw_3 = false; 
                    }
                    /*else if(j == 4)
                    {
                        hm_sw_4 = false; 
                    }*/
                }

                if(AnswerTime[i, j] >= 0f && AnswerTime[i, j] <= 2.0f)  // 回答時間が0～2秒の時
                {
                    red = 255f;
                    green = 128f + (128f * AnswerTime[i, j] / 2.0f);
                    blue = 255f * AnswerTime[i, j] / 2.0f;
                }
                else if(AnswerTime[i, j] > 2.0f && AnswerTime[i, j] <= 4.0f)  // 回答時間が2～4秒の時
                {
                    red = 255f * (4.0f - AnswerTime[i, j]) / 2.0f;
                    green = 128f + (128f * (4.0f - AnswerTime[i, j]) / 2.0f);
                    blue = 255f;
                }
            }
        }

        if(hm_sw_0)
        {
            for(int i = 0; i < 27; i++)
            {
                if(AnswerTime[i, 0] >= 0f && AnswerTime[i, 0] <= 2.0f)
                {
                    red = 1.0f;
                    green = 0.0f + (1.0f * AnswerTime[i, 0] / 2.0f);
                    blue = 1.0f * AnswerTime[i, 0] / 2.0f;
                }
                else if(AnswerTime[i, 0] > 2.0f && AnswerTime[i, 0] <= 4.0f)
                {
                    red = 1.0f * (4.0f - AnswerTime[i, 0]) / 2.0f;
                    green = 0.3f + (0.7f * (4.0f - AnswerTime[i, 0]) / 2.0f);
                    blue = 1.0f;
                }
                
                hmColor0.Add(new Color(red, green, blue, 1.0f));

                AnswerTime_str[i, 0] = AnswerTime[i, 0].ToString("F1", CultureInfo.CurrentCulture);
                hmNum0.Add(AnswerTime_str[i, 0]);
            }
        }
        if(hm_sw_1)
        {
            for(int i = 0; i < 27; i++)
            {
                if(AnswerTime[i, 1] >= 0f && AnswerTime[i, 1] <= 2.0f)
                {
                    red = 1.0f;
                    green = 0.0f + (1.0f * AnswerTime[i, 1] / 2.0f);
                    blue = 1.0f * AnswerTime[i, 1] / 2.0f;
                }
                else if(AnswerTime[i, 1] > 2.0f && AnswerTime[i, 1] <= 4.0f)
                {
                    red = 1.0f * (4.0f - AnswerTime[i, 1]) / 2.0f;
                    green = 0.3f + (0.7f * (4.0f - AnswerTime[i, 1]) / 2.0f);
                    blue = 1.0f;
                }
                
                hmColor1.Add(new Color(red, green, blue, 1.0f));
                AnswerTime_str[i, 1] = AnswerTime[i, 1].ToString("F1", CultureInfo.CurrentCulture);
                hmNum1.Add(AnswerTime_str[i, 1]);
            }
        }
        if(hm_sw_2)
        {
            for(int i = 0; i < 27; i++)
            {
                if(AnswerTime[i, 2] >= 0f && AnswerTime[i, 2] <= 2.0f)
                {
                    red = 1.0f;
                    green = 0.0f + (1.0f * AnswerTime[i, 2] / 2.0f);
                    blue = 1.0f * AnswerTime[i, 2] / 2.0f;
                }
                else if(AnswerTime[i, 2] > 2.0f && AnswerTime[i, 2] <= 4.0f)
                {
                    red = 1.0f * (4.0f - AnswerTime[i, 2]) / 2.0f;
                    green = 0.3f + (0.7f * (4.0f - AnswerTime[i, 2]) / 2.0f);
                    blue = 1.0f;
                }
                
                hmColor2.Add(new Color(red, green, blue, 1.0f));
                AnswerTime_str[i, 2] = AnswerTime[i, 2].ToString("F1", CultureInfo.CurrentCulture);
                hmNum2.Add(AnswerTime_str[i, 2]);
            }
        }
        if(hm_sw_3)
        {
            for(int i = 0; i < 27; i++)
            {
                if(AnswerTime[i, 3] >= 0f && AnswerTime[i, 3] <= 2.0f)
                {
                    red = 1.0f;
                    green = 0.0f + (1.0f * AnswerTime[i, 3] / 2.0f);
                    blue = 1.0f * AnswerTime[i, 3] / 2.0f;
                }
                else if(AnswerTime[i, 3] > 2.0f && AnswerTime[i, 3] <= 4.0f)
                {
                    red = 1.0f * (4.0f - AnswerTime[i, 3]) / 2.0f;
                    green = 0.3f + (0.7f * (4.0f - AnswerTime[i, 3]) / 2.0f);
                    blue = 1.0f;
                }
                
                hmColor3.Add(new Color(red, green, blue, 1.0f));
                AnswerTime_str[i, 3] = AnswerTime[i, 3].ToString("F1", CultureInfo.CurrentCulture);
                hmNum3.Add(AnswerTime_str[i, 3]);
            }
        }
        /*if(hm_sw_4)
        {
            for(int i = 0; i < 27; i++)
            {
                if(AnswerTime[i, 4] >= 0f && AnswerTime[i, 4] <= 2.0f)
                {
                    red = 1.0f;
                    green = 0.0f + (1.0f * AnswerTime[i, 4] / 2.0f);
                    blue = 1.0f * AnswerTime[i, 4] / 2.0f;
                }
                else if(AnswerTime[i, 4] > 2.0f && AnswerTime[i, 4] <= 4.0f)
                {
                    red = 1.0f * (4.0f - AnswerTime[i, 4]) / 2.0f;
                    green = 0.3f + (0.7f * (4.0f - AnswerTime[i, 4]) / 2.0f);
                    blue = 1.0f;
                }
                
                hmColor4.Add(new Color(red, green, blue, 1.0f));
                AnswerTime_str[i, 4] = AnswerTime[i, 4].ToString("F1", CultureInfo.CurrentCulture);
                hmNum4.Add(AnswerTime_str[i, 4]);
            }
        }*/
    }


    public void heatmap0()  // 走行
    {
        for(int i = 0; i < 27; i++)
        {
            hmImage[i].color = hmColor0[i];
            hmText[i].text = hmNum0[i];
        }
    } 
    public void heatmap1()  // 把持・配置１
    {
        for(int i = 0; i < 27; i++)
        {
            hmImage[i].color = hmColor1[i];
            hmText[i].text = hmNum1[i];
        }
    } 
    public void heatmap2()  // 把持・配置２
    {
        for(int i = 0; i < 27; i++)
        {
            hmImage[i].color = hmColor2[i];
            hmText[i].text = hmNum2[i];
        }
    } 
    public void heatmap3()  // 掘削・積込
    {
        for(int i = 0; i < 27; i++)
        {
            hmImage[i].color = hmColor3[i];
            hmText[i].text = hmNum3[i];
        }
    } 
    /*public void heatmap4()  // 把持・配置３
    {
        for(int i = 0; i < 27; i++)
        {
            hmImage[i].color = hmColor4[i];
            hmText[i].text = hmNum4[i];
        }
    } */
    /*public void heatmap5()  // デモ用
    {
        float[] num = {3.8f, 1.6f, 0.9f, 4.0f, 1.0f, 0.8f, 1.8f, 1.2f, 1.3f, 
                       1.7f, 1.4f, 1.5f, 1.1f, 0.7f, 1.3f, 1.4f, 0.8f, 2.8f,
                       1.6f, 1.8f, 1.3f, 3.2f, 1.4f, 1.7f, 3.1f, 4.0f, 3.8f};
        
        for(int i = 0; i < 27; i++)
        {
            if(num[i] >= 0f && num[i] <= 2.0f)
            {
                red = 1.0f;
                green = 0.0f + (1.0f * num[i] / 2.0f);
                blue = 1.0f * num[i] / 2.0f;
            }
            else if(num[i] > 2.0f && num[i] <= 4.0f)
            {
                red = 1.0f * (4.0f - num[i]) / 2.0f;
                green = 0.3f + (0.7f * (4.0f - num[i]) / 2.0f);
                blue = 1.0f;
            }
            
            hmImage[i].color = new Color(red, green, blue, 1.0f);
            hmText[i].text = num[i].ToString("F1", CultureInfo.CurrentCulture);
        }
    } */

}