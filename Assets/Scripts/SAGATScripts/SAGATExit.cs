using UnityEngine;
using UnityEngine.SceneManagement;

public class SAGATExit : MonoBehaviour
{
    public string sagatSceneName = "SAGATScene";

    public void OnClickReturn()
    {
        Debug.Log("[SAGATExit] Unloading SAGATScene...");
        var sagat = SceneManager.GetSceneByName(sagatSceneName);
        if (!sagat.IsValid())
        {
            Debug.LogWarning($"[SAGATExit] Scene '{sagatSceneName}' not found.");
            return;
        }

        SceneManager.UnloadSceneAsync(sagat).completed += (op) =>
        {
            Debug.Log("[SAGATExit] SAGATScene unloaded.");
            ToSAGATScene.ReactivateExperiment2();

            // ④ 再開処理とブラックアウト解除
            ToSAGATScene.OnSAGATClosed();
        };
    }
}
