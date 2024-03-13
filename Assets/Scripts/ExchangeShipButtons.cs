using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExchangeShipButtons : MonoBehaviour
{
    [SerializeField] ShipUpgradeButtons shipUpgradeButtons;
    public void a_Clicked()
    {
        SaveData.Instance.SetCurrentShipId(0);
        PointToPlayer.Instance.GetPlayerShipValues().GetComponent<ShipValueControl>().ShipModelChanged();
        shipUpgradeButtons.SetShipKey();
        shipUpgradeButtons.UpdateLevelButtons();
    }
    public void b_Clicked()
    {
        SaveData.Instance.SetCurrentShipId(1);
        PointToPlayer.Instance.GetPlayerShipValues().GetComponent<ShipValueControl>().ShipModelChanged();
        shipUpgradeButtons.SetShipKey();
        shipUpgradeButtons.UpdateLevelButtons();
    }
    public void c_Clicked()
    {
        SaveData.Instance.SetCurrentShipId(2);
        PointToPlayer.Instance.GetPlayerShipValues().GetComponent<ShipValueControl>().ShipModelChanged();
        shipUpgradeButtons.SetShipKey();
        shipUpgradeButtons.UpdateLevelButtons();
    }
}
