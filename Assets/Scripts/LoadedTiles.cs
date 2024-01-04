using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class LoadedTiles
{
    private static LoadedTiles instance;
    private List<Vector2> tilesCoords;
    private List<Vector2> currentlyLoadedCoords;
    private List<GameObject> currentlyLoadedTiles;
    public TileDeleter tileDeleter;
    public static LoadedTiles Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LoadedTiles();
                instance.Settup();
            }

            return instance;
        }
    }
    private void Settup()
    {
        currentlyLoadedTiles = GameObject.FindGameObjectsWithTag("SeaTile").ToList<GameObject>();
        currentlyLoadedCoords = new List<Vector2>();
        foreach (var tile in currentlyLoadedTiles)
        {
            currentlyLoadedCoords.Add(new Vector2(tile.transform.position.x, tile.transform.position.z));
        }
    }
    public void ResetTileList()
    {
        tilesCoords = new List<Vector2>();
        //tilesCoords.Add(new Vector2(0,0));
    }
    public void AddToTileList(Vector2 n)
    {
        tilesCoords.Add(n);
    }
    public void FinishTileLoad()
    {
        List<int> RemoveList = new List<int>();
        for(int i = 0; i < currentlyLoadedCoords.Count; i++) 
        {
            if (!tilesCoords.Contains(currentlyLoadedCoords[i]))
                RemoveList.Add(i);
        }
        for(int i = RemoveList.Count - 1; i >= 0; i--)
        {
                tileDeleter.DeleteTile(currentlyLoadedTiles[RemoveList[i]]);
                currentlyLoadedCoords.RemoveAt(RemoveList[i]);
                currentlyLoadedTiles.RemoveAt(RemoveList[i]);
        }
        List<int> AddList = new List<int>();
        for (int i = 0; i < tilesCoords.Count; i++)
        {
            if (!currentlyLoadedCoords.Contains(tilesCoords[i]))
                AddList.Add(i);
        }
        for(int i = 0; i < AddList.Count; i++)
        {
            currentlyLoadedTiles.Add(tileDeleter.AddTile(tilesCoords[AddList[i]]));
            currentlyLoadedCoords.Add(tilesCoords[AddList[i]]);
        }
    }

    void Awake()
    {
        instance = this;
    }
}
