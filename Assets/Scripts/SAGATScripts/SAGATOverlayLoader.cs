using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SAGATOverlayLoader : MonoBehaviour
{
    public static SAGATOverlayLoader Instance { get; private set; }

    [Header("SAGAT シーン名")]
    [SerializeField] private string sagatSceneName = "SAGATScene";

    [Header("必要なら参照（null可）")]
    [SerializeField] private csv_ControllerLog controllerLog;
    [SerializeField] private csv_RingQuestion ringQuestion;
    [SerializeField] private csv_operation_state opStateLog;
    [SerializeField] private csv_zx120_position positionLog;

    [Header("時間停止オプション（物理やアニメも止めたいならON）")]
    public bool freezeTimeDuringSAGAT = false;

    private string baseSceneName;   // 元のシーン名を記録
    private string activeBefore;    // 復帰時に戻すアクティブシーン名

    void Awake()
    {
        Instance = this;
    }

    // === 開く（Additiveロード） ===
    public void OpenSAGAT()
    {
        if (TestDataManager.I != null)
        {
            baseSceneName = SceneManager.GetActiveScene().name;
            TestDataManager.I.SetReturnScene(baseSceneName);
            TestDataManager.I.PauseForSAGAT();
        }

        // ログを一時停止（任意。null ならスキップ）
        controllerLog?.PauseLogging();
        ringQuestion?.PauseLogging();
        opStateLog?.PauseLogging();
        positionLog?.PauseLogging();

        if (freezeTimeDuringSAGAT) Time.timeScale = 0f;

        StartCoroutine(LoadAdditiveAndActivate());
    }

    private IEnumerator LoadAdditiveAndActivate()
    {
        var op = SceneManager.LoadSceneAsync(sagatSceneName, LoadSceneMode.Additive);
        while (!op.isDone) yield return null;

        // SAGATをアクティブシーンに（UIフォーカス/Find等のため）
        var sagatScene = SceneManager.GetSceneByName(sagatSceneName);
        activeBefore = SceneManager.GetActiveScene().name;
        SceneManager.SetActiveScene(sagatScene);
    }

    // === SAGAT側から閉じる通知を受けた後に呼ばれる ===
    public void OnSAGATClosed()
    {
        if (freezeTimeDuringSAGAT) Time.timeScale = 1f;

        // アクティブシーンを元に戻す
        if (!string.IsNullOrEmpty(activeBefore))
        {
            var baseScene = SceneManager.GetSceneByName(activeBefore);
            if (baseScene.IsValid()) SceneManager.SetActiveScene(baseScene);
        }

        // 再開
        if (TestDataManager.I != null) TestDataManager.I.ResumeFromSAGAT();

        controllerLog?.ResumeLogging();
        ringQuestion?.ResumeLogging();
        opStateLog?.ResumeLogging();
        positionLog?.ResumeLogging();
    }

    // SAGAT側から static に叩くためのヘルパ
    public static void NotifyClosed() => Instance?.OnSAGATClosed();
}
