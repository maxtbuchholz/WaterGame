using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    private bool canvasEnabled = true;
    public void DisableCanvas()
    {
        canvasEnabled = false;
        if (ButtonCollisionTracker.Instance.GetTypicalButtonBlocked()) return;
        canvas.SetActive(false);
    }
    public void EnableCanvas()
    {
        canvasEnabled = true;
        if (ButtonCollisionTracker.Instance.GetTypicalButtonBlocked()) return;
        canvas.SetActive(true);
    }
    public bool GetCanvasEnabled()
    {
        return canvasEnabled;
    }
}
