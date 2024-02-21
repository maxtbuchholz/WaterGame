using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
//using UnityFigmaBridge.Editor.FigmaApi;
//using static UnityEditor.PlayerSettings;

public class LoadMap : MonoBehaviour
{
    [SerializeField] Transform focalPosition;
    [SerializeField] GameObject waterPrefab;
    [SerializeField] GameObject islandDebugPrefab;
    [SerializeField] RectTransform overlayCanvas;
    [SerializeField] Transform waterparent;
    private float tilesPerWidth = 8;
    float tileSize;
    float islandTileSize;
    private SaveData saveData;
    float waterTileWidth = 100;
    float islandTileWidth = 150f;
    HashSet<Vector2> seaTiles;
    HashSet<Vector2> loadedTiles = new();
    HashSet<Vector2> unloadedNeighborTiles = new();
    private Vector2[] neighbors;
    private Rect overlayRect;
    private Dictionary<Vector2, string> islands;
    public void ClearMap()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        loadedTiles = new();
        unloadedNeighborTiles = new();
    }
    public async void LoadStart()
    {
        neighbors = new Vector2[] { new Vector2(-waterTileWidth, 0), new Vector2(0, -waterTileWidth), new Vector2(waterTileWidth, 0), new Vector2(0, waterTileWidth) };
        saveData = SaveData.Instance;
        tileSize = (float)Screen.width / (float)tilesPerWidth;
        islandTileSize = (float)Screen.width / (float)islandTileWidth;
        transform.localPosition = new Vector3(-(focalPosition.position.x / waterTileWidth) * tileSize, -(focalPosition.position.z / waterTileWidth) * tileSize, 0);
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
            loadedTiles.Add(focalTile);
            await TryLoadIsland(focalTile);
            StartCoroutine(TryAddNeighbors());
        }
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
                //isl.transform.localScale *= multiplyer;
                //Destroy(tex);
            }
        }
    }
    private IEnumerator TryAddNeighbors()
    {
        unloadedNeighborTiles = new();
        foreach(Vector2 pos in loadedTiles)
        foreach (Vector2 neighbor in neighbors)
        {
            if ((!unloadedNeighborTiles.Contains(pos + neighbor)) && (!loadedTiles.Contains(pos + neighbor)) && (seaTiles.Contains(pos + neighbor)))
                unloadedNeighborTiles.Add(pos + neighbor);
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
                loadedTiles.Add(nei);
                TryLoadIsland(nei);
            }
        }
        yield return null;
        if (stillNeighborsOnScreen) StartCoroutine(TryAddNeighbors());
    }
    public void SetPan(Vector2 pan)
    {
        pan *= 100;
        transform.localPosition = new Vector3((-(focalPosition.position.x / waterTileWidth) * tileSize) + pan.x, (-(focalPosition.position.z / waterTileWidth) * tileSize) + pan.y, 0);
    }
}
