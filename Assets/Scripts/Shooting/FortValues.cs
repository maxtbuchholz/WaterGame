using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortValues : MonoBehaviour
{
    [HideInInspector] public List<GameObject> fortParts;
    void Start()
    {
        fortParts = new();
        AddChildrenToFortParts(gameObject);
    }
    private void AddChildrenToFortParts(GameObject go)
    {
        fortParts.Add(go);
        for (int i = 0; i < go.transform.childCount; i++)
            AddChildrenToFortParts(go.transform.GetChild(i).gameObject);
    }
}
