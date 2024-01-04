using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDeleter : MonoBehaviour
{
    public GameObject tile;
    public void DeleteTile(GameObject des)
    {
        GameObject.DestroyImmediate(des.gameObject);
    }
    public GameObject AddTile(Vector2 pos)
    {
        return Instantiate(tile, new Vector3(pos.x, tile.transform.position.y, pos.y), tile.transform.rotation);
    }
}
