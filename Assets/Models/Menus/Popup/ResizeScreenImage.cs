using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityFigmaBridge.Runtime.UI;

public class ResizeScreenImage : MonoBehaviour
{
    [SerializeField] Transform loadingImage;
    [SerializeField] RawImage backImage;
    [SerializeField] FigmaImage loadingCircle;
    public void ResizeImage()
    {
        RectTransform lIRect = loadingImage.GetComponent<RectTransform>();
        Vector2 startSize = lIRect.rect.size;
        Vector2 canvasSize = GetComponent<RectTransform>().rect.size;

        float horFitMul = canvasSize.x / startSize.x;
        float amountHorOffScreen = ((horFitMul * startSize.y) - canvasSize.y) * canvasSize.x;
        float verFitMul = canvasSize.y / startSize.y;
        float amountVerOffScreen = ((verFitMul * startSize.x) - canvasSize.x) * canvasSize.y;
        bool chooseHor = true;
        if (amountHorOffScreen < 0)
            chooseHor = false;
        else if (amountVerOffScreen < 0)
            chooseHor = true;
        else if (amountHorOffScreen < amountVerOffScreen)
            chooseHor = true;
        else
            chooseHor = false;
        if (chooseHor)
        {
            lIRect.sizeDelta = new Vector2(horFitMul * startSize.x, horFitMul * startSize.y);
        }
        else
        {
            lIRect.sizeDelta = new Vector2(verFitMul * startSize.x, verFitMul * startSize.y);
        }
    }
    public void SetOpacity(float opacity)
    {
        Color backColor = backImage.color;
        backColor.a = opacity;
        backImage.color = backColor;
        Color circleColor = loadingCircle.color;
        circleColor.a = opacity;
        loadingCircle.color = circleColor;
    }
}
