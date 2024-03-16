using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMoneyController : MonoBehaviour
{
    SaveData saveData;
    private static PlayerMoneyController instance;
    public static PlayerMoneyController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindAnyObjectByType<PlayerMoneyController>();
            }
            return instance;
        }
    }
    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if(timeSinceChange < maxTimeChangeOnScreen)
        {
            timeSinceChange += Time.deltaTime;
            if(timeSinceChange < timeTillChangeDiss)
            {
                foreach (TextMeshProUGUI cha in textChange)
                {
                    Color chaColor = cha.color;
                    chaColor.a = 1; 
                    cha.color = chaColor;
                }
            }
            else
            {
                float high = maxTimeChangeOnScreen - (maxTimeChangeOnScreen - timeTillChangeDiss);
                float l = timeSinceChange - timeTillChangeDiss;
                l /= high;
                if (l < 0.1) l = 0;
                l = 1 - l;
                foreach (TextMeshProUGUI cha in textChange)
                {
                    Color chaColor = cha.color;
                    chaColor.a = l;
                    cha.color = chaColor;
                }
            }
        }
    }
    private float currentChange = 0;
    public float GetCurrMoney()
    {
        return saveData.GetMoney();
    }
    static float maxTimeChangeOnScreen = 2;
    static float timeTillChangeDiss = 1;
    float timeSinceChange = 2;
    public void AddMoney(float change)
    {
        saveData.AddMoney(change);
        currentChange = change;
        timeSinceChange = 0;
        foreach (TextMeshProUGUI tot in textTotal)
        {
            if(tot != null)
                tot.text = SaveData.moneySymbol + " " + saveData.GetMoney().ToString();
        }
        foreach (TextMeshProUGUI cha in textChange)
        {
            if (cha != null)
            {
                cha.text = change.ToString();
                if (change < 0)
                {
                    cha.text = "-" + SaveData.moneySymbol + Mathf.Abs(change).ToString();
                    cha.color = Color.red;
                }
                else
                {
                    cha.text = "+"+ SaveData.moneySymbol +  change.ToString();
                    cha.color = Color.green;
                }
            }
        }
    }
    void Start()
    {
        saveData = SaveData.Instance;
    }
    public List<TextMeshProUGUI> textTotal = new();
    public List<TextMeshProUGUI> textChange = new();
    public void AddMoneyTextListener(TextMeshProUGUI total, TextMeshProUGUI change)
    {
        if ((total == null) || change == null) return;
        if (saveData == null) saveData = SaveData.Instance;
        total.text = SaveData.moneySymbol + " " + saveData.GetMoney().ToString();
        change.text = "";
        if (!textTotal.Contains(total))
            textTotal.Add(total);
        if (!textChange.Contains(change))
            textChange.Add(change);
    }
}
