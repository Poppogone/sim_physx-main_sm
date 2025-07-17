using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class short_cut : MonoBehaviour
{

    [SerializeField] private GameObject target_1;
    [SerializeField] private GameObject target_2;
    [SerializeField] private GameObject target_3;
    [SerializeField] private GameObject target_4;
    [SerializeField] private GameObject target_5;
    [SerializeField] private GameObject target_6;
    [SerializeField] private GameObject target_7;
    [SerializeField] private GameObject target_8;

    //public GameObject ballPrefab;
    public int n = 2000;
    public float radius = 0.05f;
    public float depth = 5;
    public float width = 3;
    float x = 0;
    float y = 0;
    float z = 0;
    //GameObject ball;

    [SerializeField] private GameObject setting_screen;
    [SerializeField] private GameObject waiting_screen;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Alpha1))
        {
            target_1.SetActive(false);
        }
        if(Input.GetKey(KeyCode.Alpha2))
        {
            target_2.SetActive(false);
        }
        if(Input.GetKey(KeyCode.Alpha3))
        {
            target_3.SetActive(false);
        }
        if(Input.GetKey(KeyCode.Alpha4))
        {
            target_4.SetActive(false);
        }
        if(Input.GetKey(KeyCode.Alpha5))
        {
            target_5.SetActive(false);
        }
        if(Input.GetKey(KeyCode.Alpha6))
        {
            target_6.SetActive(false);
        }
        if(Input.GetKey(KeyCode.Alpha7))
        {
            target_7.SetActive(false);
        }
        if(Input.GetKey(KeyCode.Alpha8))
        {
            target_8.SetActive(false);
        }

        /*if(Input.GetKey(KeyCode.A))
        {
            GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
            foreach(GameObject Ball in balls)
            {
                //ボール削除
                Destroy(Ball);
            }
            
            for (int i = 0; i < n; i++)
            {
                GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
                foreach(GameObject Ball in balls)
                {
                    //ボール削除
                    Destroy(Ball);
                }

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
        }*/

        if(Input.GetKey(KeyCode.Alpha0))
        {
            setting_screen.SetActive(false);
            waiting_screen.SetActive(false);
        }
        if(Input.GetKey(KeyCode.Alpha9))
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        
    }
}
