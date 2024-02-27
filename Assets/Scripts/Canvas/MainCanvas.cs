using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    public void DisableCanvas()
    {
        if (ButtonCollisionTracker.Instance.GetTypicalButtonBlocked()) return;
        canvas.SetActive(false);
    }
    public void EnableCanvas()
    {
        if (ButtonCollisionTracker.Instance.GetTypicalButtonBlocked()) return;
        canvas.SetActive(true);
    }
}
