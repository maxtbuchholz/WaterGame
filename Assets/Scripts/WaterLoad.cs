using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class WaterLoad : MonoBehaviour
{
    private static WaterLoad instance;
    private List<Vector2> tilesCoords;
    private List<Vector2> currentlyLoadedCoords;
    private List<GameObject> currentlyLoadedTiles;
    [SerializeField] Transform waterParent;
    [SerializeField] GameObject water;
    [SerializeField] IslandTracker islandTracker;
    //public TileDeleter tileDeleter;
    private float maxLoadRadius = 800.0f;
    private float inBetween = 100.0f;
    Dictionary<Vector2, GameObject> waterTiles = new();

    List<Vector2> extraNoDeleteTiles = new();
    Vector2 prevLoc = new();
    bool firstLoaded = false;
    private SaveData saveData;
    private void Start()
    {
        saveData = SaveData.Instance;
        saveData.CheckLoad();
    }
    private void Update()
    {
        Vector2 curFocusPos = new Vector2(Mathf.FloorToInt(transform.position.x / inBetween) * inBetween, Mathf.FloorToInt(transform.position.z / inBetween) * inBetween);
        if (!firstLoaded || (prevLoc != curFocusPos))
        {
            firstLoaded = true;
            prevLoc = curFocusPos;
            float radii = maxLoadRadius / 2;
            List<Vector2> wantedTiles = new();
            HashSet<Vector2> atLeastOnceLoadedSeaCoords = saveData.GetSeaCoords();
            for (float z = -radii; z < radii; z += inBetween)
            {
                for (float x = -radii; x < radii; x += inBetween)
                {
                    if (Mathf.Pow(Mathf.Pow(x, 2) + Mathf.Pow(z, 2), 0.5f) < radii)
                    {
                        wantedTiles.Add(new Vector2(curFocusPos.x + x, curFocusPos.y + z));
                        if (!atLeastOnceLoadedSeaCoords.Contains(wantedTiles[^1]))
                        {
                            saveData.AddSeaCoords(wantedTiles[^1]);
                            islandTracker.TrySpawnIsland(wantedTiles[^1], false);
                        }
                        else if (saveData.IslandExists(wantedTiles[^1]))
                        {
                            islandTracker.ReSpawnIsland(wantedTiles[^1]);
                        }
                    }
                }
            }
            List<Vector2> extraTiles = new();
            foreach (KeyValuePair<Vector2, GameObject> tileC in waterTiles)
            {
                if (wantedTiles.Contains(tileC.Key))
                {
                    wantedTiles.Remove(tileC.Key);
                }
                else
                {
                    extraTiles.Add(tileC.Key);
                }
            }
            for (int i = 0; i < wantedTiles.Count; i++)
            {
                if (extraTiles.Count > 0)
                {
                    GameObject wa = waterTiles[extraTiles[^1]];
                    waterTiles.Remove(extraTiles[^1]);
                    extraTiles.RemoveAt(extraTiles.Count - 1);
                    wa.transform.position = new Vector3(wantedTiles[i].x, 0, wantedTiles[i].y);
                    waterTiles.Add(wantedTiles[i], wa);
                }
                else if (extraNoDeleteTiles.Count > 0)
                {
                    GameObject wa = waterTiles[extraNoDeleteTiles[^1]];
                    waterTiles.Remove(extraTiles[^1]);
                    extraNoDeleteTiles.RemoveAt(extraNoDeleteTiles.Count - 1);
                    wa.transform.position = new Vector3(wantedTiles[i].x, 0, wantedTiles[i].y);
                    waterTiles.Add(wantedTiles[i], wa);
                }
                else
                {
                    GameObject wa = GameObject.Instantiate(water, new Vector3(wantedTiles[i].x, 0, wantedTiles[i].y), Quaternion.identity);
                    wa.transform.parent = waterParent;
                    waterTiles.Add(wantedTiles[i], wa);
                }
            }
            //foreach()
            extraNoDeleteTiles.AddRange(extraTiles);
        }
    }
    //private void Settup()
    //{
    //    currentlyLoadedTiles = GameObject.FindGameObjectsWithTag("SeaTile").ToList<GameObject>();
    //    currentlyLoadedCoords = new List<Vector2>();
    //    foreach (var tile in currentlyLoadedTiles)
    //    {
    //        currentlyLoadedCoords.Add(new Vector2(tile.transform.position.x, tile.transform.position.z));
    //    }
    //}
    //public void ResetTileList()
    //{
    //    tilesCoords = new List<Vector2>();
    //    //tilesCoords.Add(new Vector2(0,0));
    //}
    //public void AddToTileList(Vector2 n)
    //{
    //    tilesCoords.Add(n);
    //}
    //public void FinishTileLoad()
    //{
    //    List<int> RemoveList = new List<int>();
    //    for(int i = 0; i < currentlyLoadedCoords.Count; i++) 
    //    {
    //        if (!tilesCoords.Contains(currentlyLoadedCoords[i]))
    //            RemoveList.Add(i);
    //    }
    //    for(int i = RemoveList.Count - 1; i >= 0; i--)
    //    {
    //            tileDeleter.DeleteTile(currentlyLoadedTiles[RemoveList[i]]);
    //            currentlyLoadedCoords.RemoveAt(RemoveList[i]);
    //            currentlyLoadedTiles.RemoveAt(RemoveList[i]);
    //    }
    //    List<int> AddList = new List<int>();
    //    for (int i = 0; i < tilesCoords.Count; i++)
    //    {
    //        if (!currentlyLoadedCoords.Contains(tilesCoords[i]))
    //            AddList.Add(i);
    //    }
    //    for(int i = 0; i < AddList.Count; i++)
    //    {
    //        currentlyLoadedTiles.Add(tileDeleter.AddTile(tilesCoords[AddList[i]]));
    //        currentlyLoadedCoords.Add(tilesCoords[AddList[i]]);
    //    }
    //}
}
