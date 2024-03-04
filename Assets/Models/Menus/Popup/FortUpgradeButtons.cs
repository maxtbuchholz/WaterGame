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
            health_button.interactable = true;
        else
            health_button.interactable = false;
        if (fSL.dam_level < (FortLevel.dam_level_effect.Count - 1))
            damage_button.interactable = true;
        else
            damage_button.interactable = false;
        if (fSL.mor_level < (FortLevel.mor_level_effect.Count - 1))
            mortar_button.interactable = true;
        else
            mortar_button.interactable = false;
    }
    public void hea_clicked()
    {
        if (fSL.hea_level >= (FortLevel.hea_level_effect.Count - 1)) return;
        fSL.hea_level++;
        saveData.SetFortLevels(fortKey, fSL);
        UpdateFortObject();
        UpdateLevelButtons();
    }
    public void dam_clicked()
    {
        if (fSL.dam_level >= (FortLevel.dam_level_effect.Count - 1)) return;
        fSL.dam_level++;
        saveData.SetFortLevels(fortKey, fSL);
        UpdateFortObject();
        UpdateLevelButtons();
    }
    public void mor_clicked()
    {
        if (fSL.mor_level >= (FortLevel.mor_level_effect.Count - 1)) return;
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
