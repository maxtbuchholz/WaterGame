using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCanvas : MonoBehaviour
{
    [SerializeField] RectTransform mapFrame;
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
        mapFrame.anchoredPosition = currentPos;
        mapFrame.sizeDelta = new Vector2(200, screenHeight * 0.9f);
    }
    public void MoveToFrame()
    {
        StartCoroutine(MoveToCenter(0.2f));
    }
    public void MoveToHide()
    {
        StartCoroutine(MoveToShrink(0.2f));
    }
    IEnumerator MoveToCenter(float timeToCenter)
    {
        float currentTime = 0;
        float finalHeight = 0;
        float initialHeight = mapFrame.anchoredPosition.y;
        float heightDiff = -initialHeight;
        while (currentTime < timeToCenter)
        {
            currentTime += Time.deltaTime;
            if (currentTime > timeToCenter) currentTime = timeToCenter;
            float setHeight = initialHeight + (heightDiff * (currentTime / timeToCenter));
            currentPos = new Vector2(0, setHeight);
            mapFrame.anchoredPosition = currentPos;
            yield return null;
        }
        currentPos = Vector2.zero;
        mapFrame.anchoredPosition = currentPos;
        StartCoroutine(MoveToExpand(0.2f));
    }
    IEnumerator MoveToExpand(float timeToCenter)
    {
        float currentTime = 0;
        float finalWidth = (screenWidth * 0.9f);
        float initialSize = mapFrame.rect.size.x;
        float widthDiff = (finalWidth - initialSize);
        while (currentTime < timeToCenter)
        {
            currentTime += Time.deltaTime;
            if (currentTime > timeToCenter) currentTime = timeToCenter;
            float setSize = initialSize + (widthDiff * (currentTime / timeToCenter));
            mapFrame.sizeDelta = new Vector2(setSize, mapFrame.rect.height);
            yield return null;
        }
        mapFrame.sizeDelta = new Vector2(finalWidth, mapFrame.rect.height);
    }
    IEnumerator MoveToBottom(float timeToCenter)
    {
        float currentTime = 0;
        float finalHeight = -screenHeight;
        float initialHeight = mapFrame.anchoredPosition.y;
        float heightDiff = finalHeight;
        while (currentTime < timeToCenter)
        {
            currentTime += Time.deltaTime;
            if (currentTime > timeToCenter) currentTime = timeToCenter;
            float setHeight = initialHeight + (heightDiff * (currentTime / timeToCenter));
            currentPos = new Vector2(0, setHeight);
            mapFrame.anchoredPosition = currentPos;
            yield return null;
        }
        currentPos = new Vector2(0, finalHeight);
        mapFrame.anchoredPosition = currentPos;
    }
    IEnumerator MoveToShrink(float timeToCenter)
    {
        float currentTime = 0;
        float finalWidth = 200;
        float initialSize = mapFrame.rect.size.x;
        float widthDiff = (finalWidth - initialSize);
        while (currentTime < timeToCenter)
        {
            currentTime += Time.deltaTime;
            if (currentTime > timeToCenter) currentTime = timeToCenter;
            float setSize = initialSize + (widthDiff * (currentTime / timeToCenter));
            mapFrame.sizeDelta = new Vector2(setSize, mapFrame.rect.height);
            yield return null;
        }
        mapFrame.sizeDelta = new Vector2(200, mapFrame.rect.height);
        StartCoroutine(MoveToBottom(0.2f));
    }
}
