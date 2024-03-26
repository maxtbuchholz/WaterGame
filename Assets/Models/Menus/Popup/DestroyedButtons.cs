using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedButtons : MonoBehaviour
{
    public void NewGameButtonClicked()
    {
        PlayerHealthBar.Instance.NewGameButtonPressed();
    }
    public void RespawnButtonClicked()
    {
        PlayerHealthBar.Instance.RespawnButtonPressed();
    }
}
