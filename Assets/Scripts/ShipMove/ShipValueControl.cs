 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipValueControl : MonoBehaviour
{
    [SerializeField] public Transform ship_drive;
    [SerializeField] public Transform turrets_parent;
    [HideInInspector] public List<GameObject> shipParts;
    [SerializeField] public bool isPlayer = false;
    private SaveData saveData;
    private int shipModel = 0;
    void Start()
    {
        saveData = SaveData.Instance;
        shipParts = new();
        AddChildrenToShipParts(gameObject);
        if (isPlayer)
        {
            shipModel = saveData.GetCurrentShipId();
            Set_Health(saveData.GetShipLevelValue(shipModel, "health"));
            Set_Damage(saveData.GetShipLevelValue(shipModel, "damage"));
            Set_Speed(saveData.GetShipLevelValue(shipModel, "speed"));
        }
    }
    private void Update()
    {
        transform.position = new Vector3(ship_drive.position.x, transform.position.y , ship_drive.position.z);
        //transform.rotation = ship_drive.rotation;
    }
    public void UpdateShipValues()
    {
        Set_Health(saveData.GetShipLevelValue(shipModel, "health"));
        Set_Damage(saveData.GetShipLevelValue(shipModel, "damage"));
        Set_Speed(saveData.GetShipLevelValue(shipModel, "speed"));
    }
    private void AddChildrenToShipParts(GameObject go)
    {
        shipParts.Add(go);
        for (int i = 0; i < go.transform.childCount; i++)
            AddChildrenToShipParts(go.transform.GetChild(i).gameObject);
    }
    public void Set_Health(int health)
    {
        float fHealth = ShipLevel.hea_level_effect[shipModel][health];
        GetComponent<HealthController>().SetMaxHealth(fHealth);
    }
    public void Set_Damage(int damage)
    {
        float fdamage = ShipLevel.dam_level_effect[shipModel][damage];
        foreach (Transform child in turrets_parent)
        {
            child.GetComponent<TurretController>().SetDamage(fdamage);
        }
    }
    public void Set_Speed(int speed)
    {
        float fSpeed = ShipLevel.spe_level_effect[shipModel][speed];
        ship_drive.GetComponent<ShipMovement>().SetMaxSpeed(fSpeed);
    }
}
