using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PupupBlockClick : MonoBehaviour
{
    void Start()
    {
        ButtonCollisionTracker.Instance.AddUIButton(GetComponent<RectTransform>(), 10);
    }
}
