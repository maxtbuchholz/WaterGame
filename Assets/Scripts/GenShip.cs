using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenShip : MonoBehaviour
{
    SaveData saveData;
    int key = -1;
    public void GenerateShip(int key)
    {
        this.key = key;
        saveData = SaveData.Instance;
        int[] data = saveData.GetEnemyShipData(key);
        int teamId;
        int shipId;
        if (data.Length > 0)
        {
            teamId = data[0];
            shipId = data[1];
        }
        else
        {
            teamId = Random.Range(1, 6);
            shipId = Random.Range(0, 3);
            int playerShipId = saveData.GetCurrentShipId();
            if(playerShipId == 0)
            {
                shipId = Random.Range(0, 10);
                if(shipId < 7)
                {
                    shipId = 0;
                }
                else if(shipId < 9)
                {
                    shipId = 1;
                }
                else
                {
                    shipId = 2;
                }
            }
            else if(playerShipId == 1)
            {
                shipId = Random.Range(0, 10);
                if(shipId < 4)
                {
                    shipId = 0;
                }
                else if(shipId < 8)
                {
                    shipId = 1;
                }
                else
                {
                    shipId = 2;
                }
            }
            else
            {
                shipId = Random.Range(0, 10);
                if(shipId < 3)
                {
                    shipId = 0;
                }
                else if(shipId < 6)
                {
                    shipId = 1;
                }
                else
                {
                    shipId = 2;
                }
            }
            saveData.SetEnemyShipData(key, new int[] { teamId, shipId });
        }
        GetComponent<LocalTeamController>().ForceChangeTeam(teamId);
        GetComponent<ShipValueControl>().ForceChangeShipType(shipId);
    }
    private void Update()
    {
        if (key == -1) return;
        if (saveData == null) saveData = SaveData.Instance;
        saveData.SetEnemyShipPosition(key, transform.position);
    }
    public void UnloadShip()
    {
        GetComponent<ShipValueControl>().UnloadShip();
    }
}
