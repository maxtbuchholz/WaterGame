using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyTextInit : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI totalText;
    [SerializeField] TextMeshProUGUI changeText;
    // Start is called before the first frame update
    void Start()
    {
        PlayerMoneyController.Instance.AddMoneyTextListener(totalText, changeText);
    }
}
