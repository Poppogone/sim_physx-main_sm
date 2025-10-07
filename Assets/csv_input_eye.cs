using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;
using UnityEngine.UI;

public class csv_input_eye : MonoBehaviour
{
    [Header("Logging Control")]
    public bool enabledLogging = true;   // ← デフォルトで有効に（必要ならInspectorでOFF）

    // ===== 追記：csv_zx120_position と同じ制御用フィールド =====
    private StreamWriter sw;
    private string path;
    private bool isLogging = false;
    private float startLocal;        // セッション開始（再開）時刻
    private float elapsedBefore = 0; // 累積経過時間（停止までの合計）

    // ===== 既存 =====
    private float timeNow;
    private float timeStart; // ※互換のため残すが経過時間は上記で計算

    [SerializeField] private Transform base_link;

    private float kenki_x;
    private float kenki_y;
    private float kenki_z;
    private float kenki_ry;

    [SerializeField] private ArticulationBody swing_joint;
    [SerializeField] private ArticulationBody boom_joint;
    [SerializeField] private ArticulationBody arm_joint;
    [SerializeField] private ArticulationBody bucket_joint;

    private float swing_deg;
    private float boom_deg;
    private float arm_deg;
    private float bucket_deg;

    [SerializeField] private GameObject drone;
    string states_camera;
    Vector3 headP;
    Vector3 headA;
    Vector2 filteredPos;

    [SerializeField] private RectTransform e;

    void Start()
    {
        timeStart = Time.realtimeSinceStartup; // 互換用（未使用でも残す）
    }

    void OnDestroy()
    {
        Close();
    }

    void Update()
    {
        // 1) ログ無効 or 未開始 なら何もしない
        if (!enabledLogging || !isLogging) return;

        // 2) SAGAT中は自動停止（必要ならここで Resume/ Pause を切り替え）
        if (TestDataManager.I != null && TestDataManager.I.pausedForSAGAT)
        {
            // 既に停止中でなければ停止
            PauseLogging();
            return;
        }
        else
        {
            // 停止中（= isLogging==false）で、パスがあるなら再開
            if (sw == null && !string.IsNullOrEmpty(path))
            {
                ResumeLogging();
                // 再開直後は1フレームスキップしても良いが、そのまま続行でもOK
            }
        }

        // 3) 実際の記録（元の流れは変更しない）
        GazePoint gazePoint = TobiiAPI.GetGazePoint();

        // 経過時間は停止/再開に対応した値に変更
        float t = elapsedBefore + (Time.realtimeSinceStartup - startLocal);
        timeNow = t; // 互換用

        kenki_x = base_link.position.x;
        kenki_y = base_link.position.y;
        kenki_z = base_link.position.z;
        kenki_ry = base_link.rotation.y;

        swing_deg  = swing_joint.jointPosition[0];
        boom_deg   = boom_joint.jointPosition[0];
        arm_deg    = arm_joint.jointPosition[0];
        bucket_deg = bucket_joint.jointPosition[0];

        if (drone != null)
        {
            if (drone.GetComponent<headFollowing>()?.enabled == true)
            {
                var script = drone.GetComponent<headFollowing>();
                states_camera = script.IsFree.ToString();
                headP = script.headPos;
                headA = script.headAng;
            }
            else if (drone.GetComponent<headJoystick>()?.enabled == true)
            {
                var script = drone.GetComponent<headJoystick>();
                states_camera = script.IsFree.ToString();
                headP = script.headPos;
                headA = script.headAng;
            }
            else if (drone.GetComponent<JoyStickController>()?.enabled == true)
            {
                var script = drone.GetComponent<JoyStickController>();
                states_camera = script.IsFree.ToString();
                headP = script.headPos;
                headA = script.headAng;
            }
        }

        if (gazePoint.IsRecent())
        {
            Vector2 gazePos = gazePoint.Screen;    // マウス座標風
            filteredPos = Vector2.Lerp(filteredPos, gazePos, 0.5f);
            if (e != null) e.position = new Vector3(filteredPos.x, filteredPos.y, 0);

            string[] s1 = {
                timeNow.ToString(),
                kenki_x.ToString(), kenki_y.ToString(), kenki_z.ToString(), kenki_ry.ToString(),
                swing_deg.ToString(), boom_deg.ToString(), arm_deg.ToString(), bucket_deg.ToString(),
                headP.x.ToString(), headP.y.ToString(), headP.z.ToString(),
                headA.x.ToString(), headA.y.ToString(), headA.z.ToString(),
                states_camera,
                "1",
                gazePos.x.ToString(), gazePos.y.ToString(),
                filteredPos.x.ToString(), filteredPos.y.ToString()
            };
            sw.WriteLine(string.Join(",", s1));
        }
        else
        {
            string[] s1 = {
                timeNow.ToString(),
                kenki_x.ToString(), kenki_y.ToString(), kenki_z.ToString(), kenki_ry.ToString(),
                swing_deg.ToString(), boom_deg.ToString(), arm_deg.ToString(), bucket_deg.ToString(),
                headP.x.ToString(), headP.y.ToString(), headP.z.ToString(),
                headA.x.ToString(), headA.y.ToString(), headA.z.ToString(),
                states_camera,
                "0", "NN", "NN", "NN", "NN"
            };
            sw.WriteLine(string.Join(",", s1));
        }
    }

    // ====== ファイル作成（StartCheck → start_check.cs から呼ばれる）======
    public void MakeFile(Text filenameUI)
    {
        if (filenameUI == null || string.IsNullOrWhiteSpace(filenameUI.text)) return;
        MakeFile(filenameUI.text.Trim());
    }

    public void MakeFile(string baseName)
    {
        if (!enabledLogging || isLogging) return;

        string dir = Path.Combine(Application.dataPath, "ExperimentData_2025");
        Directory.CreateDirectory(dir);
        path = Path.Combine(dir, $"{baseName}_inputeye.csv");

        bool exists = File.Exists(path);
        sw = new StreamWriter(path, append: true, new UTF8Encoding(false)) { AutoFlush = true };
        if (!exists)
        {
            sw.WriteLine(string.Join(",", new[]{
                "time","kenki_pos_x","kenki_pos_y","kenki_pos_z","kenki_ori_y",
                "swing","boom","arm","bucket",
                "h_pos_x","h_pos_y","h_pos_z","h_ang_x","h_ang_y","h_ang_z",
                "is_free","is_recent","pos_x","pos_y","filtered_pos_x","filtered_pos_y"
            }));
        }

        // ログ開始
        startLocal = Time.realtimeSinceStartup;
        elapsedBefore = 0f;
        isLogging = true;
        Debug.Log("[csv_input_eye] Start logging: " + path);
    }

    // ====== 一時停止／再開／クローズ ======
    public void PauseLogging()
    {
        if (!isLogging) return;
        elapsedBefore += (Time.realtimeSinceStartup - startLocal);
        sw.Flush();
        sw.Close();
        sw = null;
        isLogging = false;
        Debug.Log($"[csv_input_eye] Pause at {elapsedBefore:F3}s");
    }

    public void ResumeLogging()
    {
        if (isLogging) return;
        if (string.IsNullOrEmpty(path)) return;

        sw = new StreamWriter(path, append: true, new UTF8Encoding(false)) { AutoFlush = true };
        startLocal = Time.realtimeSinceStartup;
        isLogging = true;
        Debug.Log($"[csv_input_eye] Resume logging from {elapsedBefore:F3}s");
    }

    public void Close()
    {
        if (!isLogging) return;
        sw.Flush();
        sw.Close();
        sw = null;
        isLogging = false;
    }
}
