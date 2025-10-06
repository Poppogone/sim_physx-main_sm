using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class zx120StateSaver : MonoBehaviour
{
    [Header("zx120 のルート（自分自身でもOK）")]
    public Transform zx120Root;
    [Header("角度（localRotation）を保存したいパーツ群")]
    public List<Transform> partTransforms = new List<Transform>();
    [Header("Rigidbody の速度も保存・復元")]
    public bool includeRigidbodyVelocity = true;

    [System.Serializable]
    public class Snapshot {
        public Vector3 pos; public Quaternion rot;
        public Vector3 vel; public Vector3 angVel;
        public Dictionary<string, Quaternion> localRots = new Dictionary<string, Quaternion>();
    }
    private Snapshot snap;

    //void Reset(){ zx120Root = transform; }

    public void SaveNow()
    {
        if (!zx120Root) return;
        var s = new Snapshot{
            pos = zx120Root.position,
            rot = zx120Root.rotation
        };
        var rb = includeRigidbodyVelocity ? zx120Root.GetComponent<Rigidbody>() : null;
        s.vel = rb ? rb.velocity : Vector3.zero;
        s.angVel = rb ? rb.angularVelocity : Vector3.zero;

        foreach (var t in partTransforms)
        {
            if (!t) continue;
            s.localRots[GetRelPath(zx120Root, t)] = t.localRotation;
        }
        snap = s;

        if (TestDataManager.I != null)
        {
            var t = new TestDataManager.TransformSnapshot();
            t.worldPosition = s.pos;
            t.worldRotation = s.rot;
            t.rbVelocity = s.vel;
            t.rbAngularVelocity = s.angVel;
            t.partLocalRotations = s.localRots;
            TestDataManager.I.zx120Snapshot = t;
        }
    }

    public void RestoreNow()
    {
        // TestDataManager から復元
        var mgr = TestDataManager.I;
        if (!zx120Root || mgr == null || mgr.zx120Snapshot == null) return;
        var snap = mgr.zx120Snapshot;

        var rb = zx120Root.GetComponent<Rigidbody>();
        if (rb) { rb.velocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }

        zx120Root.SetPositionAndRotation(snap.worldPosition, snap.worldRotation);

        foreach (var kv in snap.partLocalRotations)
        {
            var t = FindByRelPath(zx120Root, kv.Key);
            if (t) t.localRotation = kv.Value;
        }

        if (rb && includeRigidbodyVelocity)
        {
            rb.velocity = snap.rbVelocity;
            rb.angularVelocity = snap.rbAngularVelocity;
        }
    }

    static string GetRelPath(Transform root, Transform child){
        var stack = new Stack<string>(); var t = child;
        while (t && t != root){ stack.Push(t.name); t = t.parent; }
        if (t != root) return null; return string.Join("/", stack.ToArray());
    }
    static Transform FindByRelPath(Transform root, string path){
        if (string.IsNullOrEmpty(path)) return null;
        var cur = root; foreach (var p in path.Split('/')){ cur = cur.Find(p); if (!cur) return null; }
        return cur;
    }
}
