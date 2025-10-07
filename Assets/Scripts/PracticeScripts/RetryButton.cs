using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryButton : MonoBehaviour
{
    // この関数がボタンをクリックしたときに呼ばれます
    public void RetryGame()
    {
        Debug.Log("[RetryButton] RetryGame() clicked");
        // 現在のシーンを再ロードします
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}