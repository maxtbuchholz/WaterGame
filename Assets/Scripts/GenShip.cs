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
