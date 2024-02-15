using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    private void Start()
    {
        Application.targetFrameRate = 60;
    }
    void Update()
    {
        text.text = ((int)(1f / Time.unscaledDeltaTime)).ToString();
    }
}
