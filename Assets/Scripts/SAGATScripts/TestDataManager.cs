using System.Collections.Generic;
using UnityEngine;

public class TestDataManager : MonoBehaviour
{
    public static TestDataManager I;

    [Header("SAGAT中は true（計測/出題/入力を一時停止）")]
    public bool pausedForSAGAT = false;

    [Header("戻り先のシーン名（SAGATへ行く直前に上書きされる）")]
    public string lastSceneName = "Experiment2";

    [Header("エリア滞在の累積秒（必要に応じてサイズ変更）")]
    public float[] dwellSecondsPerArea = new float[12];

    // ===== 追記再開に必要なパスと累積秒（ログごと） =====
    [Header("ログ継続用：ControllerLog")]
    public string controllerLogPath = "";
    public float controllerElapsed = 0f;

    [Header("ログ継続用：operation_state")]
    public string opstateLogPath = "";
    public float opstateElapsed = 0f;

    [Header("ログ継続用：RingQuestion")]
    public string ringLogPath = "";
    public float ringElapsed = 0f;

    [Header("ログ継続用：zx120_position")]
    public string posLogPath = "";
    public float posElapsed = 0f;

    // ======== ここから追加：zx120 の状態スナップショット保存用 ========
    [System.Serializable]
    public class TransformSnapshot
    {
        public Vector3 worldPosition;             // zx120ルート位置
        public Quaternion worldRotation;          // zx120ルート姿勢
        public Vector3 rbVelocity;                // 速度
        public Vector3 rbAngularVelocity;         // 角速度
        public Dictionary<string, Quaternion> partLocalRotations = new Dictionary<string, Quaternion>();
    }

    [Header("zx120の状態スナップショット")]
    public TransformSnapshot zx120Snapshot;       // 最新の保存状態
    // ===============================================================

    void Awake()
    {
        if (I == null) { I = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    public void PauseForSAGAT()  { pausedForSAGAT = true; }
    public void ResumeFromSAGAT(){ pausedForSAGAT = false; }
    public void SetReturnScene(string name){ lastSceneName = name; }

    public void ResetAll()
    {
        pausedForSAGAT = false;
        for (int i = 0; i < dwellSecondsPerArea.Length; i++)
            dwellSecondsPerArea[i] = 0f;

        controllerLogPath = ""; controllerElapsed = 0f;
        opstateLogPath   = ""; opstateElapsed   = 0f;
        ringLogPath      = ""; ringElapsed      = 0f;
        posLogPath       = ""; posElapsed       = 0f;

        zx120Snapshot = null; // ← 保存状態もリセット
    }
}
