using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeBox : MonoBehaviour
{
    [SerializeField] RectTransform outer;
    [SerializeField] RectTransform inner;
    [SerializeField] bool horizontal = true;
    [SerializeField] bool fit = true;
    [SerializeField] float innerWidth = 0;
    [SerializeField] float innerHeight = 0;
    bool resized = false;
    public void Resize(float finalWidth)
    {
        if (resized) return;
        resized = true;
        if (horizontal)
        {
            float perLar = finalWidth / innerWidth;// inner.rect.width;
            inner.gameObject.transform.localScale *= perLar;
            inner.sizeDelta = new Vector2(innerWidth - 100, innerHeight - 100);
        }
        else if (fit)
        {
            float perLar = Screen.width * 0.8f / innerWidth;// inner.rect.width;
            float perHei = Screen.height * 0.8f / innerHeight;// inner.rect.height;
            if(perHei < perLar)
            {
                inner.gameObject.transform.localScale *= perHei;
            }
            else
            {
                inner.gameObject.transform.localScale *= perLar;
            }
        }
    }
}
