 using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShipValueControl : MonoBehaviour
{
    [SerializeField] public Transform ship_drive = null;
    [SerializeField] GameObject shipDrivePrefab;
    [SerializeField] public Transform turrets_parent;
    [SerializeField] private Transform meshOb;
    [HideInInspector] public List<GameObject> shipParts;
    [SerializeField] ShipRotation shipRotation;
    [SerializeField] public bool isPlayer = false;
    private SaveData saveData;
    private int shipModel = -1;
    void Start()
    {
        if (ship_drive == null) SetShipDrive();
        saveData = SaveData.Instance;
        shipParts = new();
        AddChildrenToShipParts(gameObject);
        AddChildrenToShipParts(ship_drive.gameObject);
        if (isPlayer)
        {
            int newShipModel = saveData.GetCurrentShipId();
            if(shipModel != newShipModel)
            {
                shipModel = newShipModel;
                StartCoroutine(UpdateShipType());
            }
            else
                UpdateShipValues();
        }
        else
        {
            //shipModel = 0;
            //GetComponent<LocalTeamController>().ForceChangeTeam(1);
            //StartCoroutine(UpdateShipType());
        }
    }
    public void SetShipDrive()
    {
        GameObject drive = GameObject.Instantiate(shipDrivePrefab);
        drive.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        drive.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        drive.GetComponent<ShipMovement>().shipBody = this.gameObject;
        drive.GetComponent<ShipMovement>().shipRotation = shipRotation;
        ship_drive = drive.transform;
    }
    public void ForceChangeShipType(int shipId)
    {
        if (ship_drive == null) SetShipDrive();
        shipModel = shipId;
        shipModel = 2;
        StartCoroutine(UpdateShipType());
    }
    private void Update()
    {
        transform.position = new Vector3(ship_drive.position.x, transform.position.y , ship_drive.position.z);
        //transform.rotation = ship_drive.rotation;
    }
    public void ShipModelChanged()
    {
        shipModel = saveData.GetCurrentShipId();
        StartCoroutine(UpdateShipType());
    }
    public IEnumerator UpdateShipType()
    {
        List<GameObject> tempDestroy = new();
        GameObject tempTBD = new();
        tempTBD.transform.position = new Vector3(0, -100, 0);
        foreach (Transform child in transform)
        {
            if ((child.name != "default") && (child.name != "brain"))
            {
                //child.transform.parent = tempDestroy.transform;
                tempDestroy.Add(child.gameObject);
            }
            else if(child.name == "default")
            {
                GetComponent<LocalTeamController>().AddObjectToColored(child.gameObject);
            }
        }
        foreach (Transform child in ship_drive)
        {
            //child.transform.parent = tempDestroy.transform;
            if (child.name != "brain")
                tempDestroy.Add(child.gameObject);
        }
        foreach(GameObject go in tempDestroy)
        {
            go.transform.parent = tempTBD.transform;
            go.transform.localPosition = Vector3.zero;
            Destroy(go);
        }
        if (tempDestroy.Count > 0)
        {
            bool notDestroyed = false;
            while (notDestroyed)  {
                notDestroyed = false;
                foreach (GameObject go in tempDestroy)
                {
                    notDestroyed |= (go != null);
                }
                yield return null;
            }
        }
        Destroy(tempTBD);
        GameObject newShip = ShipTypesController.Instance.GetShipModel(shipModel);
        Mesh mesh = newShip.GetComponent<ShipPartPointer>().meshFilter.sharedMesh;
        Material[] maeterials = newShip.GetComponent<ShipPartPointer>().meshFilter.GetComponent<Renderer>().sharedMaterials;
        meshOb.GetComponent<MeshFilter>().mesh = mesh;
        meshOb.GetComponent<MeshCollider>().sharedMesh = mesh;
        meshOb.GetComponent<Renderer>().materials = maeterials;
        ship_drive.GetComponent<MeshCollider>().sharedMesh = mesh;
        GameObject turrets = GameObject.Instantiate(newShip.GetComponent<ShipPartPointer>().turrets);
        turrets.transform.parent = transform;
        turrets.transform.localPosition = Vector3.zero;
        turrets.transform.localRotation = Quaternion.Euler(0, 0, 0);
        turrets.transform.localScale = Vector3.one;
        turrets_parent = turrets.transform;
        TurretPointer turretPointer = turrets.GetComponent<TurretPointer>();
        PlayerFireControl playerFireControl = null;
        AIFireControl aIFireControl = null;
        HealthController healthController = GetComponent<HealthController>();
        ShipMovement shipMovement = ship_drive.GetComponent<ShipMovement>();
        shipMovement.backParticles.Clear();
        shipMovement.frontParticle = null;
        shipMovement.funnelParticles.Clear();
        playerFireControl = GetComponent<PlayerFireControl>();
        if (playerFireControl != null)
            playerFireControl.turrets = new();
        aIFireControl = GetComponent<AIFireControl>();
        if (aIFireControl != null)
            aIFireControl.turrets = new();
        GameObject details = GameObject.Instantiate(newShip.GetComponent<ShipPartPointer>().details);
        details.transform.parent = transform;
        details.transform.localPosition = Vector3.zero;
        details.transform.localRotation = Quaternion.Euler(0, 0, 0);
        details.transform.localScale = Vector3.one;
        shipMovement.funnelParticles = details.GetComponent<DetailsPointer>().funnelEffects;
        GameObject effects = GameObject.Instantiate(newShip.GetComponent<ShipPartPointer>().effects);
        effects.transform.parent = ship_drive;
        effects.transform.localPosition = Vector3.zero;
        effects.transform.localRotation = Quaternion.Euler(0, 0, 0);
        effects.transform.localScale = Vector3.one;
        shipMovement.backParticles = effects.GetComponent<EffectsPointer>().backParticles;
        shipMovement.frontParticle = effects.GetComponent<EffectsPointer>().frontParticles;
        effects.GetComponent<EffectsPointer>().frontTrail.shipMovement = ship_drive.GetComponent<ShipMovement>();
        ship_drive.GetComponent<ShipMovement>().Start();
        shipParts = new();
        AddChildrenToShipParts(gameObject);
        AddChildrenToShipParts(ship_drive.gameObject);
        foreach (TurretController turretController in turretPointer.turrets)
        {
            turretController.shipValues = this;
            if (playerFireControl != null)
                playerFireControl.turrets.Add(turretController);
            if (aIFireControl != null)
                aIFireControl.turrets.Add(turretController);
        }
        foreach (DetectHit detecthit in turretPointer.detectHit)
        {
            detecthit.healthController = healthController;
        }
        UpdateShipValues();
        GetComponent<LocalTeamController>().SetGameObjectColors();
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
