using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
//using UnityFigmaBridge.Editor.FigmaApi;
//using static UnityEditor.PlayerSettings;

public class LoadMap : MonoBehaviour
{
    [SerializeField] Transform focalPosition;
    [SerializeField] GameObject waterPrefab;
    [SerializeField] GameObject islandDebugPrefab;
    [SerializeField] GameObject shipPrefab;
    [SerializeField] RectTransform overlayCanvas;
    [SerializeField] Transform waterparent;
    [SerializeField] Transform shipparent;
    [SerializeField] MapTouchControl mapTouchControl;
    [SerializeField] Transform shipDrive;
    private GameObject ship;
    float tileSize;
    float islandTileSize;
    private SaveData saveData;
    float waterTileWidth = 100;
    float islandTileWidth = 150f;
    HashSet<Vector2> seaTiles;
    Dictionary<Vector2, GameObject> loadedTiles = new();
    Dictionary<Vector2, GameObject> loadedIslands = new();
    HashSet<Vector2> unloadedNeighborTiles = new();
    private Vector2[] neighbors;
    private Rect overlayRect;
    private float currentZoom = 8;
    private float shipWidthToHeight = 0;
    private Dictionary<Vector2, string> islands;
    public void ClearMap()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in waterparent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in shipparent)
        {
            Destroy(child.gameObject);
        }
        loadedTiles = new();
        loadedIslands = new();
        unloadedNeighborTiles = new();
        addPan = Vector2.zero;
        currentZoom = 8;
        mapTouchControl.ResetStart();
    }
    public async void LoadStart()
    {
        neighbors = new Vector2[] { new Vector2(-waterTileWidth, 0), new Vector2(0, -waterTileWidth), new Vector2(waterTileWidth, 0), new Vector2(0, waterTileWidth) };
        saveData = SaveData.Instance;
        tileSize = (float)Screen.width / (float)currentZoom;
        islandTileSize = (float)Screen.width / (float)islandTileWidth;
        transform.localPosition = new Vector3(-(focalPosition.position.x / waterTileWidth) * tileSize, -(focalPosition.position.z / waterTileWidth) * tileSize, 0);
        waterparent.localPosition = new Vector3(-(focalPosition.position.x / waterTileWidth) * tileSize, -(focalPosition.position.z / waterTileWidth) * tileSize, 0);
        Vector2 focalTile = new Vector2(Mathf.Round(focalPosition.position.x / waterTileWidth) * waterTileWidth, Mathf.Round(focalPosition.position.z / waterTileWidth) * waterTileWidth);
        seaTiles = saveData.GetSeaCoords();
        islands = saveData.GetIslandCoords();
        overlayRect = overlayCanvas.rect;
        overlayRect.min -= new Vector2(tileSize / 2, tileSize / 2);
        overlayRect.max += new Vector2(tileSize / 2, tileSize / 2);
        if (seaTiles.Contains(focalTile))
        {
            GameObject tile = GameObject.Instantiate(waterPrefab);
            tile.transform.parent = waterparent;
            tile.transform.localPosition = new Vector3((focalTile.x / waterTileWidth) * tileSize, (focalTile.y / waterTileWidth) * tileSize, 0);
            tile.GetComponent<RectTransform>().sizeDelta = new Vector2(tileSize, tileSize);
            loadedTiles.Add(focalTile, tile);
            await TryLoadIsland(focalTile);
            TryAddNeighbors();
            tile.transform.localPosition = Vector3.zero;
        }
        ship = GameObject.Instantiate(shipPrefab);
        ship.transform.parent = shipparent;
        Rect shipRect = ship.GetComponent<RectTransform>().rect;
        shipWidthToHeight = shipRect.width / shipRect.height;
        ship.transform.localPosition = addPan;// new Vector2(((focalPosition.position.x / waterTileWidth) * tileSize), ((focalPosition.position.y / waterTileWidth) * tileSize));
        ship.GetComponent<RectTransform>().sizeDelta = new Vector2((tileSize / 8) * shipWidthToHeight, tileSize / 8);
        ship.transform.rotation = Quaternion.Euler(0, 0, -shipDrive.rotation.eulerAngles.y);
    }
    private async Task TryLoadIsland(Vector2 pos)
    {
        if (islands.ContainsKey(pos))
        {
            string keyS = islands[pos];
            string filePath = Application.persistentDataPath + "/IslandImages/" + keyS + ".png";
            if (System.IO.File.Exists(filePath))
            {
                System.Byte[] bytes = await File.ReadAllBytesAsync(filePath);
                GameObject isl = GameObject.Instantiate(islandDebugPrefab);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(bytes);
                isl.GetComponent<RawImage>().texture = tex;
                //float multiplyer = tileSize * (float)tex.width;
                isl.GetComponent<RectTransform>().sizeDelta = new Vector2(tileSize * 3, tileSize * 3);
                isl.transform.parent = transform;
                isl.transform.localPosition = new Vector3((pos.x / waterTileWidth) * tileSize, (pos.y / waterTileWidth) * tileSize, -1);
                loadedIslands.Add(pos, isl);
                //isl.transform.localScale *= multiplyer;
                //Destroy(tex);
            }
        }
    }
    public void ToCenter()
    {
        StartCoroutine(CoToCenter());
    }
    private IEnumerator CoToCenter()
    {
        float secondsToGetThere = 1;
        Vector2 movePerSec = addPan/ secondsToGetThere;
        float currTime = 0;
        while (currTime < secondsToGetThere)
        {
            currTime += Time.deltaTime;
            addPan -= movePerSec * Time.deltaTime;
            yield return null;
        }
    }
    private void TryAddNeighbors()
    {
        unloadedNeighborTiles = new();
        foreach(KeyValuePair<Vector2, GameObject> pos in loadedTiles)
        foreach (Vector2 neighbor in neighbors)
        {
            if ((!unloadedNeighborTiles.Contains(pos.Key + neighbor)) && (!loadedTiles.ContainsKey(pos.Key + neighbor)) && (seaTiles.Contains(pos.Key + neighbor)))
                unloadedNeighborTiles.Add(pos.Key + neighbor);
        }
        bool stillNeighborsOnScreen = false;
        foreach(Vector2 nei in unloadedNeighborTiles)
        {
            Vector2 newTilePos = new Vector2((nei.x / waterTileWidth) * tileSize, (nei.y / waterTileWidth) * tileSize);
            if (overlayRect.Contains(newTilePos))
            {
                stillNeighborsOnScreen = true;
                GameObject tile = GameObject.Instantiate(waterPrefab);
                tile.transform.parent = waterparent;
                tile.transform.localPosition = newTilePos;
                tile.GetComponent<RectTransform>().sizeDelta = new Vector2(tileSize, tileSize);
                loadedTiles.Add(nei, tile);
                TryLoadIsland(nei);
            }
        }
        //yield return null;
        if (stillNeighborsOnScreen) TryAddNeighbors();
    }
    private Vector2 addPan = Vector2.zero;
    public void AddPan(Vector2 pan)
    {
        addPan += pan;
        //pan *= 100;
        ship.transform.localPosition = addPan;// new Vector2(((focalPosition.position.x / waterTileWidth) * tileSize), ((focalPosition.position.y / waterTileWidth) * tileSize));
        ship.transform.rotation = Quaternion.Euler(0, 0, -shipDrive.rotation.eulerAngles.y);
        transform.localPosition = new Vector2(-(focalPosition.position.x / waterTileWidth) * tileSize, -(focalPosition.position.z / waterTileWidth) * tileSize) + addPan;
        waterparent.localPosition = new Vector2(-(focalPosition.position.x / waterTileWidth) * tileSize, -(focalPosition.position.z / waterTileWidth) * tileSize) + addPan;
        //UnloadUnneeded();
        TryAddNeighbors();
        //transform.localPosition += new Vector3(((pan.x) * tileSize * 2), ((pan.y) * tileSize * 2), 0);
    }
    public void SetZoom(float zoom)
    {
        addPan *= (currentZoom / zoom);
        //Debug.Log(zoom);
        currentZoom = zoom;
        tileSize = (float)Screen.width / (float)currentZoom;
        //islandTileSize = (float)Screen.width / (float)islandTileWidth;
        foreach (KeyValuePair<Vector2, GameObject> pair in loadedTiles)
        {
            pair.Value.GetComponent<RectTransform>().sizeDelta = new Vector2(tileSize * 3, tileSize * 3);
            pair.Value.transform.localPosition = new Vector3((pair.Key.x / waterTileWidth) * tileSize, (pair.Key.y / waterTileWidth) * tileSize, -1);
        }
        foreach (KeyValuePair<Vector2, GameObject> pair in loadedIslands)
        {
            pair.Value.GetComponent<RectTransform>().sizeDelta = new Vector2(tileSize * 3, tileSize * 3);
            pair.Value.transform.localPosition = new Vector3((pair.Key.x / waterTileWidth) * tileSize, (pair.Key.y / waterTileWidth) * tileSize, -1);

        }
        ship.GetComponent<RectTransform>().sizeDelta = new Vector2((tileSize / 8) * shipWidthToHeight, tileSize / 8);
        ship.transform.localPosition = addPan;// new Vector2(((focalPosition.position.x / waterTileWidth) * tileSize), ((focalPosition.position.y / waterTileWidth) * tileSize));
        ship.transform.rotation = Quaternion.Euler(0, 0, -shipDrive.rotation.eulerAngles.y);
        transform.localPosition = new Vector2(-(focalPosition.position.x / waterTileWidth) * tileSize, -(focalPosition.position.z / waterTileWidth) * tileSize) + addPan;
        waterparent.localPosition = new Vector2(-(focalPosition.position.x / waterTileWidth) * tileSize, -(focalPosition.position.z / waterTileWidth) * tileSize) + addPan;
        //UnloadUnneeded();
        TryAddNeighbors();
    }
    private void UnloadUnneeded()
    {
        List<Vector2> remove = new();
        foreach(KeyValuePair<Vector2, GameObject> pair in loadedTiles)
        {
            Vector2 test = new Vector2((pair.Key.x / waterTileWidth) * tileSize, (pair.Key.y / waterTileWidth) * tileSize);
            if (!overlayRect.Contains(test))
            {
                remove.Add(pair.Key);
            }
        }
        foreach(Vector2 pos in remove)
        {
            GameObject rem = loadedTiles[pos];
            Destroy(rem);
            loadedTiles.Remove(pos);
            if (loadedIslands.ContainsKey(pos))
            {
                rem = loadedIslands[pos];
                Destroy(rem);
                loadedIslands.Remove(pos);
            }
        }
    }
}
