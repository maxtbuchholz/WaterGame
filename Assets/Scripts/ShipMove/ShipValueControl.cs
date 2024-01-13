using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipValueControl : MonoBehaviour
{
    [SerializeField] public Transform ship_drive;
    [HideInInspector] public List<GameObject> shipParts;
    void Start()
    {
        shipParts = new();
        AddChildrenToShipParts(gameObject);
    }
    private void Update()
    {
        transform.position = new Vector3(ship_drive.position.x, transform.position.y , ship_drive.position.z);
        //transform.rotation = ship_drive.rotation;
    }
    private void AddChildrenToShipParts(GameObject go)
    {
        shipParts.Add(go);
        for (int i = 0; i < go.transform.childCount; i++)
            AddChildrenToShipParts(go.transform.GetChild(i).gameObject);
    }
}
