using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFireControl : MonoBehaviour
{
    [SerializeField] List<TurretController> turrets;
    [SerializeField] Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = camera.ScreenPointToRay(mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.gameObject.tag != "IslandOuterCollider")          //found suitable target
                {
                    foreach(TurretController tC in turrets)
                        tC.ShootProjectile(hits[i].point);
                    i = hits.Length;
                }
            }
        }
    }
}
