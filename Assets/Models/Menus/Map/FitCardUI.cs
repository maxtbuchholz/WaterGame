using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitCardUI : MonoBehaviour
{
    [SerializeField] CardType cardType;
    void Start()
    {
        if(TryGetComponent<RectTransform>(out RectTransform rectTransform))
        {
            if (cardType == CardType.middle)
                FitCardMiddle(rectTransform);
            else if (cardType == CardType.right)
                FitCardRight(rectTransform);
            else if (cardType == CardType.left)
                FitCardLeft(rectTransform);
        }
    }
    private void FitCardMiddle(RectTransform rectTransform)
    {
        int screenHeight = Screen.height;
        int topBotPadding = (screenHeight / 10);
        rectTransform.offsetMin = new Vector2(2 * topBotPadding, topBotPadding);
        rectTransform.offsetMax = new Vector2(-2 * topBotPadding, -topBotPadding);
    }
    private void FitCardRight(RectTransform rectTransform)
    {
        int screenHeight = Screen.height;
        int topBotPadding = (screenHeight / 10);
        int width = (Screen.height - (2 * topBotPadding)) / 2;
        int smallerTopBotPadding = topBotPadding / 2;
        rectTransform.offsetMin = new Vector2(-width- smallerTopBotPadding, topBotPadding);
        rectTransform.offsetMax = new Vector2(-smallerTopBotPadding, -topBotPadding);
    }
    private void FitCardLeft(RectTransform rectTransform)
    {
        int screenHeight = Screen.height;
        int topBotPadding = (screenHeight / 10);
        int width = (Screen.height - (2 * topBotPadding)) / 2;
        int smallerTopBotPadding = topBotPadding / 2;
        rectTransform.offsetMin = new Vector2(smallerTopBotPadding, topBotPadding);
        rectTransform.offsetMax = new Vector2(width + smallerTopBotPadding, -topBotPadding);
    }
    enum CardType
    {
        middle,
        right,
        left
    }
}
