using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BinaryPopupResize : MonoBehaviour
{
    [SerializeField] RectTransform frame;
    [SerializeField] RectTransform cancelRect;
    [SerializeField] RectTransform okRect;
    [SerializeField] TextMeshProUGUI textMesh;
    void Start()
    {
        float setBySize = Mathf.Min(Screen.width, Screen.height);
        float buttonWidth = setBySize / 4;
        frame.sizeDelta = new Vector2(setBySize / 1.5f, setBySize / 2);
        okRect.sizeDelta = new Vector2(buttonWidth, buttonWidth * (okRect.rect.height / okRect.rect.width));
        cancelRect.sizeDelta = new Vector2(buttonWidth, buttonWidth * (cancelRect.rect.height / cancelRect.rect.width));
        okRect.anchoredPosition = new Vector2(-(buttonWidth / 1.5f),okRect.anchoredPosition.y);
        cancelRect.anchoredPosition = new Vector2(buttonWidth / 1.5f, cancelRect.anchoredPosition.y);
    }
    public void SetKey(int key)
    {
        this.key = key;
    }
    public void SetText(string text)
    {
        textMesh.text = text;
    }
    private int key = -1;
    public void OKCLicked()
    {
        PopupManager.Instance.SignalEndBinary(key, true);
    }
    public void CancelCLicked()
    {
        PopupManager.Instance.SignalEndBinary(key, false);
    }
}
