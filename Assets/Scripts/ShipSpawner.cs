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
    private float maxSpawnRange = 300;
    private float minSpawnRange = 50;
    private void Start()
    {
        saveData = SaveData.Instance;
        pointToPlayer = PointToPlayer.Instance;
        focalPoint = pointToPlayer.GetFocalPoint();
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
            Destroy(loadedShips[keysToRemoveLoaded[i]]);
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
                if((dst > minSpawnRange) && (dst < maxSpawnRange))
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
                enShip.transform.position = spawnShipPos;
                saveData.SetEnemyShipPosition(spawnShipId, spawnShipPos);
                loadedShips.Add(spawnShipId, enShip);
            }
        }
    }
    private Vector3 GetShipSpawnLocation(Vector3 focalPos)
    {
        int maxTries = 50;
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
                if (!hitShip)
                    return posTry;
            }
        }
        return new Vector3(0, -100, 0);
    }
}