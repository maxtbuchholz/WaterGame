using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocalTeamController : MonoBehaviour
{
    [SerializeField] int teamId = -1;
    [SerializeField] List<GameObject> coloredObjects;
    [SerializeField] FortTurretControl fortTurretControl;
    [SerializeField] PlayerFireControl playerFireControl;
    [SerializeField] bool isplayer = false;
    [SerializeField] HealthController healthController;
    private Color teamColor;
    // Start is called before the first frame update
    void Start()
    {
        if (!isplayer)
            teamId = Random.Range(1, 7);
        else
            teamId = 0;
        SetGameObjectColors();
        if (fortTurretControl != null) fortTurretControl.SetTeam(teamId);
        if (playerFireControl != null) playerFireControl.SetTeamId(teamId);
        if (healthController != null) healthController.SetTeam(teamId);
    }
    void SetGameObjectColors()
    {
        teamColor = TeamsController.Instance.GetTeamColor(teamId);
        foreach (GameObject ob in coloredObjects)
        {
            List<Material> obMat = ob.GetComponent<Renderer>().materials.ToList();
            foreach(Material mat in obMat)
            {
                if(mat.name == "fortbrick (Instance)")
                {
                    mat.SetColor("_BaseColor", teamColor);
                }
                else if (mat.name == "TeamColor (Instance)")
                {
                    mat.SetColor("_BaseColor", teamColor);
                }
            }
        }
    }
    public void ForceChangeTeam(int newTeam)
    {
        teamId = newTeam;
        SetGameObjectColors();
        if (fortTurretControl != null) fortTurretControl.SetTeam(teamId);
        if (playerFireControl != null) playerFireControl.SetTeamId(teamId);
        if (healthController != null) healthController.SetTeam(teamId);
    }
    public void AddObjectToColored(GameObject go)
    {
        coloredObjects.Add(go);
        SetGameObjectColors();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
