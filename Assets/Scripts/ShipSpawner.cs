using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSpawner : MonoBehaviour
{
    [SerializeField] GameObject shipPrefab;
    Dictionary<int, GameObject> loadedShips = new();
    private SaveData saveData;
    private PointToPlayer pointToPlayer;
    private Transform focalPoint;
    private float goalShipCount = 20;
    private float maxSpawnRange = 250;
    private float minSpawnRange = 50;
    private static ShipSpawner instance;
    public static ShipSpawner Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindAnyObjectByType<ShipSpawner>();
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
        saveData = SaveData.Instance;
        pointToPlayer = PointToPlayer.Instance;
        focalPoint = pointToPlayer.GetFocalPoint();
    }
    public void DestroyShip(GameObject ship)
    {
        List<int> keysToRemove = new();
        if (loadedShips.ContainsValue(ship))
        {
            foreach(KeyValuePair<int, GameObject> pair in loadedShips)
            {
                if(pair.Value == ship)
                {
                    keysToRemove.Add(pair.Key);
                }
            }
        }
        foreach(int i in keysToRemove)
        {
            loadedShips.Remove(i);
            SaveData.Instance.EnemyShipDestroyed(i);
        }
    }
    private void Update()
    {
        Vector3 focalPos = focalPoint.position;
        List<int> keysToRemoveLoaded = new();
        foreach(KeyValuePair<int, GameObject> pair in loadedShips)
        {
            float dst = Vector3.Distance(focalPos, pair.Value.transform.position);
            if (dst > maxSpawnRange)
                keysToRemoveLoaded.Add(pair.Key);
        }
        for(int i = 0; i < keysToRemoveLoaded.Count; i++)     //unload far away ships
        {
            loadedShips[keysToRemoveLoaded[i]].GetComponent<HealthController>().UnloadShip();
            loadedShips[keysToRemoveLoaded[i]].GetComponent<GenShip>().UnloadShip();
            loadedShips.Remove(keysToRemoveLoaded[i]);
        }
        if(loadedShips.Count < goalShipCount)   //then load more ships
        {
            Dictionary<int, Vector3> AllShipsPos = saveData.GetAllEnemyShipPositions();
            bool foundShipPos = false;
            Vector3 spawnShipPos = Vector3.zero;
            int spawnShipId = -1;
            foreach(KeyValuePair<int, Vector3> pair in AllShipsPos)         //look through prev saved ships to spawn
            {
                float dst = Vector3.Distance(focalPos, pair.Value);
                if((dst < maxSpawnRange))
                {
                    if (!loadedShips.ContainsKey(pair.Key))
                    {
                        foundShipPos = true;
                        spawnShipPos = pair.Value;
                        spawnShipId = pair.Key;
                        break;
                    }
                }
            }
            if (!foundShipPos)                          //no ship previously saved to spawn
            {
                spawnShipPos = GetShipSpawnLocation(focalPos);
                if (spawnShipPos.y != -100)
                {
                    spawnShipId = saveData.GetNewEnemyShipKey();
                    foundShipPos = true;
                }
            }
            if (foundShipPos)
            {
                GameObject enShip = GameObject.Instantiate(shipPrefab);
                enShip.GetComponent<ShipValueControl>().SetPositionInit(spawnShipPos);
                enShip.GetComponent<GenShip>().GenerateShip(spawnShipId);
                //enShip.transform.position = spawnShipPos;
                saveData.SetEnemyShipPosition(spawnShipId, spawnShipPos);
                loadedShips.Add(spawnShipId, enShip);
            }
        }
    }
    private Vector3 GetShipSpawnLocation(Vector3 focalPos)
    {
        int maxTries = 10;
        float furthestDst = -1;
        Vector3 bestPos = Vector3.zero;
        for(int i = 0; i < maxTries; i++)
        {
            float angle = Random.Range(0.0f, 360.0f) * Mathf.Deg2Rad;
            float range = Random.Range(minSpawnRange, maxSpawnRange);
            Vector2 vecTry = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * range;
            Vector3 posTry = new Vector3(focalPos.x + vecTry.x, 0, focalPos.z + vecTry.y);
            RaycastHit[] hits = Physics.BoxCastAll(new Vector3(posTry.x, 50, posTry.z), new Vector3(10, 1, 10), Vector3.down, Quaternion.identity);
            bool hitLand = false;
            foreach (RaycastHit hit in hits)
                if (hit.collider.CompareTag("Land"))
                    hitLand = true;
            if (!hitLand)
            {
                bool hitShip = false;
                foreach (KeyValuePair<int, GameObject> pair in loadedShips)
                    if (Vector3.Distance(pair.Value.transform.position, posTry) < 30)
                        hitShip = true;
                if (!hitShip)                           //position is technically okay
                {
                    float totalDistance = 0;
                    foreach(KeyValuePair<int, GameObject> pair in loadedShips)
                    {
                        totalDistance += Vector3.Distance(pair.Value.transform.position, posTry); 
                    }
                    Vector3 playerPos = PointToPlayer.Instance.GetPlayerShip().transform.position;
                    float playerDst = Vector3.Distance(playerPos, posTry);
                    totalDistance += 2 * playerDst;
                    totalDistance /= (loadedShips.Count + 2);
                    if((furthestDst == -1) || (totalDistance > furthestDst))
                    {
                        furthestDst = totalDistance;
                        bestPos = posTry;
                    }
                }
            }
        }
        if (furthestDst != -1)
            return bestPos;
        return new Vector3(0, -100, 0);
    }
}