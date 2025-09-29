using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class ToSAGATScene : MonoBehaviour
{
    [Header("遷移先シーン名")]
    public string targetSceneName = "SAGATScene";

    [Header("ブラックアウトUI (黒Image)")]
    public Image blackoutImage;   // Inspectorで黒Imageを割り当てておく

    [Tooltip("このColliderをTriggerにする（推奨）")]
    public bool forceTrigger = true;

    private static bool hasActivated = false; // 全体で一度だけ有効にするフラグ
    private bool fired = false;               // このオブジェクトでの発火防止

    void Awake()
    {
        var col = GetComponent<Collider>();
        if (forceTrigger) col.isTrigger = true;
        SetAlpha(0f); // 最初は透明
    }

    void OnTriggerEnter(Collider other)
    {
        if (fired || hasActivated) return;  // すでに実行済みなら無効
        fired = true;
        hasActivated = true;               // 今後は無効化
        StartCoroutine(LoadSAGAT());
    }

    void OnCollisionEnter(Collision collision)
    {
        if (fired || hasActivated) return;
        fired = true;
        hasActivated = true;
        StartCoroutine(LoadSAGAT());
    }

    IEnumerator LoadSAGAT()
    {
        if (TestDataManager.I != null)
        {
            TestDataManager.I.SetReturnScene(SceneManager.GetActiveScene().name);
            TestDataManager.I.PauseForSAGAT();
        }

        SetAlpha(1f);  // 即座に黒画面
        yield return new WaitForSecondsRealtime(3f);
        SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);
    }

    void SetAlpha(float a)
    {
        if (blackoutImage != null)
        {
            var c = blackoutImage.color;
            c.a = Mathf.Clamp01(a);
            blackoutImage.color = c;
        }
    }
}
