using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHit : MonoBehaviour
{
    [SerializeField] GameObject waterSplashEffect;
    [SerializeField] GameObject explosionEffect;
    [SerializeField] GameObject cannonFireEffect;
    public List<GameObject> shipParts;
    private Collider projCollider;
    private void Start()
    {
        projCollider = GetComponent<Collider>();
        foreach (GameObject go in shipParts)
        {
            if (go.TryGetComponent<Collider>(out Collider col))
            {
                Physics.IgnoreCollision(col, projCollider);
            }
        }
        gameObject.layer = 10;
        GameObject explosion = GameObject.Instantiate(cannonFireEffect);
        explosion.transform.position = transform.position;
    }
    void OnCollisionEnter(Collision collision)
    {
        Collide(collision.gameObject.tag);
        //Debug.Break();
    }
    private void Collide(string tag)
    {
        Debug.Log("tag: " + tag);
        if (tag != "IslandOuterCollider")
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<Collider>());
            GetComponent<Renderer>().enabled = false;
            timeSinceHit = GetComponent<TrailRenderer>().time;

            if (tag == "SeaTile")
            {
                GameObject splash = GameObject.Instantiate(waterSplashEffect);
                splash.transform.position = transform.position;
            }
            else
            {
                GameObject explosion = GameObject.Instantiate(explosionEffect);
                explosion.transform.position = transform.position;
            }
        }
    }
    float timeSinceHit = -100;
    private void Update()
    {
        if (timeSinceHit != -100)
        {
            if(timeSinceHit > 0)
            {
                timeSinceHit -= Time.deltaTime;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
