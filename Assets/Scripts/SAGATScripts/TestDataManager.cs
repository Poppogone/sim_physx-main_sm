using UnityEngine;


public class TestDataManager : MonoBehaviour
{
    public static TestDataManager I;   // グローバル参照

    [Header("SAGAT中は true（計測/出題/入力を一時停止）")]
    public bool pausedForSAGAT = false;

    [Header("戻り先のシーン名（SAGATへ行く直前に上書きされる）")]
    public string lastSceneName = "Experiment2";

    [Header("エリア滞在の累積秒（必要数に調整）")]
    public float[] dwellSecondsPerArea = new float[8];

    void Awake()
    {
        // シングルトン確立（重複があれば自分を破棄）
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject); // シーンをまたいで破棄しない
    }

    // ---- 便利メソッド ----
    public void PauseForSAGAT()  { pausedForSAGAT = true; }
    public void ResumeFromSAGAT() { pausedForSAGAT = false; }
    public void SetReturnScene(string sceneName) { lastSceneName = sceneName; }

    // 必要ならユーティリティ（例：全リセット）
    public void ResetAll()
    {
        pausedForSAGAT = false;
        for (int i = 0; i < dwellSecondsPerArea.Length; i++) dwellSecondsPerArea[i] = 0f;
    }
}
