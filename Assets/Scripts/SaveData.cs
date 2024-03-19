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
    private float money = 1500;
    public float GetMoney()
    {
        return money;
    }
    public void AddMoney(float change)
    {
        money += change;
    }
    private int currShipId = 0;
    public int GetCurrentShipId()
    {
        return currShipId;
    }
    public void SetCurrentShipId(int sId)
    {
        currShipId = sId;
    }
    Dictionary<string, int> shipLevels = new();
    public int GetShipLevelValue(int shipId, string value)
    {
        value = shipId + value;
        if (shipLevels.ContainsKey(value))
            return shipLevels[value];
        shipLevels.Add(value, 0);
        return shipLevels[value];
    }
    public void SetShipLevelValue(int shipId, string value, int level)
    {
        value = shipId + value;
        if (shipLevels.ContainsKey(value))
            shipLevels[value] = level;
        else
            shipLevels.Add(value, level);
    }
    private Dictionary<int, bool> unlockedShips = new();
    public void SetShipUnlocked(int shipId, bool unlocked)
    {
        if (unlockedShips.ContainsKey(shipId)) unlockedShips[shipId] = unlocked;
        else unlockedShips.Add(shipId, unlocked);
    }
    public bool GetShipUnlocked(int shipId)
    {
        if (unlockedShips.ContainsKey(shipId)) return unlockedShips[shipId];
        else { unlockedShips[shipId] = false; return false; }
    }
    public static string moneySymbol = "™";
    private Dictionary<string, float[]> islandData = new();
    public float[] GetIslandData(string key)
    {
        if (islandData.ContainsKey(key)) return islandData[key];
        return new float[] { };
    }
    public void SetIslandData(string key, float[] data)
    {
        if (islandData.ContainsKey(key)) islandData[key] = data;
        else islandData.Add(key, data);
    }
    private Dictionary<int, Vector3> enemyShipPositions = new();
    public Dictionary<int, Vector3> GetAllEnemyShipPositions()
    {
        return enemyShipPositions;
    }
    public void SetEnemyShipPosition(int key, Vector3 pos)
    {
        if (enemyShipPositions.ContainsKey(key)) enemyShipPositions[key] = pos;
        else enemyShipPositions.Add(key, pos);
    }
    public int GetNewEnemyShipKey()
    {
        int i = 0;
        while (true)
        {
            if (!enemyShipPositions.ContainsKey(i)) return i;
            i++;
            if (i > 99999999) return -1;
        }
    }
}