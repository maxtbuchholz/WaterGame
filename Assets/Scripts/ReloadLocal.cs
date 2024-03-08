using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadLocal : MonoBehaviour
{
    [SerializeField] Transform mask;
    [SerializeField] Transform inner;
    private void Start()
    {
        height = GetComponent<RectTransform>().rect.height;
    }
    private float height = 0;
    private bool previouslyUnloaded = false;
    public void SetReladPer(float percent)
    {
        mask.GetComponent<RectTransform>().localPosition = new Vector2(0, -percent * height);
        inner.GetComponent<RectTransform>().localPosition = new Vector2(0, percent * height);
        if((percent > 0) && (!previouslyUnloaded))                //set to unloaded
        {
            previouslyUnloaded = true;
            inner.GetComponent<Image>().color = Color.red;
        }
        else if ((percent == 0) && (previouslyUnloaded))                //set to loaded
        {
            previouslyUnloaded = false;
            inner.GetComponent<Image>().color = Color.white;
        }
    }
}
