using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SAGATCorrectExporter : MonoBehaviour
{
    public Button exportButton;           // InspectorでExportボタンを割り当て
    public RectTransform workspace;       // Panel_Answer を割り当て
    public string outputFile = "Assets/SAGATData_2025/SAGAT_Correct.csv";

    void Start()
    {
        exportButton.onClick.AddListener(ExportCorrectData);
    }

    void ExportCorrectData()
    {
        string folderPath = Path.Combine(Application.dataPath, "SAGATData_2025");
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        string path = Path.Combine(Application.dataPath, "SAGATData_2025", "SAGAT_Correct.csv");

        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine("Name,X,Y");

            foreach (Transform child in workspace)
            {
                var rt = child.GetComponent<RectTransform>();
                if (rt != null)
                {
                    Vector2 pos = rt.anchoredPosition;
                    writer.WriteLine($"{child.name},{pos.x},{pos.y}");
                }
            }
        }

        Debug.Log("Exported correct data to: " + path);
    }
}
