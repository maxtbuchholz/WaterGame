using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FortUpgradeButtons : MonoBehaviour
{
    [SerializeField] RectTransform buttonParent;
    [SerializeField] TextMeshProUGUI health_level_text;
    [SerializeField] TextMeshProUGUI damage_level_text;
    [SerializeField] TextMeshProUGUI mortar_level_text;
    [SerializeField] TextMeshProUGUI health_price_text;
    [SerializeField] TextMeshProUGUI damage_price_text;
    [SerializeField] TextMeshProUGUI mortar_price_text;
    [SerializeField] Button health_button;
    [SerializeField] Button damage_button;
    [SerializeField] Button mortar_button;
    //private RectTransform body;
    private void Start()
    {
        //body = GetComponent<RectTransform>();
        InitButtonsSize();
    }
    private SaveData saveData;
    private string fortKey;
    private FortSaveLevel fSL;
    public void SetFortKey(string key)
    {
        fortKey = key;
        saveData = SaveData.Instance;
        fSL = saveData.GetFortLevels(fortKey);
        if (fSL != null)
        {
            UpdateLevelButtons();
        }
    }
    private void UpdateLevelButtons()
    {
        if (fSL == null) return;
        health_level_text.text = fSL.hea_level + " / " + (FortLevel.hea_level_effect.Count - 1);
        damage_level_text.text = fSL.dam_level + " / " + (FortLevel.dam_level_effect.Count - 1);
        mortar_level_text.text = fSL.mor_level + " / " + (FortLevel.mor_level_effect.Count - 1);
        if (fSL.hea_level < (FortLevel.hea_level_effect.Count - 1))
        {
            health_level_text.color = Color.black;
            health_button.interactable = true;
            health_price_text.text = FortLevel.hea_cost_to_next_level[fSL.hea_level].ToString();
            if (saveData.GetMoney() >= FortLevel.hea_cost_to_next_level[fSL.hea_level])
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
        if (fSL.dam_level < (FortLevel.dam_level_effect.Count - 1))
        {
            damage_level_text.color = Color.black;
            damage_button.interactable = true;
            damage_price_text.text = FortLevel.dam_cost_to_next_level[fSL.dam_level].ToString();
            if (saveData.GetMoney() >= FortLevel.dam_cost_to_next_level[fSL.dam_level])
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
        if (fSL.mor_level < (FortLevel.mor_level_effect.Count - 1))
        {
            mortar_level_text.color = Color.black;
            mortar_button.interactable = true;
            mortar_price_text.text = FortLevel.mor_cost_to_next_level[fSL.mor_level].ToString();
            if(saveData.GetMoney() >= FortLevel.mor_cost_to_next_level[fSL.mor_level])
            {
                mortar_price_text.color = Color.green;
            }
            else
                mortar_price_text.color = Color.red;
        }
        else
        {
            mortar_button.interactable = false;
            mortar_price_text.text = "-----";
            mortar_level_text.color = Color.gray;
            mortar_price_text.color = Color.gray;
        }
    }
    public void hea_clicked()
    {
        if (fSL.hea_level >= (FortLevel.hea_level_effect.Count - 1)) return;
        if (saveData.GetMoney() < FortLevel.hea_cost_to_next_level[fSL.hea_level]) return;
        PlayerMoneyController.Instance.AddMoney(-FortLevel.hea_cost_to_next_level[fSL.hea_level]);
        fSL.hea_level++;
        saveData.SetFortLevels(fortKey, fSL);
        UpdateFortObject();
        UpdateLevelButtons();
    }
    public void dam_clicked()
    {
        if (fSL.dam_level >= (FortLevel.dam_level_effect.Count - 1)) return;
        if (saveData.GetMoney() < FortLevel.dam_cost_to_next_level[fSL.dam_level]) return;
        PlayerMoneyController.Instance.AddMoney(-FortLevel.dam_cost_to_next_level[fSL.dam_level]);
        fSL.dam_level++;
        saveData.SetFortLevels(fortKey, fSL);
        UpdateFortObject();
        UpdateLevelButtons();
    }
    public void mor_clicked()
    {
        if (fSL.mor_level >= (FortLevel.mor_level_effect.Count - 1)) return;
        if (saveData.GetMoney() < FortLevel.mor_cost_to_next_level[fSL.mor_level]) return;
        PlayerMoneyController.Instance.AddMoney(-FortLevel.mor_cost_to_next_level[fSL.mor_level]);
        fSL.mor_level++;
        saveData.SetFortLevels(fortKey, fSL);
        UpdateFortObject();
        UpdateLevelButtons();
    }
    private void UpdateFortObject()
    {
        LoadedObjects.Instance.GetLoadedFort(fortKey).GetComponent<FortLevel>().LevelsUpdate();
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
