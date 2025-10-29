using UnityEngine;

public class TargetDisplayWatchdog : MonoBehaviour
{
    Camera cam;
    int last;

    void Awake()
    {
        cam = GetComponent<Camera>();
        last = cam.targetDisplay;
        Debug.Log($"[TD] Awake: {last}");
    }

    void Update()
    {
        if (cam.targetDisplay != last)
        {
            Debug.LogWarning($"[TD] CHANGED: {last} -> {cam.targetDisplay}\n{System.Environment.StackTrace}");
            last = cam.targetDisplay;
        }
    }

    void OnEnable()
    {
        // 強制固定したいときはこれを有効化
        // cam = cam ?? GetComponent<Camera>();
        // cam.targetDisplay = 1; // Display2 を使うなら 1（0始まり: 0=Display1, 1=Display2）
    }
}
