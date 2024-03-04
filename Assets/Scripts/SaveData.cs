using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    private static SaveData instance;
    public static SaveData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindAnyObjectByType<SaveData>();
            }
            return instance;
        }
    }
    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        islandKeyIncriment = islandCoordsToKey.Count;
    }
    int islandKeyIncriment = 0;
    private HashSet<Vector2> seenSeaCoords = new();
    public void AddSeaCoords(Vector2 seaCoord)
    {
        if (!seenSeaCoords.Contains(seaCoord)) seenSeaCoords.Add(seaCoord);
    }
    public HashSet<Vector2> GetSeaCoords()
    {
        return seenSeaCoords;
    }
    private Dictionary<Vector2, string> islandCoordsToKey = new();
    public bool IslandExists(Vector2 pos)
    {
        return islandCoordsToKey.ContainsKey(pos);
    }
    public void AddIslandCoords(Vector2 islCoord, string key)
    {
        if (!islandCoordsToKey.ContainsKey(islCoord)) islandCoordsToKey.Add(islCoord, key);
    }
    public int GetNewIslandKey(Vector2 islCoord)
    {
        if (islandCoordsToKey.ContainsKey(islCoord)) return -1;
        islandKeyIncriment++;
        return islandKeyIncriment;
    }
    public string GetIslandKeyFromCoord(Vector2 islCoord)
    {
        if (!islandCoordsToKey.ContainsKey(islCoord)) return "-1";
        return islandCoordsToKey[islCoord];
    }
    public Dictionary<Vector2, string> GetIslandCoords()
    {
        return islandCoordsToKey;
    }
    private Dictionary<string, Vector3> fortKeyToCoords = new();
    public void AddFort(string key, Vector3 pos)
    {
        if (!fortKeyToCoords.ContainsKey(key)) fortKeyToCoords.Add(key, pos);
    }
    public Vector3 GetFortPos(string key)
    {
        if (fortKeyToCoords.ContainsKey(key)) return fortKeyToCoords[key];
        return Vector3.zero;
    }
    public bool FortExists(string key)
    {
        return fortKeyToCoords.ContainsKey(key);
    }
    public string GetFortKeyFromPos(Vector3 pos)
    {
        foreach(KeyValuePair<string, Vector3> pair in fortKeyToCoords)
        {
            if ((pair.Value.x == pos.x) && (pair.Value.z == pos.z)) return pair.Key;
        }
        return "";
    }
    Dictionary<string, int> fortTeam = new();
    public void SetFortTeam(string key, int team)
    {
        if (fortTeam.ContainsKey(key)) fortTeam[key] = team;
        else fortTeam.Add(key, team);
    }
    public int GetFortTeam(string key)
    {
        if (fortTeam.ContainsKey(key)) return fortTeam[key];
        return -1;
    }
    Dictionary<string, FortSaveLevel> fortLevels = new();
    public void SetFortLevels(string key, FortSaveLevel fSL)
    {
        if (fortLevels.ContainsKey(key)) fortLevels[key] = fSL;
        else fortLevels.Add(key, fSL);
    }
    public FortSaveLevel GetFortLevels(string key)
    {
        if (fortLevels.ContainsKey(key)) return fortLevels[key];
        return null;
    }
    Dictionary<string, Vector3> mortarPositions = new();
    public void SetMortarPos(string key, Vector3 pos)
    {
        if (mortarPositions.ContainsKey(key)) mortarPositions[key] = pos;
        else mortarPositions.Add(key, pos);
    }
    public Vector3 GetMortarPos(string key)
    {
        if (mortarPositions.ContainsKey(key)) return mortarPositions[key];
        return Vector3.zero;
    }
    public bool MortarPosExists(string key)
    {
        return mortarPositions.ContainsKey(key);
    }
}