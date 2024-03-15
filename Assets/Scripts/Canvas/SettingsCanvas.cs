using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsCanvas : MonoBehaviour
{
    [SerializeField] RectTransform settingsFrame;
    [SerializeField] RectTransform buttonFrame;
    [SerializeField] RectTransform scroll;
    [SerializeField] SettingsColorButtons settingsColorButtons;
    [SerializeField] ResizeBox resizeBox;
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
        settingsFrame.anchoredPosition = currentPos;
        settingsFrame.sizeDelta = new Vector2(100, screenHeight * 0.8f);
        buttonFrame.anchoredPosition = currentPos;
        buttonFrame.sizeDelta = new Vector2(100, screenHeight * 0.8f);
        //scroll.offsetMax = Vector2.zero;
        //scroll.offsetMin = Vector2.zero;
    }
    public void MoveToFrame()
    {
        settingsColorButtons.InitColors();
        PlayerPrefsController.Instance.InitSliders();
        resizeBox.Resize(Screen.width * 0.8f);
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
        float initialHeight = settingsFrame.anchoredPosition.y;
        float heightDiff = -initialHeight;
        while (currentTime < timeToCenter)
        {
            currentTime += Time.deltaTime;
            if (currentTime > timeToCenter) currentTime = timeToCenter;
            float setHeight = initialHeight + (heightDiff * (currentTime / timeToCenter));
            currentPos = new Vector2(0, setHeight);
            settingsFrame.anchoredPosition = currentPos;
            buttonFrame.anchoredPosition = currentPos;
            yield return null;
        }
        currentPos = Vector2.zero;
        settingsFrame.anchoredPosition = currentPos;
        buttonFrame.anchoredPosition = currentPos;
        StartCoroutine(MoveToExpand(0.2f));
    }
    IEnumerator MoveToExpand(float timeToCenter)
    {
        float currentTime = 0;
        float finalWidth = (screenWidth * 0.8f);
        float initialSize = settingsFrame.rect.size.x;
        float widthDiff = (finalWidth - initialSize);
        while (currentTime < timeToCenter)
        {
            currentTime += Time.deltaTime;
            if (currentTime > timeToCenter) currentTime = timeToCenter;
            float setSize = initialSize + (widthDiff * (currentTime / timeToCenter));
            settingsFrame.sizeDelta = new Vector2(setSize, settingsFrame.rect.height);
            buttonFrame.sizeDelta = new Vector2(setSize, settingsFrame.rect.height);
            yield return null;
        }
        settingsFrame.sizeDelta = new Vector2(finalWidth, settingsFrame.rect.height);
        buttonFrame.sizeDelta = new Vector2(finalWidth, settingsFrame.rect.height);
        //scroll.offsetMax = Vector2.zero;
        //scroll.offsetMin = Vector2.zero;
    }
    IEnumerator MoveToBottom(float timeToCenter)
    {
        float currentTime = 0;
        float finalHeight = -screenHeight;
        float initialHeight = settingsFrame.anchoredPosition.y;
        float heightDiff = finalHeight;
        while (currentTime < timeToCenter)
        {
            currentTime += Time.deltaTime;
            if (currentTime > timeToCenter) currentTime = timeToCenter;
            float setHeight = initialHeight + (heightDiff * (currentTime / timeToCenter));
            currentPos = new Vector2(0, setHeight);
            settingsFrame.anchoredPosition = currentPos;
            buttonFrame.anchoredPosition = currentPos;
            yield return null;
        }
        currentPos = new Vector2(0, finalHeight);
        settingsFrame.anchoredPosition = currentPos;
        buttonFrame.anchoredPosition = currentPos;
    }
    IEnumerator MoveToShrink(float timeToCenter)
    {
        float currentTime = 0;
        float finalWidth = 100;
        float initialSize = settingsFrame.rect.size.x;
        float widthDiff = (finalWidth - initialSize);
        while (currentTime < timeToCenter)
        {
            currentTime += Time.deltaTime;
            if (currentTime > timeToCenter) currentTime = timeToCenter;
            float setSize = initialSize + (widthDiff * (currentTime / timeToCenter));
            settingsFrame.sizeDelta = new Vector2(setSize, settingsFrame.rect.height);
            buttonFrame.sizeDelta = new Vector2(setSize, settingsFrame.rect.height);
            yield return null;
        }
        settingsFrame.sizeDelta = new Vector2(100, settingsFrame.rect.height);
        StartCoroutine(MoveToBottom(0.2f));
        //scroll.offsetMax = Vector2.zero;
        //scroll.offsetMin = Vector2.zero;
    }
}
