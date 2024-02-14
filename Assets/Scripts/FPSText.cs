using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text; 
    void Update()
    {
        text.text = ((int)(1f / Time.unscaledDeltaTime)).ToString();
    }
}
