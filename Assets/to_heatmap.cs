using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class to_heatmap : MonoBehaviour
{
    //public static float[,] ansTime;
    [SerializeField] private GameObject Chara_pop_1;  
    public static float[,] ansTime = new float[27, 4];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToHeatmap()
    {
        question_and_record_1 script;
        script = Chara_pop_1.GetComponent<question_and_record_1>();
        //public static float[,] ansTime = new float[27, 5];
        ansTime = script.AnsTime;  // question_and_record_1.cs から
        SceneManager.LoadScene("Heatmap");
    }
}
