using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallGenerator : MonoBehaviour
{
    public GameObject ballPrefab;
    public int n = 1000;
    public float radius = 0.05f;
    public float depth = 5.0f;//5
    public float width = 1.5f;//3
    private bool transform_sw_1;
    private bool transform_sw_2;
    private bool ball_sw_1 = false;
    //private bool ball_sw_2 = false;


    [SerializeField] private GameObject op_state;

    int area_now;
    float x = 0;
    float y = 0;
    float z = 0;
    GameObject ball;

    // Start is called before the first frame update
    void Start()
    {
        transform_sw_1 = false;
        transform_sw_2 = false;

        for (int i = 0; i < n; i++)
        {
            //ボール生成
            ball = Instantiate(ballPrefab) as GameObject;

            // ボール移動範囲設定
            x = Random.Range(-width / 2.0f + radius, width / 2.0f - radius);
            y = Random.Range(radius - 0.25f, 4.0f);
            z = Random.Range(-depth / 2.0f + radius, depth / 2.0f - radius);

            // ボール移動
            ball.transform.position = new Vector3(27.0f + x, y, -2.5f + z);

            //UnityEngine.Debug.Log("Ok");
        }

        ball_sw_1 = true;
    }


    // Update is called once per frame
    /*
    void Update()
    {
        operation_state_check script;
        script = op_state.GetComponent<operation_state_check>();
        area_now = script.area_now;

        //area5後にarea4に入ると，ボール削除・生成・ボールプール２へ移動
        if(area_now == 0)
        {
            transform_sw_1 = true;
        }

        if(transform_sw_1)
        {
            if(area_now == 4)
            {
                GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
                foreach(GameObject Ball in balls)
                {
                    //ボール削除
                    Destroy(Ball);
                    
                }
                
                for (int i = 0; i < n; i++)
                {
                    //ボール生成
                    ball = Instantiate(ballPrefab) as GameObject;

                    // ボール移動範囲設定
                    x = Random.Range(-width/2.0f + radius, width/2.0f - radius);
                    y = Random.Range(radius - 0.25f, 4.0f);
                    z = Random.Range(-depth/2.0f + radius, depth/2.0f - radius);

                    // ボール移動
                    ball.transform.position = new Vector3(8.25f+x, y, -2.5f+z);

                    UnityEngine.Debug.Log("Trans_2");
                    
                }
                transform_sw_1 = false;
            }
        }

        //area10後にarea4に入ると，ボール削除・生成・ボールプール１へ移動
        if(area_now == 10)
        {
            transform_sw_2 = true;
        }

        if(transform_sw_2)
        {
            if(area_now == 4)
            {
                GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
                foreach(GameObject Ball in balls)
                {
                    //ボール削除
                    Destroy(Ball);
                }
                
                for (int i = 0; i < n; i++)
                {
                    //ボール生成
                    ball = Instantiate(ballPrefab) as GameObject;

                    // ボール移動範囲設定
                    x = Random.Range(-width/2.0f + radius, width/2.0f - radius);
                    y = Random.Range(radius - 0.25f, 4.0f);
                    z = Random.Range(-depth/2.0f + radius, depth/2.0f - radius);

                    // ボール移動
                    ball.transform.position = new Vector3(28f+x, y, -2.5f+z);

                    UnityEngine.Debug.Log("Trans_1");
                }

                transform_sw_2 = false;
            }
        }

    }*/
    
    void Update()
    {
        operation_state_check script;
        script = op_state.GetComponent<operation_state_check>();
        area_now = script.area_now;

        //area0後にarea1に入ると，ボール削除・生成・ボールプール1へ移動
        if(area_now == 0)
        {
            transform_sw_1 = true;
        }

        if(transform_sw_1)
        {
            if(area_now == 1)
            {
                if (!ball_sw_1)
                {
                    GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
                    foreach (GameObject Ball in balls)
                    {
                        //ボール削除
                        Destroy(Ball);

                    }

                    for (int i = 0; i < n; i++)
                    {
                        //ボール生成
                        ball = Instantiate(ballPrefab) as GameObject;

                        // ボール移動範囲設定
                        x = Random.Range(-width / 2.0f + radius, width / 2.0f - radius);
                        y = Random.Range(radius - 0.25f, 4.0f);
                        z = Random.Range(-depth / 2.0f + radius, depth / 2.0f - radius);

                        // ボール移動
                        ball.transform.position = new Vector3(27f + x, y, -2.5f + z);

                        UnityEngine.Debug.Log("Trans_1：area0 → area1");

                    }
                    ball_sw_1 = true;
                }
                transform_sw_1 = false;
            }
        }

        //area3後にarea2に入ると，ボール削除・生成・ボールプール2へ移動
        if(area_now == 3)
        {
            transform_sw_2 = true;
        }

        if(transform_sw_2)
        {
            if(area_now == 2)
            {
                if (ball_sw_1)
                {
                    GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
                    foreach (GameObject Ball in balls)
                    {
                        //ボール削除
                        Destroy(Ball);
                    }

                    for (int i = 0; i < n; i++)
                    {
                        //ボール生成
                        ball = Instantiate(ballPrefab) as GameObject;

                        // ボール移動範囲設定
                        x = Random.Range(-width / 2.0f + radius, width / 2.0f - radius);
                        y = Random.Range(radius - 0.25f, 4.0f);
                        z = Random.Range(-depth / 2.0f + radius, depth / 2.0f - radius);

                        // ボール移動
                        ball.transform.position = new Vector3(7.5f + x, y, -2.5f + z);

                        UnityEngine.Debug.Log("Trans_2:area3 → area2");
                    }
                    ball_sw_1 = false;
                }
                transform_sw_2 = false;
            }
        }

    }

}