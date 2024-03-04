using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarController : MonoBehaviour
{
    [SerializeField] LocalTeamController localTeamController;
    public void SetTeam(int teamID)
    {
        localTeamController.ForceChangeTeam(teamID);
    }
}
