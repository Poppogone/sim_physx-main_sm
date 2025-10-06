using UnityEngine;
using UnityEngine.SceneManagement;

public class SAGATOverlayController : MonoBehaviour
{
    [Header("SAGATを閉じるときに呼ぶ")]
    public void Exit()
    {
        string thisScene = gameObject.scene.name;  // 今のシーン（= SAGAT）
        SceneManager.UnloadSceneAsync(thisScene).completed += _ =>
        {
            // Experiment2 側の Loader に「閉じたよ」を通知
            SAGATOverlayLoader.NotifyClosed();
        };
    }

    void OnEnable()
    {
        // イベントシステムが重複する場合は、SAGAT側の EventSystem を「無い」状態にするか
        // Experiment2側の EventSystem を活かして、SAGAT Canvas を Screen Space - Overlay に設定。
        // 必要ならここで調整してください（プロジェクトに合わせて）。
    }
}
