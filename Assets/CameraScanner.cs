using UnityEngine;

public class CameraScanner : MonoBehaviour
{
    void Start()
    {
        var cams = Resources.FindObjectsOfTypeAll<Camera>();
        Debug.Log("=== Camera scan start ===");
        foreach (var cam in cams)
        {
            if (cam == null) continue;
            var scn = cam.gameObject.scene;
            string sname = scn.IsValid() ? scn.name : "(no scene)";
            Debug.Log($"[CAM] name='{cam.name}', scene='{sname}', enabled={cam.enabled}, " +
                      $"depth={cam.depth}, display={cam.targetDisplay}, clear={cam.clearFlags}");
        }
        Debug.Log("=== Camera scan end ===");
    }
}
