using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivacyPolicyButton : MonoBehaviour
{
    public void PrivacyButtonClicked()
    {
        Application.OpenURL("https://sites.google.com/view/blunder-boats-privacy-policy/home");
    }
}
