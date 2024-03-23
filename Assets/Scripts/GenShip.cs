using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenShip : MonoBehaviour
{
    public void Start()
    {
        GenerateShip();
    }
    public void GenerateShip()
    {
        int teamId = Random.Range(1, 6);
        GetComponent<LocalTeamController>().ForceChangeTeam(teamId);
        int shipId = Random.Range(0, 3);
        GetComponent<ShipValueControl>().ForceChangeShipType(shipId);
    }
}
