//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TileDeleter : MonoBehaviour
//{
//    [SerializeField] IslandTracker islandTracker;
//    public GameObject tile;
//    private List<Vector2> allOnceLoadedSeaTiles = new();
//    public void DeleteTile(GameObject des)
//    {
//        GameObject.DestroyImmediate(des.gameObject);
//    }
//    public GameObject AddTile(Vector2 pos)
//    {
//        if (!allOnceLoadedSeaTiles.Contains(pos))
//        {
//            allOnceLoadedSeaTiles.Add(pos);
//            islandTracker.TrySpawnIsland(pos);
//        }
//        return Instantiate(tile, new Vector3(pos.x, 0, pos.y), tile.transform.rotation);
//    }
//}
