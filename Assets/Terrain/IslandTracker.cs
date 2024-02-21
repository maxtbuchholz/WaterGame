using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.PlayerSettings;

public class IslandTracker : MonoBehaviour
{
    [SerializeField] GameObject islandPrefab;
    // Start is called before the first frame update
    List<Vector2> existingIslands = new();
    Dictionary<Vector2, GameObject> loadedIslands = new();
    float islandLoadDistance = 1000; //water tiles is 750
    float minIslandDistacnce = 150;
    int xDivPos = 0;
    int zDivPos = 0;
    float IntDivAmount = 50;
    private SaveData saveData;
    void Start()
    {
        saveData = SaveData.Instance;
        TrySpawnIsland(Vector2.zero, true);
        TrySpawnIsland(new Vector2(100, 0), true);
        TrySpawnIsland(new Vector2(-100, 0), true);
        TrySpawnIsland(new Vector2(0, 100), true);
        TrySpawnIsland(new Vector2(0, -100), true);
        //existingIslands.Add(new Vector2(0, 0));
        //int keyI = saveData.GetIslandKey(new Vector2(0, 0));
        //saveData.AddIslandCoords(new Vector2(0, 0), keyI);
        //UpdateLoadedIslands();
    }
    private void UpdateLoadedIslands()
    {
        Vector3 currentPos = transform.position;
        Vector2 currentPos2 = transform.position;
        List<Vector2> islandPosToDestroy = new();
        foreach (KeyValuePair<Vector2, GameObject> v2GO in loadedIslands)           //remove islands that should no longer be loaded
        {
            if(Vector3.Distance(currentPos, v2GO.Key) > islandLoadDistance)
            {
                Destroy(v2GO.Value);
                islandPosToDestroy.Add(v2GO.Key);
            }
        }
        foreach(Vector2 v2 in islandPosToDestroy)                                   //actualy remove the islands that should be destroyed
        {
            loadedIslands.Remove(v2);
        }
        for(int i = 0; i < existingIslands.Count; i++)                              //load 'cached' islands that should be now loaded as they are now in range
        {
            if (!loadedIslands.ContainsKey(existingIslands[i]))
            {
                if (Vector2.Distance(existingIslands[i], currentPos2) <= islandLoadDistance)
                {
                    GameObject island = GameObject.Instantiate(islandPrefab, new Vector3(existingIslands[i].x, 0, existingIslands[i].y), Quaternion.identity);
                    loadedIslands.Add(existingIslands[i], island);
                }
            }
        }
    }
    public void TrySpawnIsland(Vector2 pos, bool allwaysSpawn)
    {
        if ((Random.Range(0.0f, 1.0f) > 0.5f) || allwaysSpawn)
        {
            bool farEnoughOut = true;
            foreach (Vector2 checkPos in existingIslands)
                farEnoughOut &= (Vector2.Distance(pos, checkPos) >= minIslandDistacnce);
            if (farEnoughOut)
            {
                existingIslands.Add(pos);
                GameObject island = GameObject.Instantiate(islandPrefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
                int keyI = saveData.GetIslandKey(pos);
                island.GetComponent<IslandGenerator>().StartGenerate(keyI);
                loadedIslands.Add(pos, island);
                saveData.AddIslandCoords(pos, keyI);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        int currDivX = (int)(transform.position.x / IntDivAmount);
        int currDivZ = (int)(transform.position.z / IntDivAmount);
        if((currDivX != xDivPos) || (currDivZ != zDivPos))
        {
            xDivPos = currDivX;
            zDivPos = currDivZ;
            UpdateLoadedIslands();
        }
    }
}
