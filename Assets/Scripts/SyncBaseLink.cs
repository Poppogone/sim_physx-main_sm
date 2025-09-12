using UnityEngine;

public class SyncBaseLink : MonoBehaviour
{
    private Transform baseLink;

    void Start()
    {
        baseLink = transform.Find("base_link");
        if (baseLink == null)
        {
            Debug.LogError("base_link が見つかりません。階層を確認してください。");
        }
    }

    void LateUpdate()
    {
        if (baseLink != null)
        {
            baseLink.position = transform.position;
            baseLink.rotation = transform.rotation;
        }
    }
}
