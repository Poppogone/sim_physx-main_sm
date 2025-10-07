using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggablePaletteItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("必須")]
    public RectTransform workspace;          // Panel_Answer
    public Sprite obstacleSprite;            // 置く見た目（未設定なら自分のImageを使用）
    public Vector2 placedSize = new Vector2(64, 64);

    [Header("正解と対応させる名前（固定名）")]
    public string logicalName = "Correct_Corn";  // ← Inspectorで設定（例：Correct_Corn, Correct_Box）

    private RectTransform draggingObj;
    private Canvas rootCanvas;

    void Awake()
    {
        rootCanvas = GetComponentInParent<Canvas>();
        if (obstacleSprite == null)
        {
            var img = GetComponent<Image>();
            if (img) obstacleSprite = img.sprite;
        }
    }

    public void OnBeginDrag(PointerEventData e)
    {
        // オブジェクトを新規作成（番号なし）
        var go = new GameObject(logicalName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(PlacedObstacle));
        go.transform.SetParent(workspace, false);

        var img = go.GetComponent<Image>();
        img.sprite = obstacleSprite;
        img.raycastTarget = true;

        draggingObj = go.GetComponent<RectTransform>();
        draggingObj.sizeDelta = placedSize;

        // ドラッグ開始位置を反映
        RectTransformUtility.ScreenPointToLocalPointInRectangle(workspace, e.position, e.pressEventCamera, out var local);
        draggingObj.anchoredPosition = local;
    }

    public void OnDrag(PointerEventData e)
    {
        if (!draggingObj) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(workspace, e.position, e.pressEventCamera, out var local);
        draggingObj.anchoredPosition = local;
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (!draggingObj) return;

        // ワークスペース外にドロップされたら削除
        if (!RectTransformUtility.RectangleContainsScreenPoint(workspace, e.position, e.pressEventCamera))
            Destroy(draggingObj.gameObject);

        draggingObj = null;
    }
}
