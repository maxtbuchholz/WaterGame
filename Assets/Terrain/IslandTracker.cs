using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandTracker : MonoBehaviour
{
    [SerializeField] GameObject islandPrefab;
    // Start is called before the first frame update
    List<Vector2> existingIslands = new();
    Dictionary<Vector2, GameObject> loadedIslands = new();
    float islandLoadDistance = 1000; //water tiles is 750
    float minIslandDistacnce = 50;
    int xDivPos = 0;
    int zDivPos = 0;
    float IntDivAmount = 50;
    void Start()
    {
        existingIslands.Add(new Vector2(0, 0));
        UpdateLoadedIslands();
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
    public void TrySpawnIsland(Vector2 pos)
    {
        if (Random.Range(0.0f, 1.0f) > 0.95f)
        {
            bool farEnoughOut = true;
            foreach (Vector2 checkPos in existingIslands)
                farEnoughOut &= (Vector2.Distance(pos, checkPos) >= minIslandDistacnce);
            if (farEnoughOut)
            {
                existingIslands.Add(pos);
                GameObject island = GameObject.Instantiate(islandPrefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
                loadedIslands.Add(pos, island);
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
