using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandTracker : MonoBehaviour
{
    [SerializeField] GameObject islandPrefab;
    // Start is called before the first frame update
    List<Vector2> existingIslands = new();
    List<GameObject> loadedIslands = new();
    List<Vector2> loadedCoords = new();
    float islandLoadDistance = 500;
    float minIslandDistacnce = 150;
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
        for (int i = loadedIslands.Count - 1; i >= 0; i--)
        {
            if(Vector3.Distance(currentPos, loadedIslands[i].transform.position) > islandLoadDistance)
            {
                Destroy(loadedIslands[i]);
                loadedIslands.RemoveAt(i);
                loadedCoords.RemoveAt(i);
            }
        }
        for(int i = 0; i < existingIslands.Count; i++)
        {
            if (!loadedCoords.Contains(existingIslands[i]))
            {
                if (Vector2.Distance(existingIslands[i], currentPos2) <= islandLoadDistance)
                {
                    loadedIslands.Add(GameObject.Instantiate(islandPrefab, existingIslands[i], Quaternion.identity));
                    loadedCoords.Add(existingIslands[i]);
                }
            }
        }
    }
    int islandCount = 0;
    public void TrySpawnIsland(Vector2 pos)
    {
        if (Random.Range(0.0f, 1.0f) > 0.98f)
        {
            bool farEnoughOut = true;
            foreach (Vector2 checkPos in existingIslands)
                farEnoughOut &= (Vector2.Distance(pos, checkPos) >= minIslandDistacnce);
            if (farEnoughOut && (islandCount < 2))
            {
                islandCount++;
                existingIslands.Add(pos);
                loadedCoords.Add(pos);
                loadedIslands.Add(GameObject.Instantiate(islandPrefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity));
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
