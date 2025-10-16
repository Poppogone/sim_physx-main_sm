using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SAGATImageExporter : MonoBehaviour
{
    [Header("描画先（SAGATの回答パネルなど）")]
    public RectTransform workspace;

    [Header("CSVファイル（TextAssetとしてアタッチ）")]
    public TextAsset sagatCorrectCsv;    // 白丸
    public TextAsset attachedCsv;        // 赤丸

    [Header("丸の見た目")]
    public Sprite dotSprite;
    public float dotSize = 20f;
    public Color colorWhite = Color.white;
    public Color colorRed   = Color.red;

    [Header("キャプチャ保存設定")]
    public bool cropWorkspaceBottomRight = true; // true=workspace右下を基準に切り出し
    public int cropWidth  = 512;
    public int cropHeight = 512;
    public bool debugSaveFullScreen = false;     // ← 切り分け用：trueなら画面全体を保存

    [Header("保存先（Assets直下に作成）")]
    public string outputFolderName = "SAGATData_2025";
    public string fileBaseName     = "SAGAT_test";
    public bool   addTimestamp     = true;

    [Header("ボタンの押した感 (任意)")]
    public Button exportButton;
    public Color pressedColor = new Color(0.8f, 0.8f, 0.8f);
    public float pressedFlashSeconds = 0.12f;

    [Header("自動バインド（忘れ対策）")]
    public bool autoBindOnClick = true;

    private readonly List<GameObject> _spawnedDots = new List<GameObject>();
    private Canvas _rootCanvas;

    void Awake()
    {
        // ← 親Canvasを workspace 基準で取得（ズレ対策）
        _rootCanvas = (workspace != null)
            ? workspace.GetComponentInParent<Canvas>()
            : GetComponentInParent<Canvas>();

        if (workspace == null)
            Debug.LogError("[SAGATImageExporter] workspace 未設定です。RectTransform を割り当ててください。");
        if (_rootCanvas == null)
            Debug.LogWarning("[SAGATImageExporter] 親に Canvas が見つかりません（Screen Space Overlay なら問題なし）。");

        // ← OnClick 自動バインド（Inspector未設定でも動く）
        if (autoBindOnClick && exportButton == null)
        {
            exportButton = GetComponent<Button>();
            if (exportButton != null)
            {
                exportButton.onClick.AddListener(() => RunExport());
            }
        }
    }

    public void RunExport() => StartCoroutine(RunExportRoutine());

    private IEnumerator RunExportRoutine()
    {
        // 押した演出
        Image btnImg = null;
        Color origCol = Color.white;
        if (exportButton != null)
        {
            exportButton.interactable = false;
            btnImg = exportButton.GetComponent<Image>();
            if (btnImg != null) { origCol = btnImg.color; btnImg.color = pressedColor; }
        }

        bool success = false;

        if (workspace == null)
        {
            Debug.LogError("[SAGATImageExporter] workspace が未設定のため中断");
        }
        else
        {
            ClearSpawned();

            if (sagatCorrectCsv != null) SpawnDots(ReadPoints(sagatCorrectCsv.text), colorWhite, "WHITE");
            else Debug.LogWarning("[SAGATImageExporter] sagatCorrectCsv 未設定（白丸なし）");

            if (attachedCsv != null) SpawnDots(ReadPoints(attachedCsv.text), colorRed, "RED");
            else Debug.LogWarning("[SAGATImageExporter] attachedCsv 未設定（赤丸なし）");

            yield return null;
            yield return new WaitForEndOfFrame();

            string path = SaveCroppedImage();
            if (!string.IsNullOrEmpty(path))
            {
                Debug.Log($"[SAGATImageExporter] ✅ Saved: {path}");
                success = true;
            }
            else
            {
                Debug.LogError("[SAGATImageExporter] ❌ 保存に失敗しました。");
            }
        }

        if (exportButton != null)
        {
            yield return new WaitForSecondsRealtime(pressedFlashSeconds);
            if (btnImg != null) btnImg.color = origCol;
            exportButton.interactable = true;
        }

        if (!success)
        {
            Debug.LogWarning("[SAGATImageExporter] 処理は完了しましたが画像が生成されませんでした。");
        }
    }

    // --- CSV ---
    private List<Vector2> ReadPoints(string csvText)
    {
        var list = new List<Vector2>();
        if (string.IsNullOrEmpty(csvText)) return list;

        using (var reader = new System.IO.StringReader(csvText))
        {
            string header = reader.ReadLine();
            if (header == null) return list;

            var cols = SplitCsvLine(header);
            int idxX = -1, idxY = -1;
            for (int i = 0; i < cols.Length; i++)
            {
                var h = cols[i].Trim().Trim('\"');
                if (string.Equals(h, "X", StringComparison.OrdinalIgnoreCase)) idxX = i;
                if (string.Equals(h, "Y", StringComparison.OrdinalIgnoreCase)) idxY = i;
            }
            if (idxX < 0 || idxY < 0) { idxX = Mathf.Min(1, cols.Length - 1); idxY = Mathf.Min(2, cols.Length - 1); }

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = SplitCsvLine(line);
                if (parts.Length <= Mathf.Max(idxX, idxY)) continue;
                if (TryParseFloat(parts[idxX], out float x) && TryParseFloat(parts[idxY], out float y))
                    list.Add(new Vector2(x, y));
            }
        }
        return list;
    }

    private string[] SplitCsvLine(string line)
    {
        var list = new List<string>(); var sb = new StringBuilder(); bool inQ = false;
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '\"') { if (inQ && i + 1 < line.Length && line[i + 1] == '\"') { sb.Append('\"'); i++; } else inQ = !inQ; }
            else if (c == ',' && !inQ) { list.Add(sb.ToString()); sb.Length = 0; }
            else sb.Append(c);
        }
        list.Add(sb.ToString()); return list.ToArray();
    }

    private bool TryParseFloat(string s, out float v)
    {
        s = s.Trim().Trim('\"');
        return float.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out v)
            || float.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out v);
    }

    // --- 丸描画 ---
    private void SpawnDots(List<Vector2> points, Color color, string tagPrefix)
    {
        foreach (var p in points)
        {
            var go = new GameObject($"{tagPrefix}_Dot", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(workspace, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(dotSize, dotSize);
            rt.anchoredPosition = p;
            var img = go.GetComponent<Image>(); img.color = color; if (dotSprite) img.sprite = dotSprite;
            _spawnedDots.Add(go);
        }
    }
    private void ClearSpawned() { foreach (var go in _spawnedDots) if (go) Destroy(go); _spawnedDots.Clear(); }

    // --- 保存 ---
    private string SaveCroppedImage()
    {
        try
        {
            var full = ScreenCapture.CaptureScreenshotAsTexture();
            if (full == null) { Debug.LogError("[SAGATImageExporter] ScreenCapture が null"); return null; }

            Rect crop;
            if (debugSaveFullScreen)
            {
                crop = new Rect(0, 0, full.width, full.height);
            }
            else if (cropWorkspaceBottomRight && workspace != null)
            {
                var corners = new Vector3[4];
                workspace.GetWorldCorners(corners); // 0:BL 1:TL 2:TR 3:BR
                var cam = (_rootCanvas != null) ? _rootCanvas.worldCamera : null;
                Vector3 br = RectTransformUtility.WorldToScreenPoint(cam, corners[3]);

                int x = Mathf.RoundToInt(br.x - cropWidth);
                int y = Mathf.RoundToInt(br.y);
                x = Mathf.Clamp(x, 0, full.width  - 1);
                y = Mathf.Clamp(y, 0, full.height - 1);
                int w = Mathf.Clamp(cropWidth,  1, full.width  - x);
                int h = Mathf.Clamp(cropHeight, 1, full.height - y);
                crop = new Rect(x, y, w, h);
            }
            else
            {
                int x = Mathf.Max(0, Screen.width - cropWidth);
                int y = 0;
                int w = Mathf.Clamp(cropWidth,  1, Screen.width  - x);
                int h = Mathf.Clamp(cropHeight, 1, Screen.height - y);
                crop = new Rect(x, y, w, h);
            }

            var cut = new Texture2D((int)crop.width, (int)crop.height, TextureFormat.RGBA32, false);
            cut.ReadPixels(crop, 0, 0);
            cut.Apply();

            string dir = Path.Combine(Application.dataPath, outputFolderName);
            Directory.CreateDirectory(dir);
            string name = string.IsNullOrEmpty(fileBaseName) ? "export" : fileBaseName;
            if (addTimestamp) name += "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string path = Path.Combine(dir, name + ".png");

            File.WriteAllBytes(path, cut.EncodeToPNG());

            Destroy(full); Destroy(cut);
            return path;
        }
        catch (Exception ex)
        {
            Debug.LogError("[SAGATImageExporter] 例外: " + ex);
            return null;
        }
    }
}
