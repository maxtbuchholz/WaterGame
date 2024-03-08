using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdradeShipCanvas : MonoBehaviour
{
    [SerializeField] RectTransform shipFrame;
    [SerializeField] RectTransform buttonFrame;
    [SerializeField] ShipUpgradeButtons shipUpgradeButtons;
    int screenHeight;
    int screenWidth;
    Vector2 currentPos;
    void Start()
    {
        screenHeight = Screen.height;
        screenWidth = Screen.width;
        //Rect rect = mapFrame.rect;
        //rect.position = new Vector2(0, -screenHeight);
        currentPos = new Vector2(0, -screenHeight);
        shipFrame.anchoredPosition = currentPos;
        shipFrame.sizeDelta = new Vector2(100, screenHeight * 0.8f);
        buttonFrame.anchoredPosition = currentPos;
        buttonFrame.sizeDelta = new Vector2(100, screenHeight * 0.8f);
    }
    public void MoveToFrame()
    {
        StartCoroutine(MoveToCenter(0.2f));
        shipUpgradeButtons.SetShipKey();
    }
    public void MoveToHide()
    {
        StartCoroutine(MoveToShrink(0.2f));
    }
    IEnumerator MoveToCenter(float timeToCenter)
    {
        float currentTime = 0;
        float finalHeight = 0;
        float initialHeight = shipFrame.anchoredPosition.y;
        float heightDiff = -initialHeight;
        while (currentTime < timeToCenter)
        {
            currentTime += Time.deltaTime;
            if (currentTime > timeToCenter) currentTime = timeToCenter;
            float setHeight = initialHeight + (heightDiff * (currentTime / timeToCenter));
            currentPos = new Vector2(0, setHeight);
            shipFrame.anchoredPosition = currentPos;
            buttonFrame.anchoredPosition = currentPos;
            yield return null;
        }
        currentPos = Vector2.zero;
        shipFrame.anchoredPosition = currentPos;
        buttonFrame.anchoredPosition = currentPos;
        StartCoroutine(MoveToExpand(0.2f));
    }
    IEnumerator MoveToExpand(float timeToCenter)
    {
        float currentTime = 0;
        float finalWidth = (screenWidth * 0.8f);
        float initialSize = shipFrame.rect.size.x;
        float widthDiff = (finalWidth - initialSize);
        while (currentTime < timeToCenter)
        {
            currentTime += Time.deltaTime;
            if (currentTime > timeToCenter) currentTime = timeToCenter;
            float setSize = initialSize + (widthDiff * (currentTime / timeToCenter));
            shipFrame.sizeDelta = new Vector2(setSize, shipFrame.rect.height);
            buttonFrame.sizeDelta = new Vector2(setSize, shipFrame.rect.height);
            yield return null;
        }
        shipFrame.sizeDelta = new Vector2(finalWidth, shipFrame.rect.height);
        buttonFrame.sizeDelta = new Vector2(finalWidth, shipFrame.rect.height);
    }
    IEnumerator MoveToBottom(float timeToCenter)
    {
        float currentTime = 0;
        float finalHeight = -screenHeight;
        float initialHeight = shipFrame.anchoredPosition.y;
        float heightDiff = finalHeight;
        while (currentTime < timeToCenter)
        {
            currentTime += Time.deltaTime;
            if (currentTime > timeToCenter) currentTime = timeToCenter;
            float setHeight = initialHeight + (heightDiff * (currentTime / timeToCenter));
            currentPos = new Vector2(0, setHeight);
            shipFrame.anchoredPosition = currentPos;
            buttonFrame.anchoredPosition = currentPos;
            yield return null;
        }
        currentPos = new Vector2(0, finalHeight);
        shipFrame.anchoredPosition = currentPos;
        buttonFrame.anchoredPosition = currentPos;
    }
    IEnumerator MoveToShrink(float timeToCenter)
    {
        float currentTime = 0;
        float finalWidth = 100;
        float initialSize = shipFrame.rect.size.x;
        float widthDiff = (finalWidth - initialSize);
        while (currentTime < timeToCenter)
        {
            currentTime += Time.deltaTime;
            if (currentTime > timeToCenter) currentTime = timeToCenter;
            float setSize = initialSize + (widthDiff * (currentTime / timeToCenter));
            shipFrame.sizeDelta = new Vector2(setSize, shipFrame.rect.height);
            buttonFrame.sizeDelta = new Vector2(setSize, shipFrame.rect.height);
            yield return null;
        }
        shipFrame.sizeDelta = new Vector2(100, shipFrame.rect.height);
        StartCoroutine(MoveToBottom(0.2f));
    }
}
