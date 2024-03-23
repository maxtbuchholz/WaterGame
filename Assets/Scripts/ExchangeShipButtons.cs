using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExchangeShipButtons : MonoBehaviour
{
    [SerializeField] ShipUpgradeButtons shipUpgradeButtons;
    [SerializeField] Button a_button;
    [SerializeField] Button b_button;
    [SerializeField] Button c_button;
    [SerializeField] TextMeshProUGUI a_price;
    [SerializeField] TextMeshProUGUI b_price;
    [SerializeField] TextMeshProUGUI c_price;
    [SerializeField] Image a_image;
    [SerializeField] Image b_image;
    [SerializeField] Image c_image;
    SaveData saveData;
    public void Start()
    {
        saveData = SaveData.Instance;
        GetComponent<ResizeBox>().Resize(Screen.width * 0.8f);
        InitButtons();
    }
    public void InitButtons()
    {
        float currentShip = saveData.GetCurrentShipId();
        Color unlokedColor = new Color(1, 0.843f, 0);
        if(currentShip == 0)
        {
            a_image.color = unlokedColor;
            b_image.color = Color.gray;
            c_image.color = Color.gray;
        }
        else if(currentShip == 1)
        {
            a_image.color = Color.gray;
            b_image.color = unlokedColor;
            c_image.color = Color.gray;
        }
        else if (currentShip == 2)
        {
            a_image.color = Color.gray;
            b_image.color = Color.gray;
            c_image.color = unlokedColor;
        }
        float currentMoney = saveData.GetMoney();
        bool a_unlocked = true;
        bool b_unlocked = saveData.GetShipUnlocked(1);
        bool c_unlocked = saveData.GetShipUnlocked(2);
        if (a_unlocked)
        {
            a_price.text = "---";
            a_price.color = Color.gray;
            a_button.interactable = true;
        }
        else
        {
            a_price.text = SaveData.moneySymbol + " " +  shipPrices[0].ToString();
            if (currentMoney >= shipPrices[0])
            {
                a_price.color = Color.green;
                a_button.interactable = true;
            }
            else
            {
                a_price.color = Color.red;
                a_button.interactable = false;
            }
        }
        if (b_unlocked)
        {
            b_price.text = "---";
            b_price.color = Color.gray;
            b_button.interactable = true;
        }
        else
        {
            b_price.text = SaveData.moneySymbol + " " + shipPrices[1].ToString();
            if (currentMoney >= shipPrices[1])
            {
                b_price.color = Color.green;
                b_button.interactable = true;
            }
            else
            {
                b_price.color = Color.red;
                b_button.interactable = false;
            }
        }
        if (c_unlocked)
        {
            c_price.text = "---";
            c_price.color = Color.gray;
            c_button.interactable = true;
        }
        else
        {
            c_price.text = SaveData.moneySymbol + " " + shipPrices[2].ToString();
            if (currentMoney >= shipPrices[2])
            {
                c_price.color = Color.green;
                c_button.interactable = true;
            }
            else
            {
                c_price.color = Color.red;
                c_button.interactable = false;
            }
        }
    }
    public void a_Clicked()
    {
        //if (!saveData.GetShipUnlocked(0))
        //{
        //    saveData.SetShipUnlocked(0, true);
        //    PlayerMoneyController.Instance.AddMoney(-shipPrices[0]);
        //}
        SaveData.Instance.SetCurrentShipId(0);
        PointToPlayer.Instance.GetPlayerShipValues().GetComponent<ShipValueControl>().ShipModelChanged();
        shipUpgradeButtons.SetShipKey();
        shipUpgradeButtons.UpdateLevelButtons();
        InitButtons();
    }
    public void b_Clicked()
    {
        if (!saveData.GetShipUnlocked(1))
        {
            saveData.SetShipUnlocked(1, true);
            PlayerMoneyController.Instance.AddMoney(-shipPrices[1]);
        }
        SaveData.Instance.SetCurrentShipId(1);
        PointToPlayer.Instance.GetPlayerShipValues().GetComponent<ShipValueControl>().ShipModelChanged();
        shipUpgradeButtons.SetShipKey();
        shipUpgradeButtons.UpdateLevelButtons();
        InitButtons();
    }
    public void c_Clicked()
    {
        if (!saveData.GetShipUnlocked(2))
        {
            saveData.SetShipUnlocked(2, true);
            PlayerMoneyController.Instance.AddMoney(-shipPrices[2]);
        }
        SaveData.Instance.SetCurrentShipId(2);
        PointToPlayer.Instance.GetPlayerShipValues().GetComponent<ShipValueControl>().ShipModelChanged();
        shipUpgradeButtons.SetShipKey();
        shipUpgradeButtons.UpdateLevelButtons();
        InitButtons();
    }
    public static Dictionary<int, float> shipPrices = new()
    {
        {0, 100 },
        {1, 250 },
        {2, 800 }
    };
}
