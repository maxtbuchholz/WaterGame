using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipUpgradeButtons : MonoBehaviour
{
    [SerializeField] RectTransform buttonParent;
    [SerializeField] TextMeshProUGUI health_level_text;
    [SerializeField] TextMeshProUGUI damage_level_text;
    [SerializeField] TextMeshProUGUI speed_level_text;
    [SerializeField] TextMeshProUGUI health_price_text;
    [SerializeField] TextMeshProUGUI damage_price_text;
    [SerializeField] TextMeshProUGUI speed_price_text;
    [SerializeField] Button health_button;
    [SerializeField] Button damage_button;
    [SerializeField] Button speed_button;
    //private RectTransform body;
    private void Start()
    {
        //body = GetComponent<RectTransform>();
        InitButtonsSize();
    }
    private SaveData saveData;
    private int shipKey;
    public void SetShipKey()
    {
        saveData = SaveData.Instance;
        shipKey = saveData.GetCurrentShipId();
        UpdateLevelButtons();
        Debug.Log("Updating ship buttons");
    }
    private int cHL, cDL, cSL;
    private void UpdateLevelButtons()
    {
        cHL = saveData.GetShipLevelValue(shipKey, "health");
        cDL = saveData.GetShipLevelValue(shipKey, "damage");
        cSL = saveData.GetShipLevelValue(shipKey, "speed");
        health_level_text.text = cHL + " / " + (ShipLevel.hea_level_effect[shipKey].Count - 1);
        damage_level_text.text = cDL + " / " + (ShipLevel.dam_level_effect[shipKey].Count - 1);
        speed_level_text.text = cSL + " / " + (ShipLevel.spe_level_effect[shipKey].Count - 1);
        if (cHL < (ShipLevel.hea_level_effect[shipKey].Count - 1))
        {
            health_level_text.color = Color.black;
            health_button.interactable = true;
            health_price_text.text = ShipLevel.hea_cost_to_next_level[shipKey][cHL].ToString();
            if (saveData.GetMoney() >= ShipLevel.hea_cost_to_next_level[shipKey][cHL])
            {
                health_price_text.color = Color.green;
            }
            else
                health_price_text.color = Color.red;
        }
        else
        {
            health_button.interactable = false;
            health_price_text.text = "-----";
            health_level_text.color = Color.gray;
            health_price_text.color = Color.gray;
        }
        if (cDL < (ShipLevel.dam_level_effect[shipKey].Count - 1))
        {
            damage_level_text.color = Color.black;
            damage_button.interactable = true;
            damage_price_text.text = ShipLevel.dam_cost_to_next_level[shipKey][cDL].ToString();
            if (saveData.GetMoney() >= ShipLevel.dam_cost_to_next_level[shipKey][cDL])
            {
                damage_price_text.color = Color.green;
            }
            else
                damage_price_text.color = Color.red;
        }
        else
        {
            damage_button.interactable = false;
            damage_price_text.text = "-----";
            damage_level_text.color = Color.gray;
            damage_price_text.color = Color.gray;
        }
        if (cSL < (ShipLevel.spe_level_effect[shipKey].Count - 1))
        {
            speed_level_text.color = Color.black;
            speed_button.interactable = true;
            speed_price_text.text = ShipLevel.spe_cost_to_next_level[shipKey][cSL].ToString();
            if (saveData.GetMoney() >= ShipLevel.spe_cost_to_next_level[shipKey][cSL])
            {
                speed_price_text.color = Color.green;
            }
            else
                speed_price_text.color = Color.red;
        }
        else
        {
            speed_button.interactable = false;
            speed_price_text.text = "-----";
            speed_level_text.color = Color.gray;
            speed_price_text.color = Color.gray;
        }
    }
    public void hea_clicked()
    {
        if (cHL >= (ShipLevel.hea_level_effect[shipKey].Count - 1)) return;
        if (saveData.GetMoney() < ShipLevel.hea_cost_to_next_level[shipKey][cHL]) return;
        PlayerMoneyController.Instance.AddMoney(-ShipLevel.hea_cost_to_next_level[shipKey][cHL]);
        cHL++;
        saveData.SetShipLevelValue(shipKey, "health", cHL);
        PointToPlayer.Instance.GetPlayerShipValues().GetComponent<ShipValueControl>().UpdateShipValues();
        UpdateLevelButtons();
    }
    public void dam_clicked()
    {
        if (cDL >= (ShipLevel.dam_level_effect[shipKey].Count - 1)) return;
        if (saveData.GetMoney() < ShipLevel.dam_cost_to_next_level[shipKey][cDL]) return;
        PlayerMoneyController.Instance.AddMoney(-ShipLevel.dam_cost_to_next_level[shipKey][cDL]);
        cDL++;
        saveData.SetShipLevelValue(shipKey, "damage", cDL);
        PointToPlayer.Instance.GetPlayerShipValues().GetComponent<ShipValueControl>().UpdateShipValues();
        UpdateLevelButtons();
    }
    public void spe_clicked()
    {
        if (cSL >= (ShipLevel.spe_level_effect[shipKey].Count - 1)) return;
        if (saveData.GetMoney() < ShipLevel.spe_cost_to_next_level[shipKey][cSL]) return;
        PlayerMoneyController.Instance.AddMoney(-ShipLevel.spe_cost_to_next_level[shipKey][cSL]);
        cSL++;
        saveData.SetShipLevelValue(shipKey, "speed", cSL);
        PointToPlayer.Instance.GetPlayerShipValues().GetComponent<ShipValueControl>().UpdateShipValues();
        UpdateLevelButtons();
    }
    public void InitButtonsSize()
    {
        float wantedFullWidth = buttonParent.rect.width;
        float wantedFullHeight = buttonParent.rect.height;
        float bodyWidth = Screen.width;// body.rect.width;
        float bodyHeight = Screen.height;// body.rect.height;
        float wantedRatio = wantedFullWidth / wantedFullHeight;
        float bodyRatio = bodyWidth / bodyHeight;
        if(wantedRatio > bodyRatio)                     //align to width
        {
            float mult = bodyWidth / wantedFullWidth;
            buttonParent.localScale = new Vector2(mult, mult);
        }
        else
        {                                               //align to height
            float mult = bodyHeight / wantedFullHeight;
            buttonParent.localScale = new Vector2(mult, mult);
        }

    }
}
