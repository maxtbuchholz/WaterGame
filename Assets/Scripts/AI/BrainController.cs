using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainController : MonoBehaviour
{
    [SerializeField] GameObject straight_brain;
    [SerializeField] GameObject avoid_brain;
    [SerializeField] GameObject back_brain;
    [SerializeField] GameObject debugSphere;
    private readonly float timeBetweenChecks = 2.0f;
    Material sphereMaterial;
    private float time = 0;
    private void Start()
    {
        Material[] fortMaterals = debugSphere.GetComponent<MeshRenderer>().materials;
        foreach (Material mat in fortMaterals)
        {
            if (mat.name == "Ship (Instance)")
            {
                sphereMaterial = mat;
            }
        }

        straight_brain.GetComponent<Circles_Drive_Agent>().SetEnables(true);
        avoid_brain.GetComponent<Around_Drive_Agent>().SetEnables(false);
        back_brain.GetComponent<BackBrain>().SetEnables(false);
        sphereMaterial.SetColor("_BaseColor", Color.green);
    }
    private void Update()
    {
        time -= Time.deltaTime;
        if(time <= 0)
        {
            if (NearLand())
            {
                straight_brain.GetComponent<Circles_Drive_Agent>().SetEnables(false);
                avoid_brain.GetComponent<Around_Drive_Agent>().SetEnables(true);
                back_brain.GetComponent<BackBrain>().SetEnables(false);
                sphereMaterial.SetColor("_BaseColor", Color.yellow);
            }
            else
            {
                straight_brain.GetComponent<Circles_Drive_Agent>().SetEnables(true);
                avoid_brain.GetComponent<Around_Drive_Agent>().SetEnables(false);
                back_brain.GetComponent<BackBrain>().SetEnables(false);
                sphereMaterial.SetColor("_BaseColor", Color.green);
            }
            time = timeBetweenChecks;
        }
    }
    private bool NearLand()
    {
        return (GetLandInDirection(10, 50) || GetLandInDirection(0, 50) || GetLandInDirection(-10, 50));
        float boxHalfWidth = 50;
        RaycastHit[] hits = Physics.BoxCastAll(transform.position + new Vector3(0, 2, 0), new Vector3(boxHalfWidth, 1, boxHalfWidth), Vector3.down, Quaternion.identity, 1);
        foreach (RaycastHit hit in hits)
            if (hit.collider.CompareTag("Land"))
                return true;
        return false;
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Land") && !back_brain.GetComponent<BackBrain>().ena)
        {
            straight_brain.GetComponent<Circles_Drive_Agent>().SetEnables(false);
            avoid_brain.GetComponent<Around_Drive_Agent>().SetEnables(false);
            back_brain.GetComponent<BackBrain>().SetEnables(true);
            sphereMaterial.SetColor("_BaseColor", Color.red);
            time = 10;
        }
    }
    private bool GetLandInDirection(float angle, float distance)
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, (Quaternion.Euler(0, angle, 0) * transform.forward), distance);
        float groundHitDistance = -1;
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Land"))
                if ((groundHitDistance == -1) || (hit.distance < groundHitDistance))
                    return true;
        }
        return false;
    }
}
