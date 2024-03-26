using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FortLevel : MonoBehaviour
{
    private int hea_level = 0;
    private int dam_level = 0;
    private int mor_level = 0;
    private SaveData saveData;
    [SerializeField] FortTurretControl fortTurretControl;
    [SerializeField] HealthController healthControl;
    [SerializeField] GameObject mortarPrefab;
    public void InitFromKey(string key)
    {
        this.key = key;
        saveData = SaveData.Instance;
        saveData.SetFortCenter(key, centralLocation);
        LevelsUpdate();
    }
    public void LevelsUpdate()
    {
        FortSaveLevel fSL = saveData.GetFortLevels(key);
        if (fSL != null)                                     //fort has been set before, load from these levels
        {
            hea_level = fSL.hea_level;
            dam_level = fSL.dam_level;
            mor_level = fSL.mor_level;
            healthControl.SetMaxHealth(hea_level_effect[hea_level]);
            fortTurretControl.SetDamage(dam_level_effect[dam_level]);
        }
        else
        {                                                   //new fort, handle accordingly
            hea_level = Random.Range(0, hea_level_effect.Count);
            dam_level = Random.Range(0, dam_level_effect.Count);
            mor_level = Mathf.Max(Random.Range(0, mor_level_effect.Count), 1);
            fSL = new FortSaveLevel(hea_level, dam_level, mor_level);
            saveData.SetFortLevels(key, fSL);
        }
        LoadedObjects loadedObjects = LoadedObjects.Instance;
        List<Vector3> existantMembers = new List<Vector3>() { new Vector3(transform.position.x, 0, transform.position.z) };
        List<string> mortarsToInit = new();
        for(int i = 0; i <= fSL.mor_level; i++)
        {
            for(int j = 0; j < mor_level_effect[i]; j++)
            {
                string mortarKey = key + "_mortar_" + i + "_" + j;
                GameObject mortar = loadedObjects.GetLoadedMortar(mortarKey);
                if (mortar == null)                                             //we only have to do things if the mortar is not loaded in
                {
                    if (saveData.MortarPosExists(mortarKey))                    //mortar previously initialized, respawn at same location
                    {
                        Vector3 position = saveData.GetMortarPos(mortarKey);
                        MortarController mC = SpawnMortar(position, mortarKey);
                        //MortarController mC = SpawnInitMortar(position, mortarKey);
                        if (loadedObjects.GetLoadedFort(key) != null)
                            loadedObjects.GetLoadedFort(key).GetComponent<LocalTeamController>().AddChildMortar(mC);
                    }
                    else
                    {
                        mortarsToInit.Add(mortarKey);
                    }
                }
                else
                {
                    existantMembers.Add(mortar.transform.position);
                }
            }
        }
        for(int i = 0; i < mortarsToInit.Count; i++)          //spawn in these new mortars
        {
            float checkPointsPerTSD = 20;
            float angleIncriment = 360 / checkPointsPerTSD;
            float currCheckAngle = 0;
            RaycastHit[] hits;
            //List<Vector3> mortPosi = new();
            Vector3 furthestPos = new Vector3(0, -100, 0);
            float furthestDst = -1;
            while (currCheckAngle < 360)                                    //find possible location around central location if front of fort
            {
                float xAdd = Mathf.Sin(currCheckAngle * Mathf.Rad2Deg);
                float zAdd = Mathf.Cos(currCheckAngle * Mathf.Rad2Deg);
                Vector3 dir = new Vector3(xAdd, 0, zAdd);
                Vector3 mortPosi = new Vector3(0, -100, 0);
                hits = Physics.RaycastAll(centralLocation, dir, 40);
                foreach (RaycastHit hit in hits)
                    if (hit.collider.CompareTag("Land"))
                        mortPosi = hit.point;
                if(mortPosi.y != -100)
                {
                    float furthestDsetFromOthers = -1;
                    for(int j = 0; j < existantMembers.Count; j++)
                    {
                        float tempDst = Vector3.Distance(existantMembers[j], mortPosi);
                        if ((tempDst < furthestDsetFromOthers) || (furthestDsetFromOthers == -1))
                            furthestDsetFromOthers = tempDst;
                    }
                    if((furthestDsetFromOthers != -1) && (furthestDsetFromOthers > furthestDst))
                    {
                        furthestDst = furthestDsetFromOthers;
                        furthestPos = mortPosi;
                    }
                }
                currCheckAngle += angleIncriment;
            }
            if (furthestDst != -1)
            {
                MortarController mC = SpawnInitMortar(furthestPos, mortarsToInit[i]);
                if (loadedObjects.GetLoadedFort(key) != null)
                    loadedObjects.GetLoadedFort(key).GetComponent<LocalTeamController>().AddChildMortar(mC);
                existantMembers.Add(furthestPos);
            }
        }
    }
    public MortarController SpawnMortar(Vector3 pos, string key)
    {
        pos.y = 0.65f;
        GameObject mo = GameObject.Instantiate(mortarPrefab);
        mo.GetComponent<HealthController>().SetMaxHealth(50);
        mo.name = key;
        mo.transform.parent = transform;
        mo.transform.position = pos;
        LoadedObjects.Instance.AddLoadedMortar(key, mo);
        return mo.GetComponent<MortarController>();
    }
    public MortarController SpawnInitMortar(Vector3 pos, string key)
    {
        pos.y = 0.65f;
        GameObject mo = GameObject.Instantiate(mortarPrefab);
        mo.GetComponent<HealthController>().SetMaxHealth(50);
        mo.name = key;
        mo.transform.parent = transform;
        mo.transform.position = pos;
        saveData.SetMortarPos(key, pos);
        LoadedObjects.Instance.AddLoadedMortar(key, mo);
        return mo.GetComponent<MortarController>();
    }
    Vector3 centralLocation = new Vector3(0, -100, 0);
    public void SetCentralLocation(Vector3 location)
    {
        centralLocation = location;
    }
    private Vector3 GetNewMortarPos()
    {
        return Vector3.zero;
    }
    private string key;
    public string GetKey()
    {
        return key;
    }
    public static Dictionary<int, float> dam_level_effect = new Dictionary<int, float>()
    {
        { 0, 20 },
        { 1, 25 },
        { 2, 35 },
        { 3, 50 },
    };
    public static Dictionary<int, float> hea_level_effect = new Dictionary<int, float>()
    {
        { 0, 150 },
        { 1, 200 },
        { 2, 250 },
        { 3, 400 },
    };
    public static Dictionary<int, float> mor_level_effect = new Dictionary<int, float>()
    {
        { 0, 0 },
        { 1, 1 },
        { 2, 1 },
        { 3, 1 },
    };
    public static Dictionary<int, float> dam_cost_to_next_level = new Dictionary<int, float>()
    {
        { 0, 50 },
        { 1, 150 },
        { 2, 250 }
    };
    public static Dictionary<int, float> hea_cost_to_next_level = new Dictionary<int, float>()
    {
        { 0, 50 },
        { 1, 100 },
        { 2, 200 },
    };
    public static Dictionary<int, float> mor_cost_to_next_level = new Dictionary<int, float>()
    {
        { 0, 100 },
        { 1, 125 },
        { 2, 150 },
    };
}
[System.Serializable]
public class FortSaveLevel
{
    public int hea_level;
    public int dam_level;
    public int mor_level;
    public FortSaveLevel(int hea_level, int dam_level, int mor_level)
    {
        this.hea_level = hea_level;
        this.dam_level = dam_level;
        this.mor_level = mor_level;
    }
}
