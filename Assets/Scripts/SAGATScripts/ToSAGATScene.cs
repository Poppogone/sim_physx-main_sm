using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class ToSAGATScene : MonoBehaviour
{
    [Header("遷移先シーン名")]
    public string targetSceneName = "SAGATScene";

    [Header("ブラックアウトUI (黒Image)")]
    public Image blackoutImage;                 // 画面全体を覆う黒Image（Canvas内）
    public float blackoutSeconds = 3.0f;        // ① 黒表示の時間

    [Header("復帰後のクールダウン(秒) ※保険")]
    public float rearmCooldown = 1.0f;
    private static float sRearmCooldown = 1.0f;

    [Header("UIも非表示にする？")]
    [Tooltip("Experiment2 内の UI(Graphic) も見えなくします")]
    public bool alsoHideUI = true;

    private static bool hasFired = false;
    private static bool inTransition = false;
    private static float rearmTime = 0f;

    // 表示先Display継承用
    private static string expSceneName = "";
    private static int expTargetDisplay = 0;

    // blackout の静的参照（SAGAT終了時にも触るため）
    private static Image sBlackout;

    // ---- 非表示にしたコンポーネントの復元用 ----
    private class RendererState { public Renderer r; public bool wasEnabled; }
    private class GraphicState  { public Graphic  g; public bool wasEnabled; }
    private class CameraState   { public Camera   c; public bool wasEnabled; }

    private static readonly List<RendererState> hiddenRenderers = new List<RendererState>();
    private static readonly List<GraphicState>  hiddenGraphics  = new List<GraphicState>();
    private static readonly List<CameraState>   hiddenCameras   = new List<CameraState>();

    void Start()
    {
        sRearmCooldown = (rearmCooldown > 0f) ? rearmCooldown : 1.0f;
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = false; // 衝突で検知
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasFired) return;
        if (Time.time < rearmTime) return;

        // 不要な当たりは無視
        if (collision.gameObject.CompareTag("Obstacle") ||
            collision.gameObject.CompareTag("Ball") ||
            collision.gameObject.CompareTag("Stone"))
            return;

        if (inTransition) return;
        inTransition = true;

        StartCoroutine(TransitionSequence());
    }

    private IEnumerator TransitionSequence()
    {
        var exp = SceneManager.GetActiveScene();
        expSceneName = exp.name;
        Debug.Log($"[ToSAGATScene] Transition started from '{expSceneName}'");

        // 1) 実験処理の一時停止（③）
        PauseAll();

        // 2) ブラックアウトを即時ON（①）
        if (blackoutImage != null)
        {
            var c = blackoutImage.color;
            c.a = 1f;
            blackoutImage.color = c;
            blackoutImage.gameObject.SetActive(true);
            sBlackout = blackoutImage;
        }
        else
        {
            Debug.LogWarning("[ToSAGATScene] blackoutImage not set.");
        }

        // 3) 3秒静止（①）
        yield return new WaitForSecondsRealtime(Mathf.Max(0f, blackoutSeconds));

        // 4) 表示先Displayを記録
        expTargetDisplay = 0;
        foreach (var cam in Resources.FindObjectsOfTypeAll<Camera>())
        {
            if (cam && cam.gameObject.scene == exp && cam.enabled)
            {
                expTargetDisplay = cam.targetDisplay;
                break;
            }
        }

        // 5) Experiment2 の見た目をOFF（負荷削減。黒はすでに出ている）（②の直前）
        HideExperiment2Visuals(exp);

        // 6) SAGAT を Additive でロード（②）
        yield return SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);

        var sagat = SceneManager.GetSceneByName(targetSceneName);
        if (sagat.IsValid()) SceneManager.SetActiveScene(sagat);

        foreach (var cam in Resources.FindObjectsOfTypeAll<Camera>())
        {
            if (cam == null) continue;
            if (cam.gameObject.scene == sagat)
            {
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = Color.black;
                cam.depth = 10;
                cam.targetDisplay = expTargetDisplay;
                cam.enabled = true;
            }
        }

        hasFired = true;
        Debug.Log("[ToSAGATScene] SAGATScene loaded and camera enabled");
    }

    // ===== 実験処理 停止・再開 =====
    private static void PauseAll()
    {
        // グローバルフラグ
        TestDataManager.I?.PauseForSAGAT();
                
        var opcheck = Object.FindObjectOfType<operation_state_check>(true);
        if (opcheck != null)
        {
            opcheck.RecordSAGATEvent("SAGAT_ENTER");
        }

        // RingQuestion を即停止（出題中を消して次を出さない）
        var rar = Object.FindObjectOfType<Ring_AnswerRecord>(true);
        if (rar != null)
            rar.PauseQuestions();

        // 各CSVを一時停止
        var writer = GameObject.Find("CSV writer");
        if (writer != null)
        {
            writer.GetComponent<csv_ControllerLog>()?.PauseLogging();
            writer.GetComponent<csv_operation_state>()?.PauseLogging();
            writer.GetComponent<csv_zx120_position>()?.PauseLogging();
            writer.GetComponent<csv_RingQuestion>()?.PauseLogging(); // ★今はダミーでもOK
        }



        Debug.Log("[ToSAGATScene] PauseAll done (RingQuestion & CSV paused).");
    }

    private static void ResumeAll()
    {
        // 各CSVを再開
        var writer = GameObject.Find("CSV writer");
        if (writer != null)
        {
            writer.GetComponent<csv_ControllerLog>()?.ResumeLogging();
            writer.GetComponent<csv_operation_state>()?.ResumeLogging();
            writer.GetComponent<csv_zx120_position>()?.ResumeLogging();
            writer.GetComponent<csv_RingQuestion>()?.ResumeLogging(); // ★今はダミーでもOK
        }

        // RingQuestionを再開
        var rar = Object.FindObjectOfType<Ring_AnswerRecord>(true);
        if (rar != null)
            rar.ResumeQuestions();

        // フラグ解除
        TestDataManager.I?.ResumeFromSAGAT();

        var opcheck = Object.FindObjectOfType<operation_state_check>(true);
        if (opcheck != null)
        {
            opcheck.RecordSAGATEvent("SAGAT_EXIT");
        }

        Debug.Log("[ToSAGATScene] ResumeAll done (RingQuestion & CSV resumed).");
    }

    // Experiment2 内の見た目だけをオフにする
    private static void HideExperiment2Visuals(Scene exp)
    {
        hiddenRenderers.Clear();
        hiddenGraphics.Clear();
        hiddenCameras.Clear();

        foreach (var r in Resources.FindObjectsOfTypeAll<Renderer>())
        {
            if (!r) continue;
            if (r.gameObject.scene != exp) continue;
            hiddenRenderers.Add(new RendererState { r = r, wasEnabled = r.enabled });
            r.enabled = false;
        }

        if (FindAlsoHideUIFlag())
        {
            foreach (var g in Resources.FindObjectsOfTypeAll<Graphic>())
            {
                if (!g) continue;
                if (g.gameObject.scene != exp) continue;
                hiddenGraphics.Add(new GraphicState { g = g, wasEnabled = g.enabled });
                g.enabled = false;
            }
        }

        foreach (var c in Resources.FindObjectsOfTypeAll<Camera>())
        {
            if (!c) continue;
            if (c.gameObject.scene != exp) continue;
            hiddenCameras.Add(new CameraState { c = c, wasEnabled = c.enabled });
            c.enabled = false;
        }

        Debug.Log($"[ToSAGATScene] Hidden visuals in '{exp.name}': " +
                  $"{hiddenRenderers.Count} Renderers, {hiddenGraphics.Count} UI Graphics, {hiddenCameras.Count} Cameras.");
    }

    private static bool FindAlsoHideUIFlag()
    {
        foreach (var inst in Resources.FindObjectsOfTypeAll<ToSAGATScene>())
        {
            if (inst != null) return inst.alsoHideUI;
        }
        return true;
    }

    private static void RestoreExperiment2Visuals()
    {
        int rc = 0, gc = 0, cc = 0;

        foreach (var st in hiddenRenderers)
        {
            if (st.r) { st.r.enabled = st.wasEnabled; rc++; }
        }
        hiddenRenderers.Clear();

        foreach (var st in hiddenGraphics)
        {
            if (st.g) { st.g.enabled = st.wasEnabled; gc++; }
        }
        hiddenGraphics.Clear();

        foreach (var st in hiddenCameras)
        {
            if (st.c) { st.c.enabled = st.wasEnabled; cc++; }
        }
        hiddenCameras.Clear();

        Debug.Log($"[ToSAGATScene] Restored visuals: {rc} Renderers, {gc} UI Graphics, {cc} Cameras.");
    }

    public static void ReactivateExperiment2()
    {
        Debug.Log($"[ToSAGATScene] Reactivating Experiment2 ('{expSceneName}') ...");

        // 1) Experiment2 の見た目を復元
        RestoreExperiment2Visuals();

        // 2) アクティブシーンを戻す
        var exp = SceneManager.GetSceneByName(expSceneName);
        if (!exp.IsValid())
        {
            Debug.LogWarning($"[ToSAGATScene] Scene '{expSceneName}' not found!");
            inTransition = false;
            return;
        }
        SceneManager.SetActiveScene(exp);
        Debug.Log($"[ToSAGATScene] ActiveScene set to '{exp.name}'");

        rearmTime = Time.time + sRearmCooldown;
        inTransition = false;
    }

    public static void ResetOneShot()
    {
        hasFired = false;
        inTransition = false;
        rearmTime = 0f;
    }

    // === ④ SAGAT終了通知：SAGATExit から呼ぶ ===
    public static void OnSAGATClosed()
    {
        // ブラックアウトを消す
        if (sBlackout != null)
        {
            var c = sBlackout.color;
            c.a = 0f;
            sBlackout.color = c;
            sBlackout.gameObject.SetActive(false);
        }

        // 実験処理を再開（④）
        ResumeAll();

        // 次回の発火ガード解除
        //ResetOneShot();
    }
}
