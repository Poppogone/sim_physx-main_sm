using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SAGATSaver : MonoBehaviour
{
    public Button saveButton;              // InspectorでSaveボタンを割り当て
    public RectTransform workspace;        // Panel_Answer を割り当て
    public string correctFile = "Assets/SAGATData_2025/SAGAT_Correct.csv";

    void Start()
    {
        saveButton.onClick.AddListener(SaveAndEvaluate);
    }

    void SaveAndEvaluate()
    {
        // 保存先フォルダ（Assets/SAGATData_2025）
        string folderPath = Path.Combine(Application.dataPath, "SAGATData_2025");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log("Created folder: " + folderPath);
        }

        // ファイル名（日時付き）
        string fileName = "SAGAT_" + DateTime.Now.ToString("MMdd_HHmm") + ".csv";
        string path = Path.Combine(folderPath, fileName);

        // --- 回答データの収集 ---
        List<ObstacleData> answerData = new List<ObstacleData>();
        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine("Name,X,Y");

            foreach (Transform child in workspace)
            {
                var rt = child.GetComponent<RectTransform>();
                if (rt != null)
                {
                    Vector2 pos = rt.anchoredPosition;
                    answerData.Add(new ObstacleData(child.name, pos));
                    writer.WriteLine($"{child.name},{pos.x},{pos.y}");
                }
            }
        }

        // --- 正解データをロード ---
        var correctData = LoadCsv(correctFile);

        // --- 評価処理 ---
        float totalDist = 0f;
        int matchCount = 0;
        int falseNegativeCount = 0;
        int falsePositiveCount = 0;

        HashSet<int> usedAnswer = new HashSet<int>();
        List<string> evalLines = new List<string>();
        evalLines.Add("");
        evalLines.Add("Result,Name,Distance");

        foreach (var correct in correctData)
        {
            float minDist = float.MaxValue;
            int minIdx = -1;

            for (int i = 0; i < answerData.Count; i++)
            {
                if (usedAnswer.Contains(i)) continue;
                if (answerData[i].name != correct.name) continue;

                float dist = Vector2.Distance(answerData[i].pos, correct.pos);
                if (dist < minDist)
                {
                    minDist = dist;
                    minIdx = i;
                }
            }

            if (minIdx >= 0)
            {
                usedAnswer.Add(minIdx);
                totalDist += minDist;
                matchCount++;
                evalLines.Add($"Match,{correct.name},{minDist:F2}");
            }
            else
            {
                falseNegativeCount++;
                evalLines.Add($"FalseNegative,{correct.name},N/A");
            }
        }

        // 未使用回答 = False Positive
        for (int i = 0; i < answerData.Count; i++)
        {
            if (!usedAnswer.Contains(i))
            {
                falsePositiveCount++;
                evalLines.Add($"FalsePositive,{answerData[i].name},N/A");
            }
        }

        // まとめ
        if (matchCount > 0)
        {
            float ave = totalDist / matchCount;
            evalLines.Add($"AverageDistance,,{ave:F2}");
        }
        else
        {
            evalLines.Add("AverageDistance,,N/A");
        }

        evalLines.Add($"FalseNegativeCount,,{falseNegativeCount}");
        evalLines.Add($"FalsePositiveCount,,{falsePositiveCount}");

        // --- 評価結果を追記 ---
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            foreach (var line in evalLines)
                writer.WriteLine(line);
        }

        Debug.Log("Saved with evaluation: " + path);
    }

    List<ObstacleData> LoadCsv(string path)
    {
        var list = new List<ObstacleData>();
        if (!File.Exists(path)) return list;

        using (var reader = new StreamReader(path))
        {
            bool firstLine = true;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (firstLine) { firstLine = false; continue; }

                var cols = line.Split(',');
                if (cols.Length < 3) continue;

                string name = cols[0];
                if (float.TryParse(cols[1], out float x) &&
                    float.TryParse(cols[2], out float y))
                {
                    list.Add(new ObstacleData(name, new Vector2(x, y)));
                }
            }
        }
        return list;
    }

    struct ObstacleData
    {
        public string name;
        public Vector2 pos;
        public ObstacleData(string n, Vector2 p)
        {
            name = n;
            pos = p;
        }
    }
}
