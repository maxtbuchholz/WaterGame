using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableInRangeOfPlayer : MonoBehaviour
{
    float timeBetweenChecks = 2.0f;
    float time = 0;
    float minLoadDst = 100;
    bool loaded = true;
    [SerializeField] List<GameObject> effects;
    Transform player = null;
    private void Start()
    {
        player = PointToPlayer.Instance.GetFocalPoint();
    }
    void Update()
    {
        if(time <= 0)
        {
            float dst = Vector3.Distance(player.position, transform.position);
            if(dst <= minLoadDst)
            {
                if(loaded == false)
                {
                    foreach (GameObject go in effects)
                        go.SetActive(true);       
                }
                loaded = true;
            }
            else
            {
                if (loaded == true)
                {
                    foreach (GameObject go in effects)
                        go.SetActive(false);
                }
                loaded = false;
            }
        }
        time -= Time.deltaTime;
    }
}
