using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityFigmaBridge.Runtime.UI;

public class HealthBarVisability : MonoBehaviour
{
    [SerializeField] List<Transform> healthBarObjects;
    [SerializeField] GameObject moveBar;
    public void SetAppear(bool appear)
    {
        if (appear)
        {
            foreach(Transform on in healthBarObjects)
            {
                //if(on.TryGetComponent<SpriteRenderer>(out SpriteRenderer sR))
                //{
                //    sR.enabled = true;
                //}
                if (on.TryGetComponent<FigmaImage>(out FigmaImage fI))
                {
                    fI.enabled = true;
                }
                if (on.TryGetComponent<Image>(out Image iM))
                {
                    iM.enabled = true;
                }
            }
        }
        else
        {
            foreach (Transform on in healthBarObjects)
            {
                //if (on.TryGetComponent<SpriteRenderer>(out SpriteRenderer sR))
                //{
                //    sR.enabled = false;
                //}
                if (on.TryGetComponent<FigmaImage>(out FigmaImage fI))
                {
                    fI.enabled = false;
                }
                if (on.TryGetComponent<Image>(out Image iM))
                {
                    iM.enabled = false;
                }
            }
        }
    }
    public Color GetBarColor()
    {
        return moveBar.GetComponent<Image>().color;
    }
    public void SetBarColor(Color color)
    {
        Color oldColor = moveBar.GetComponent<Image>().color;
        moveBar.GetComponent<Image>().color = new Color(oldColor.r, oldColor.g, oldColor.b, color.a);
    }
    public GameObject GetMoveBar()
    {
        return moveBar;
    }
}
