using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
    public void CheckLoad()
    {
        if (!didInit)
            LoadFilesOuter();
    }
    private Vector3 playerShipPos;
    private float playerShipRot;
    int islandKeyIncriment = 0;
    private float money = 500;
    private int currShipId = 0;
    private Dictionary<Vector2, string> islandCoordsToKey = new();
    private Dictionary<string, Vector3> fortKeyToCoords = new();
    Dictionary<string, int> fortTeam = new();
    Dictionary<string, FortSaveLevel> fortLevels = new();
    Dictionary<string, Vector3> mortarPositions = new();
    Dictionary<string, int> shipLevels = new();
    private Dictionary<int, bool> unlockedShips = new();
    private Dictionary<string, float[]> islandData = new();
    private Dictionary<int, Vector3> enemyShipPositions = new();
    private HashSet<Vector2> seenSeaCoords = new();
    private Dictionary<int, int[]> enemyShipData = new();
    Dictionary<string, float[]> fortCenter = new();
    float[] seed;
    private IEnumerator Save_Co_Player()
    {
        float minTimeBetween = 0.5f;
        float time = minTimeBetween;
        while (true)
        {
            time -= Time.deltaTime;
            if(time <= 0)
            {
                string destination = Application.persistentDataPath + "/" + "player_ship_pos" + ".txt";
                FileStream file_p;
                if (File.Exists(destination))
                {
                    file_p = File.OpenWrite(destination);
                }
                else file_p = File.Create(destination);

                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(file_p, Serialize.V3(playerShipPos));
                    file_p.Close();
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                }
                destination = Application.persistentDataPath + "/" + "player_ship_rot" + ".txt";
                FileStream file_r;
                if (File.Exists(destination))
                {
                    file_r = File.OpenWrite(destination);
                }
                else file_r = File.Create(destination);

                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(file_r, (playerShipRot));
                    file_r.Close();
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                }


                time = minTimeBetween + Random.Range(0, 0.5f);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    private IEnumerator Save_Co_Land()
    {
        float minTimeBetween = 0.6f;
        float time = minTimeBetween;
        while (true)
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                string destination = Application.persistentDataPath + "/" + "island_coord_keys" + ".txt";
                FileStream file;
                if (File.Exists(destination))
                {
                    file = File.OpenWrite(destination);
                }
                else file = File.Create(destination);

                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(file, Serialize.V2_S(islandCoordsToKey));
                    file.Close();
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                }
                destination = Application.persistentDataPath + "/" + "fort_key_coords" + ".txt";
                if (File.Exists(destination))
                {
                    file = File.OpenWrite(destination);
                }
                else file = File.Create(destination);

                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(file, Serialize.S_V3(fortKeyToCoords));
                    file.Close();
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                }
                destination = Application.persistentDataPath + "/" + "fort_team" + ".txt";
                if (File.Exists(destination))
                {
                    file = File.OpenWrite(destination);
                }
                else file = File.Create(destination);

                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(file, fortTeam);
                    file.Close();
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                }
                destination = Application.persistentDataPath + "/" + "fort_levels" + ".txt";
                if (File.Exists(destination))
                {
                    file = File.OpenWrite(destination);
                }
                else file = File.Create(destination);
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(file, fortLevels);
                    file.Close();
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                }
                destination = Application.persistentDataPath + "/" + "mortar_poss" + ".txt";
                if (File.Exists(destination))
                {
                    file = File.OpenWrite(destination);
                }
                else file = File.Create(destination);
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(file, Serialize.S_V3(mortarPositions));
                    file.Close();
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                }
                time = minTimeBetween + Random.Range(0, 0.5f);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    private IEnumerator Save_Co_Enemy()
    {
        float minTimeBetween = 0.5f;
        float time = minTimeBetween;
        while (true)
        {
            FileStream file;
            time -= Time.deltaTime;
            if (time <= 0)
            {
                string destination = Application.persistentDataPath + "/" + "enemy_ship_poss" + ".txt";
                if (File.Exists(destination))
                {
                    file = File.OpenWrite(destination);
                }
                else file = File.Create(destination);
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(file, Serialize.I_V3(enemyShipPositions));
                    file.Close();
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                }
                destination = Application.persistentDataPath + "/" + "enemy_ship_poss" + ".txt";
                if (File.Exists(destination))
                {
                    file = File.OpenWrite(destination);
                }
                else file = File.Create(destination);
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(file, Serialize.I_V3(enemyShipPositions));
                    file.Close();
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                }
                destination = Application.persistentDataPath + "/" + "enemy_ship_data" + ".txt";
                if (File.Exists(destination))
                {
                    file = File.OpenWrite(destination);
                }
                else file = File.Create(destination);
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(file, enemyShipData);
                    file.Close();
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                }
                time = minTimeBetween + Random.Range(0, 0.5f);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    public void NewGame()
    {
        LoadFilesOuter(true);
    }
    public bool FirstTimeLoad = false;
    private void LoadFilesOuter(bool forceReload = false)
    {
        didInit = true;
        bool success = false;
        if (!forceReload)
            success = LoadFilesInner();
        if (!success || forceReload)
        {
            FirstTimeLoad = true;
            playerShipPos = new Vector3(-20, 0, -20);
            //InitFile("player_ship_pos",Serialize.V3(playerShipPos));
            playerShipRot = 0;
            InitFile("player_ship_rot", playerShipRot);
            money = 500;
            InitFile("money", money);
            currShipId = 0;
            InitFile("curr_ship_id", currShipId);
            islandCoordsToKey = new();
            InitFile("island_coord_keys",Serialize.V2_S(islandCoordsToKey));
            fortKeyToCoords = new();
            InitFile("fort_key_coords", Serialize.S_V3(fortKeyToCoords));
            fortTeam = new();
            InitFile("fort_team", fortTeam);
            fortLevels = new();
            InitFile("fort_levels", fortLevels);
            mortarPositions = new();
            InitFile("mortar_poss",Serialize.S_V3(mortarPositions));
            shipLevels = new();
            InitFile("ship_levels", shipLevels);
            unlockedShips = new();
            InitFile("unlocked_ships", unlockedShips);
            islandData = new();
            InitFile("island_data", islandData);
            enemyShipPositions = new();
            InitFile("enemy_ship_poss",Serialize.I_V3(enemyShipPositions));
            seenSeaCoords = new();
            InitFile("seen_sea_coords",Serialize.HV2(seenSeaCoords));
            enemyShipData = new();
            InitFile("enemy_ship_data", enemyShipData);
            fortCenter = new();
            InitFile("fort_center", fortCenter);
            seed = new float[] { Random.Range(0, 1_000_000), Random.Range(0, 1_000_000) };
            InitFile("seed", seed);

        }
        islandKeyIncriment = islandCoordsToKey.Count;
        if (!forceReload)
        {
            StartCoroutine(Save_Co_Player());
            StartCoroutine(Save_Co_Land());
            StartCoroutine(Save_Co_Enemy());
        }
    }
    private void InitFile(string filename, object data)
    {
        string destination = Application.persistentDataPath + "/" + filename + ".txt";
        FileStream file;
        if (!File.Exists(destination))
        {
            file = File.Create(destination);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, data);
            file.Close();
        }
        else
        {
            file = File.OpenWrite(destination);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, data);
            file.Close();
        }
    }
    private bool LoadFilesInner()
    {
        if (TryLoadDataFromFile("player_ship_pos", out object data))
        {
            try
            {
                playerShipPos = Deserialize.V3((float[])data);
            }
            catch (System.Exception e)
            {
                return false;
            }
        }
        else return false;
        if (TryLoadDataFromFile("player_ship_rot", out data))
        {
            try
            {
                playerShipRot = (float)data;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }
        else return false;
        if (TryLoadDataFromFile("money", out data))
        {
            try
            {
                money = (float)data;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }
        else return false;
        if (TryLoadDataFromFile("curr_ship_id", out data))
        {
            try
            {
                currShipId = (int)data;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }
        else return false;
        if (TryLoadDataFromFile("island_coord_keys", out data))
        {
            try
            {
                islandCoordsToKey = Deserialize.V2_S((Dictionary<float[], string>)data);
            }
            catch (System.Exception e)
            {
                return false;
            }
        }
        else return false;
        if (TryLoadDataFromFile("fort_key_coords", out data))
        {
            try
            {
                fortKeyToCoords = Deserialize.S_V3((Dictionary<string, float[]>)data);
            }
            catch (System.Exception e)
            {
                return false;
            }
        }
        else return false;
        if (TryLoadDataFromFile("fort_team", out data))
        {
            try
            {
                fortTeam = (Dictionary<string, int>)data;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }
        else return false;
        if (TryLoadDataFromFile("fort_levels", out data))
        {
            try
            {
                fortLevels = (Dictionary<string, FortSaveLevel>)data;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }
        else return false;
        if (TryLoadDataFromFile("mortar_poss", out data))
        {
            try
            {
                mortarPositions = Deserialize.S_V3((Dictionary<string, float[]>)data);
            }
            catch (System.Exception e)
            {
                return false;
            }
        }
        else return false;
        if (TryLoadDataFromFile("ship_levels", out data))
        {
            try
            {
                shipLevels = (Dictionary<string, int>)data;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }
        else return false;
        if (TryLoadDataFromFile("unlocked_ships", out data))
        {
            try
            {
                unlockedShips = (Dictionary<int, bool>)data;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }
        else return false;
        if (TryLoadDataFromFile("island_data", out data))
        {
            try
            {
                islandData = (Dictionary<string, float[]>)data;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }
        else return false;
        if (TryLoadDataFromFile("enemy_ship_poss", out data))
        {
            try
            {
                enemyShipPositions = Deserialize.I_V3((Dictionary<int, float[]>)data);
            }
            catch (System.Exception e)
            {
                return false;
            }
        }
        else return false;
        if (TryLoadDataFromFile("seen_sea_coords", out data))
        {
            try
            {
                seenSeaCoords = Deserialize.HV2((HashSet<float[]>)data);
            }
            catch (System.Exception e)
            {
                return false;
            }
        }
        else return false;
        if (TryLoadDataFromFile("enemy_ship_data", out data))
        {
            try
            {
                enemyShipData = (Dictionary<int, int[]>)data;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }
        else return false;
        if (TryLoadDataFromFile("fort_center", out data))
        {
            try
            {
                fortCenter = (Dictionary<string, float[]>)data;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }
        else return false;
        if (TryLoadDataFromFile("seed", out data))
        {
            try
            {
                seed = (float[])data;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }
        else return false;
        return true;
    }
    private bool TryLoadDataFromFile(string filename, out object data)
    {
        string destination = Application.persistentDataPath + "/" + filename + ".txt";
        FileStream file;
        if (File.Exists(destination))
        {
            try
            {
                file = File.OpenRead(destination);
                BinaryFormatter bf = new BinaryFormatter();
                object read = bf.Deserialize(file);
                file.Close();
                data = read;
                return true;
                //teamColor = SemiDeserializeColorDict((Dictionary<int, float[]>)read);
            }
            catch (System.Exception e)
            {
                data = null;
                return false;
            }
        }
        data = null;
        return false;
    }
    private bool didInit = false;
    public void SetPlayerRot(float rot)
    {
        if (!didInit) LoadFilesOuter();
        playerShipRot = rot;
    }
    public float GetPlayerRot()
    {
        if (!didInit) LoadFilesOuter();
        return playerShipRot;
    }
    public void SetPlayerPos(Vector3 pos)
    {
        if (!didInit) LoadFilesOuter();
        playerShipPos = pos;
    }
    public Vector3 GetPlayerPos()
    {
        if (!didInit) LoadFilesOuter();
        return playerShipPos;
    }
    public void AddSeaCoords(Vector2 seaCoord)
    {
        if (!didInit) LoadFilesOuter();
        if (!seenSeaCoords.Contains(seaCoord)) seenSeaCoords.Add(seaCoord);
        string destination = Application.persistentDataPath + "/" + "seen_sea_coords" + ".txt";
        FileStream file;
        if (File.Exists(destination))
        {
            file = File.OpenWrite(destination);
        }
        else file = File.Create(destination);
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, Serialize.HV2(seenSeaCoords));
            file.Close();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    public HashSet<Vector2> GetSeaCoords()
    {
        if (!didInit) LoadFilesOuter();
        return seenSeaCoords;
    }
    public bool IslandExists(Vector2 pos)
    {
        if (!didInit) LoadFilesOuter();
        return islandCoordsToKey.ContainsKey(pos);
    }
    public void AddIslandCoords(Vector2 islCoord, string key)
    {
        if (!didInit) LoadFilesOuter();
        if (!islandCoordsToKey.ContainsKey(islCoord)) islandCoordsToKey.Add(islCoord, key);
    }
    public int GetNewIslandKey(Vector2 islCoord)
    {
        if (!didInit) LoadFilesOuter();
        if (islandCoordsToKey.ContainsKey(islCoord)) return -1;
        islandKeyIncriment++;
        return islandKeyIncriment;
    }
    public string GetIslandKeyFromCoord(Vector2 islCoord)
    {
        if (!didInit) LoadFilesOuter();
        if (!islandCoordsToKey.ContainsKey(islCoord)) return "-1";
        return islandCoordsToKey[islCoord];
    }
    public Dictionary<Vector2, string> GetIslandCoords()
    {
        if (!didInit) LoadFilesOuter();
        return islandCoordsToKey;
    }
    public void AddFort(string key, Vector3 pos)
    {
        if (!didInit) LoadFilesOuter();
        if (!fortKeyToCoords.ContainsKey(key)) fortKeyToCoords.Add(key, pos);
    }
    public Vector3 GetFortPos(string key)
    {
        if (!didInit) LoadFilesOuter();
        if (fortKeyToCoords.ContainsKey(key)) return fortKeyToCoords[key];
        return Vector3.zero;
    }
    public bool FortExists(string key)
    {
        if (!didInit) LoadFilesOuter();
        return fortKeyToCoords.ContainsKey(key);
    }
    public string GetFortKeyFromPos(Vector3 pos)
    {
        if (!didInit) LoadFilesOuter();
        foreach (KeyValuePair<string, Vector3> pair in fortKeyToCoords)
        {
            if ((pair.Value.x == pos.x) && (pair.Value.z == pos.z)) return pair.Key;
        }
        return "";
    }
    public void SetFortTeam(string key, int team)
    {
        if (!didInit) LoadFilesOuter();
        if (fortTeam.ContainsKey(key)) fortTeam[key] = team;
        else fortTeam.Add(key, team);
    }
    public int GetFortTeam(string key)
    {
        if (!didInit) LoadFilesOuter();
        if (fortTeam.ContainsKey(key)) return fortTeam[key];
        return -1;
    }
    public void SetFortLevels(string key, FortSaveLevel fSL)
    {
        if (!didInit) LoadFilesOuter();
        if (fortLevels.ContainsKey(key)) fortLevels[key] = fSL;
        else fortLevels.Add(key, fSL);
    }
    public FortSaveLevel GetFortLevels(string key)
    {
        if (!didInit) LoadFilesOuter();
        if (fortLevels.ContainsKey(key)) return fortLevels[key];
        return null;
    }
    public void SetMortarPos(string key, Vector3 pos)
    {
        if (!didInit) LoadFilesOuter();
        if (mortarPositions.ContainsKey(key)) mortarPositions[key] = pos;
        else mortarPositions.Add(key, pos);
    }
    public Vector3 GetMortarPos(string key)
    {
        if (!didInit) LoadFilesOuter();
        if (mortarPositions.ContainsKey(key)) return mortarPositions[key];
        return Vector3.zero;
    }
    public bool MortarPosExists(string key)
    {
        if (!didInit) LoadFilesOuter();
        return mortarPositions.ContainsKey(key);
    }
    public float GetMoney()
    {
        if (!didInit) LoadFilesOuter();
        return money;
    }
    public void AddMoney(float change)
    {
        if (!didInit) LoadFilesOuter();
        money += change;
        string destination = Application.persistentDataPath + "/" + "money" + ".txt";
        FileStream file;
        if (File.Exists(destination))
        {
            file = File.OpenWrite(destination);
        }
        else file = File.Create(destination);

        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, money);
            file.Close();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    public int GetCurrentShipId()
    {
        if (!didInit) LoadFilesOuter();
        return currShipId;
    }
    public void SetCurrentShipId(int sId)
    {
        if (!didInit) LoadFilesOuter();
        currShipId = sId;
        string destination = Application.persistentDataPath + "/" + "curr_ship_id" + ".txt";
        FileStream file;
        if (File.Exists(destination))
        {
            file = File.OpenWrite(destination);
        }
        else file = File.Create(destination);

        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, currShipId);
            file.Close();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    public int GetShipLevelValue(int shipId, string value)
    {
        if (!didInit) LoadFilesOuter();
        value = shipId + value;
        if (shipLevels.ContainsKey(value))
            return shipLevels[value];
        shipLevels.Add(value, 0);
        return shipLevels[value];
    }
    public void SetShipLevelValue(int shipId, string value, int level)
    {
        if (!didInit) LoadFilesOuter();
        value = shipId + value;
        if (shipLevels.ContainsKey(value))
            shipLevels[value] = level;
        else
            shipLevels.Add(value, level);
        string destination = Application.persistentDataPath + "/" + "ship_levels" + ".txt";
        FileStream file;
        if (File.Exists(destination))
        {
            file = File.OpenWrite(destination);
        }
        else file = File.Create(destination);
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, shipLevels);
            file.Close();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    public void SetShipUnlocked(int shipId, bool unlocked)
    {
        if (!didInit) LoadFilesOuter();
        if (unlockedShips.ContainsKey(shipId)) unlockedShips[shipId] = unlocked;
        else unlockedShips.Add(shipId, unlocked);
        string destination = Application.persistentDataPath + "/" + "unlocked_ships" + ".txt";
        FileStream file;
        if (File.Exists(destination))
        {
            file = File.OpenWrite(destination);
        }
        else file = File.Create(destination);
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, unlockedShips);
            file.Close();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    public bool GetShipUnlocked(int shipId)
    {
        if (!didInit) LoadFilesOuter();
        if (unlockedShips.ContainsKey(shipId)) return unlockedShips[shipId];
        else { unlockedShips[shipId] = false; return false; }
    }
    public static string moneySymbol = "™";
    public float[] GetIslandData(string key)
    {
        if (!didInit) LoadFilesOuter();
        if (islandData.ContainsKey(key)) return islandData[key];
        return new float[] { };
    }
    public void SetIslandData(string key, float[] data)
    {
        if (!didInit) LoadFilesOuter();
        if (islandData.ContainsKey(key)) islandData[key] = data;
        else islandData.Add(key, data);
        string destination = Application.persistentDataPath + "/" + "island_data" + ".txt";
        FileStream file;
        if (File.Exists(destination))
        {
            file = File.OpenWrite(destination);
        }
        else file = File.Create(destination);
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, islandData);
            file.Close();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    public Dictionary<int, Vector3> GetAllEnemyShipPositions()
    {
        if (!didInit) LoadFilesOuter();
        return enemyShipPositions;
    }
    public void SetEnemyShipPosition(int key, Vector3 pos)
    {
        if (!didInit) LoadFilesOuter();
        if (enemyShipPositions.ContainsKey(key)) enemyShipPositions[key] = pos;
        else enemyShipPositions.Add(key, pos);
    }
    public int GetNewEnemyShipKey()
    {
        if (!didInit) LoadFilesOuter();
        int i = 0;
        while (true)
        {
            if (!enemyShipPositions.ContainsKey(i)) return i;
            i++;
            if (i > 99999999) return -1;
        }
    }
    public void EnemyShipDestroyed(int key)
    {
        if (!didInit) LoadFilesOuter();
        if (enemyShipPositions.ContainsKey(key)) enemyShipPositions.Remove(key);
        if (enemyShipData.ContainsKey(key)) enemyShipData.Remove(key);
    }
    public int[] GetEnemyShipData(int key)
    {
        if (!didInit) LoadFilesOuter();
        if (enemyShipData.ContainsKey(key))
        {
            return enemyShipData[key];
        }
        return new int[] { };
    }
    public void SetEnemyShipData(int key, int[] data)
    {
        if (!didInit) LoadFilesOuter();
        if (enemyShipData.ContainsKey(key))
        {
            enemyShipData[key] = data;
        }
        else
        {
            enemyShipData.Add(key, data);
        }
    }
    public Vector3 GetFortCenter(string key)
    {
        if (!didInit) LoadFilesOuter();
        if (fortCenter.ContainsKey(key)) return new Vector3(fortCenter[key][0], 0, fortCenter[key][1]);
        return new Vector3(0, -100, 0);
    }
    public void SetFortCenter(string key, Vector3 data)
    {
        if (!didInit) LoadFilesOuter();
        if (fortCenter.ContainsKey(key)) fortCenter[key] = new float[] { data.x, data.z };
        fortCenter[key] = new float[] { data.x, data.z };
        InitFile("fort_center", fortCenter);
    }
    public int GetAmountOfPlayerFortTeams()
    {
        int amount = 0;
        foreach(KeyValuePair<string, int> par in fortTeam)
        {
            if (par.Value == 0)
                amount++;
        }
        return amount;
    }
    public string GetClosestFriendlyFort(Vector3 pos)
    {
        string bestKey = "";
        float bestDist = -1;
        foreach(KeyValuePair<string, Vector3> pair in fortKeyToCoords)
        {
            float dst = Vector3.Distance(pos, pair.Value);
            if (fortTeam[pair.Key] == 0)
            {
                if ((bestDist == -1) || (dst < bestDist))
                {
                    bestDist = dst;
                    bestKey = pair.Key;
                }
            }
        }
        return bestKey;
    }
    public float[] GetSeed()
    {
        if (!didInit) LoadFilesOuter();
        return seed;
    }
}