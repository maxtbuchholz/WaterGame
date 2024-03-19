using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Serialize
{
    public static float[] V3(Vector3 v3)
    {
        return new float[] { v3.x, v3.y, v3.z };
    }
    public static float[] Q (Quaternion q)
    {
        return new float[] { q.w, q.x, q.y, q.z };
    }
    public static Dictionary<float[], string> V2_S(Dictionary<Vector2, string> v2_s)
    {
        Dictionary<float[], string> V2_S = new();
        foreach (KeyValuePair<Vector2, string> pair in v2_s)
            V2_S.Add(new float[] { pair.Key.x, pair.Key.y }, pair.Value);
        return V2_S;
    }
    public static Dictionary<string, float[]> S_V3(Dictionary<string, Vector3> s_v3)
    {
        Dictionary<string, float[]> S_V3 = new();
        foreach (KeyValuePair<string, Vector3> pair in s_v3)
            S_V3.Add(pair.Key, new float[] { pair.Value.x, pair.Value.y, pair.Value.z });
        return S_V3;
    }
    public static Dictionary<int, float[]> I_V3(Dictionary<int, Vector3> i_v3)
    {
        Dictionary<int, float[]> I_V3 = new();
        foreach (KeyValuePair<int, Vector3> pair in i_v3)
            I_V3.Add(pair.Key, new float[] { pair.Value.x, pair.Value.y, pair.Value.z });
        return I_V3;
    }
    public static HashSet<float[]> HV2(HashSet<Vector2> hv2)
    {
        HashSet<float[]> HV2 = new();
        foreach (Vector2 v2 in hv2)
            HV2.Add(new float[] { v2.x, v2.y });
        return HV2;
    }
}
public static class Deserialize
{
    public static Vector3 V3(float[] v3)
    {
        return new Vector3(v3[0], v3[1], v3[2]);
    }
    public static Quaternion Q(float[] q)
    {
        return new Quaternion(q[0], q[1], q[2], q[3]);
    }
    public static Dictionary<Vector2, string> V2_S(Dictionary<float[], string> v2_s)
    {
        Dictionary<Vector2, string> V2_S = new();
        foreach (KeyValuePair<float[], string> pair in v2_s)
            V2_S.Add(new Vector2(pair.Key[0], pair.Key[1]), pair.Value);
        return V2_S;
    }
    public static Dictionary<string, Vector3> S_V3(Dictionary<string, float[]> s_v3)
    {
        Dictionary<string, Vector3> S_V3 = new();
        foreach (KeyValuePair<string, float[]> pair in s_v3)
            S_V3.Add(pair.Key, new Vector3(pair.Value[0], pair.Value[1], pair.Value[2]));
        return S_V3;
    }
    public static Dictionary<int, Vector3> I_V3(Dictionary<int, float[]> i_v3)
    {
        Dictionary<int, Vector3> I_V3 = new();
        foreach (KeyValuePair<int, float[]> pair in i_v3)
            I_V3.Add(pair.Key, new Vector3(pair.Value[0], pair.Value[1], pair.Value[2]));
        return I_V3;
    }
    public static HashSet<Vector2> HV2(HashSet<float[]> hv2)
    {
        HashSet<Vector2> HV2 = new();
        foreach (float[] v2 in hv2)
            HV2.Add(new Vector2 (v2[0], v2[1] ));
        return HV2;
    }
}
