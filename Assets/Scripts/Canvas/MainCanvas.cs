using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    public void DisableCanvas()
    {
        canvas.SetActive(false);
    }
    public void EnableCanvas()
    {
        canvas.SetActive(true);
    }
}
