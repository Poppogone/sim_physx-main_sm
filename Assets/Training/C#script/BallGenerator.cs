using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallGenerator : MonoBehaviour
{
    public GameObject ballPrefab;
    public int n = 1000;
    public float radius = 0.05f;
    public float depth = 5;
    public float width = 3;
    private bool transform_sw_1;
    private bool transform_sw_2;
    
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
            x = Random.Range(-width/2.0f + radius, width/2.0f - radius);
            y = Random.Range(radius - 0.25f, 4.0f);
            z = Random.Range(-depth/2.0f + radius, depth/2.0f - radius);

            // ボール移動
            ball.transform.position = new Vector3(x, y, -9.75f+z);

            UnityEngine.Debug.Log("Ok");
        }
    }


    // Update is called once per frame
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
                    ball.transform.position = new Vector3(-54.7f+x, y, 35.15f+z);

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
                    ball.transform.position = new Vector3(x, y, -9.75f+z);

                    UnityEngine.Debug.Log("Trans_1");
                }

                transform_sw_2 = false;
            }
        }

    }
}