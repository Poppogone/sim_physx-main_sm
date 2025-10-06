using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // UIのGraphicを消す場合に使う

[RequireComponent(typeof(Collider))]
public class ToSAGATScene : MonoBehaviour
{
    [Header("遷移先シーン名")]
    public string targetSceneName = "SAGATScene";

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

        EnterSAGAT();
    }

    void EnterSAGAT()
    {
        var exp = SceneManager.GetActiveScene();
        expSceneName = exp.name;
        Debug.Log($"[ToSAGATScene] Transition started from '{expSceneName}'");

        // 1) 表示先Display（Experiment2の代表カメラ）を記録
        expTargetDisplay = 0;
        foreach (var cam in Resources.FindObjectsOfTypeAll<Camera>())
        {
            if (cam && cam.gameObject.scene == exp && cam.enabled)
            {
                expTargetDisplay = cam.targetDisplay;
                break;
            }
        }

        // 2) Experiment2内の「見た目」だけ消す（Renderer/UI Graphic/Camera）
        HideExperiment2Visuals(exp);

        // 3) SAGATをAdditiveでロードし、SAGAT側カメラを有効化
        SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive)
            .completed += (op) =>
            {
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
            };
    }

    // Experiment2 内の見た目だけをオフにする
    private static void HideExperiment2Visuals(Scene exp)
    {
        hiddenRenderers.Clear();
        hiddenGraphics.Clear();
        hiddenCameras.Clear();

        // Renderer（Mesh / SkinnedMesh / Sprite / ParticleSystemRenderer など全部含む）
        foreach (var r in Resources.FindObjectsOfTypeAll<Renderer>())
        {
            if (!r) continue;
            if (r.gameObject.scene != exp) continue;

            hiddenRenderers.Add(new RendererState { r = r, wasEnabled = r.enabled });
            r.enabled = false;
        }

        // UI（希望する場合のみ）
        if (/* alsoHideUI */ FindAlsoHideUIFlag())
        {
            foreach (var g in Resources.FindObjectsOfTypeAll<Graphic>())
            {
                if (!g) continue;
                if (g.gameObject.scene != exp) continue;

                hiddenGraphics.Add(new GraphicState { g = g, wasEnabled = g.enabled });
                g.enabled = false;
            }
        }

        // Experiment2側カメラも無効化（SAGAT側カメラだけ描画）
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

    // Inspectorの alsoHideUI を静的文脈から読むためのヘルパ（同一シーン内の任意1つのインスタンスから拾う）
    private static bool FindAlsoHideUIFlag()
    {
        foreach (var inst in Resources.FindObjectsOfTypeAll<ToSAGATScene>())
        {
            if (inst != null) return inst.alsoHideUI;
        }
        return true; // 見つからなければ既定で true
    }

    // 非表示にしたものを元に戻す
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
}
