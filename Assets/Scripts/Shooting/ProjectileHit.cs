using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHit : MonoBehaviour
{
    [SerializeField] GameObject waterSplashEffect;
    void OnCollisionEnter(Collision collision)
    {
        Collide(collision.gameObject.tag);
    }
    private void Collide(string tag)
    {
        Debug.Log("tag: " + tag);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        Destroy(GetComponent<Rigidbody>());
        Destroy(GetComponent<Collider>());
        GetComponent<Renderer>().enabled = false;
        timeSinceHit = GetComponent<TrailRenderer>().time;

        if(tag == "SeaTile")
        {
            GameObject splash = GameObject.Instantiate(waterSplashEffect);
            splash.transform.position = transform.position;
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
