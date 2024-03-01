using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompasUpdate : MonoBehaviour
{
    [SerializeField] NavTouch navTouch;
    public void UpdateCompas(float dir)
    {
        transform.rotation = Quaternion.Euler(0, 0, dir);
    }
    public void CompasClicked()
    {
        navTouch.RotateToNorth();
    }
}
