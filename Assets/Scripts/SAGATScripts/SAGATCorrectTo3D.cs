using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SAGATCorrectTo3D : MonoBehaviour
{
    public Button exportButton;              // Exportボタン
    public RectTransform workspace;          // Panel_Answer
    public Camera sagatCamera;               // 上から見たカメラ
    public Transform[] correctObjects;       // Terrain上に置いた正解オブジェクト群

    void Start()
    {
        exportButton.onClick.AddListener(ExportCorrectData);
    }

    void ExportCorrectData()
    {
        string folderPath = Path.Combine(Application.dataPath, "SAGATData_2025");
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        string path = Path.Combine(folderPath, "SAGAT_Correct.csv");

        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine("Name,X,Y");

            foreach (Transform obj in correctObjects)
            {
                if (obj == null) continue;

                // ワールド座標 → スクリーン座標
                Vector3 screenPos = sagatCamera.WorldToScreenPoint(obj.position);

                // スクリーン座標 → Panel_Answerのローカル座標
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(workspace, screenPos, null, out Vector2 uiPos))
                {
                    writer.WriteLine($"{obj.name},{uiPos.x},{uiPos.y}");
                }
            }
        }

        Debug.Log("Exported 3D correct data to: " + path);
    }
}
