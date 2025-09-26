using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SAGATScore : MonoBehaviour
{
    [Header("ファイル設定")]
    public string correctFile = "Assets/SAGATData_2025/SAGAT_Correct.csv";
    public string answerFile = ""; // 最新の回答CSVを指定

    void Start()
    {
        CompareObstacles(correctFile, answerFile);
    }

    void CompareObstacles(string correctPath, string answerPath)
    {
        if (!File.Exists(correctPath) || !File.Exists(answerPath))
        {
            Debug.LogError("CSVファイルが見つかりません");
            return;
        }

        var correctData = LoadCsv(correctPath);
        var answerData = LoadCsv(answerPath);

        float totalDist = 0f;
        int matchCount = 0;

        // マッチング済み回答を除外するためのフラグ
        HashSet<int> usedAnswer = new HashSet<int>();

        foreach (var correct in correctData)
        {
            float minDist = float.MaxValue;
            int minIdx = -1;

            for (int i = 0; i < answerData.Count; i++)
            {
                if (usedAnswer.Contains(i)) continue;

                if (answerData[i].name != correct.name) continue; // 種類一致のみ

                float dist = Vector2.Distance(answerData[i].pos, correct.pos);
                if (dist < minDist)
                {
                    minDist = dist;
                    minIdx = i;
                }
            }

            if (minIdx >= 0)
            {
                // 最も近い回答に割り当て
                usedAnswer.Add(minIdx);
                totalDist += minDist;
                matchCount++;
                Debug.Log($"Match {correct.name}: 距離={minDist:F2}");
            }
            else
            {
                Debug.LogWarning($"False Negative (置き忘れ): {correct.name} at {correct.pos}");
            }
        }

        // False Positive: 未使用の回答
        for (int i = 0; i < answerData.Count; i++)
        {
            if (!usedAnswer.Contains(i))
            {
                Debug.LogWarning($"False Positive (余計): {answerData[i].name} at {answerData[i].pos}");
            }
        }

        if (matchCount > 0)
        {
            float ave = totalDist / matchCount;
            Debug.Log($"平均距離={ave:F2}, マッチ数={matchCount}");
        }
        else
        {
            Debug.Log("マッチなし");
        }
    }

    List<ObstacleData> LoadCsv(string path)
    {
        var list = new List<ObstacleData>();
        using (var reader = new StreamReader(path))
        {
            bool firstLine = true;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (firstLine) { firstLine = false; continue; } // ヘッダスキップ

                var cols = line.Split(',');
                if (cols.Length < 3) continue;

                string name = cols[0];
                float x = float.Parse(cols[1]);
                float y = float.Parse(cols[2]);
                list.Add(new ObstacleData(name, new Vector2(x, y)));
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
