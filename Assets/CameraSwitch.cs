using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] private GameObject UAVcamera_task1;
    [SerializeField] private GameObject UAVcamera_task2;
    [SerializeField] private GameObject op_state;

    private int area_now = -1;

    void Update()
    {
        operation_state_check script = op_state.GetComponent<operation_state_check>();
        area_now = script.area_now;

        Debug.Log($"[CameraSwitch] area_now = {area_now}");

        if (area_now == 2)
        {
            UAVcamera_task1.SetActive(false);
            UAVcamera_task2.SetActive(true);
            Debug.Log("[CameraSwitch] → camera2 に切り替え (area 2)");
        }
        else
        {
            UAVcamera_task2.SetActive(false);
            UAVcamera_task1.SetActive(true);
            Debug.Log("[CameraSwitch] → camera1 に戻す (area != 2)");
        }
    }
}
