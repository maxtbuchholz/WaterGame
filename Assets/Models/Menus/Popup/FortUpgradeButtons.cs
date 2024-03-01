using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortUpgradeButtons : MonoBehaviour
{
    [SerializeField] RectTransform buttonParent;
    private RectTransform body;
    private void Start()
    {
        body = GetComponent<RectTransform>();
        InitButtonsSize();
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
