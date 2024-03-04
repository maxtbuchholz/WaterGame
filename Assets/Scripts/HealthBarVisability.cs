using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarVisability : MonoBehaviour
{
    [SerializeField] List<Transform> healthBarObjects;
    public void SetAppear(bool appear)
    {
        if (appear)
        {
            foreach(Transform on in healthBarObjects)
            {
                if(on.TryGetComponent<SpriteRenderer>(out SpriteRenderer sR))
                {
                    sR.enabled = true;
                }
            }
        }
        else
        {
            foreach (Transform on in healthBarObjects)
            {
                if (on.TryGetComponent<SpriteRenderer>(out SpriteRenderer sR))
                {
                    sR.enabled = false;
                }
            }
        }
    }
}
