using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    private static SaveData instance;
    public static SaveData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindAnyObjectByType<SaveData>();
            }
            return instance;
        }
    }
    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        islandKeyIncriment = islandCoordsToKey.Count;
    }
    int islandKeyIncriment = 0;
    private HashSet<Vector2> seenSeaCoords = new();
    public void AddSeaCoords(Vector2 seaCoord)
    {
        if (!seenSeaCoords.Contains(seaCoord)) seenSeaCoords.Add(seaCoord);
    }
    public HashSet<Vector2> GetSeaCoords()
    {
        return seenSeaCoords;
    }
    private Dictionary<Vector2, string> islandCoordsToKey = new();
    public void AddIslandCoords(Vector2 islCoord, int key)
    {
        if (!islandCoordsToKey.ContainsKey(islCoord)) islandCoordsToKey.Add(islCoord, key.ToString());
    }
    public int GetIslandKey(Vector2 islCoord)
    {
        if (islandCoordsToKey.ContainsKey(islCoord)) return -1;
        islandKeyIncriment++;
        return islandKeyIncriment;
    }
    public Dictionary<Vector2, string> GetIslandCoords()
    {
        return islandCoordsToKey;
    }
}
