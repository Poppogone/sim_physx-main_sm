using UnityEngine;
using UnityEngine.SceneManagement;

public class SAGATExit : MonoBehaviour
{
    public void OnClickReturn()
    {
        // 一時停止解除
        if (TestDataManager.I != null)
            TestDataManager.I.ResumeFromSAGAT();

        // 記録しておいた元シーンへ
        string target = (TestDataManager.I != null && !string.IsNullOrEmpty(TestDataManager.I.lastSceneName))
            ? TestDataManager.I.lastSceneName
            : "Experiment2";  // フォールバック

        SceneManager.LoadScene(target, LoadSceneMode.Single);
    }
}
