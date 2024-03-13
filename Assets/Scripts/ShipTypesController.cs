using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipTypesController : MonoBehaviour
{
    [SerializeField] List<GameObject> ships;
    private static ShipTypesController instance;
    public static ShipTypesController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindAnyObjectByType<ShipTypesController>();
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
    public GameObject GetShipModel(int model)
    {
        if (ships.Count > model)
            return ships[model];
        return ships[0];
    }
}
