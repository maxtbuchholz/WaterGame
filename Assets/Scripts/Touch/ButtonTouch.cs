using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTouch : MonoBehaviour
{
    void Start()
    {
        ButtonCollisionTracker.Instance.AddUIButton(GetComponent<RectTransform>(), 0);
    }
}
