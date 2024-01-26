using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskObject : MonoBehaviour
{
    [SerializeField] GameObject[] maskedObjects;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < maskedObjects.Length; i++)
        {
            maskedObjects[i].GetComponent<MeshRenderer>().material.renderQueue = 3002;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
