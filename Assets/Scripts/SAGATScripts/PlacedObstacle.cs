using UnityEngine;
using UnityEngine.EventSystems;

public class PlacedObstacle : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData e)
    {
        if (e.button == PointerEventData.InputButton.Right)
        {
            //Debug.Log("Right click delete " + gameObject.name);
            Destroy(gameObject);
        }
    }
}
