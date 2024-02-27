using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocalPointFollow : MonoBehaviour
{
    [SerializeField] Transform mainShip;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(mainShip.position.x, transform.position.y, mainShip.position.z);
    }
    public void SetFollowTransform(Transform newF)
    {
        mainShip = newF;
    }
}
