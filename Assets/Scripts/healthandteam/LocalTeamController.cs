using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LocalTeamController : MonoBehaviour
{
    [SerializeField] int teamId = -1;
    [SerializeField] List<GameObject> coloredObjects;
    [SerializeField] FortTurretControl fortTurretControl;
    [SerializeField] PlayerFireControl playerFireControl;
    [SerializeField] AIFireControl aIFireControl;
    [SerializeField] bool isplayer = false;
    [SerializeField] HealthController healthController;
    [HideInInspector] ShipMovement shipMovement;
    [SerializeField] bool isMortar = false;
    private Color teamColor;
    // Start is called before the first frame update
    void Start()
    {
        if (teamId == -1)
            SetTeamRandom();
        UpdateMortarTeam();
        TeamsController.Instance.AddLocalTeam(this);
    }
    void SetTeamRandom()
    {
        if (!isplayer)
            teamId = Random.Range(1, 6);
        else
            teamId = 0;
        SetGameObjectColors();
        if (fortTurretControl != null) fortTurretControl.SetTeam(teamId);
        if (playerFireControl != null) playerFireControl.SetTeamId(teamId);
        if (aIFireControl != null) aIFireControl.SetTeamId(teamId);
        if (healthController != null) healthController.SetTeam(teamId);
        UpdateMortarTeam();
        if (shipMovement != null)
        {
            shipMovement.teamId = teamId;
        }
    }
    public void SetShipDrive(ShipMovement shipMovement)
    {
        this.shipMovement = shipMovement;
        shipMovement.teamId = teamId;
    }
    public void SetGameObjectColors()
    {
        //if (isMortar) Debug.Log("Changing Color");
        teamColor = TeamsController.Instance.GetTeamColor(teamId, this);
        foreach (GameObject ob in coloredObjects)
        {
            if (ob != null)
            {
                List<Material> obMat = ob.GetComponent<Renderer>().materials.ToList();
                foreach (Material mat in obMat)
                {
                    if (mat.name == "fortbrick (Instance)")
                    {
                        mat.SetColor("_BaseColor", teamColor);
                    }
                    else if (mat.name == "TeamColor (Instance)")
                    {
                        mat.SetColor("_BaseColor", teamColor);
                    }
                    else if (mat.name == "ship_color (Instance)")
                    {
                        mat.SetColor("_BaseColor", teamColor);
                    }
                }
                if (ob.name == "MoveBar")
                {
                    if (ob.TryGetComponent<Image>(out Image iM))
                    {
                        iM.color = teamColor;
                    }
                }
            }
        }
    }
    public int GetTeam()
    {
        if (teamId == -1)
            SetTeamRandom();
        return teamId;
    }
    public void ForceChangeTeam(int newTeam)
    {
        teamId = newTeam;
        SetGameObjectColors();
        if (fortTurretControl != null) fortTurretControl.SetTeam(teamId);
        if (playerFireControl != null) playerFireControl.SetTeamId(teamId);
        if (aIFireControl != null) aIFireControl.SetTeamId(teamId);
        if (healthController != null) healthController.SetTeam(teamId);
        UpdateMortarTeam();
        if(shipMovement != null)
        {
            shipMovement.teamId = newTeam;
        }
    }
    private List<MortarController> mortarControllers = new();
    public void AddChildMortar(MortarController mortarController)
    {
        mortarController.SetTeam(teamId);
        if(!mortarControllers.Contains(mortarController))
            mortarControllers.Add(mortarController);
    }
    public void UpdateMortarTeam()
    {
        foreach (MortarController mC in mortarControllers)
            mC.SetTeam(teamId);
    }
    public void AddObjectToColored(GameObject go)
    {
        if (!coloredObjects.Contains(go))
        {
            coloredObjects.Add(go);
            SetGameObjectColors();
        }
    }
}
