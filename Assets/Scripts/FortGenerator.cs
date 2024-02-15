using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FortGenerator : MonoBehaviour
{
    [SerializeField] GameObject fortPrefab;
    Vector3 center = Vector3.zero;
    public void LoadFort(float xPos, float zPos, float islandXWidth, float islandZWidth)
    {
        islandXWidth /= 2;
        islandZWidth /= 2;
        center = new Vector3(transform.position.x, 0, transform.position.z);
        List<Vector3> corners = new List<Vector3>() {   new Vector3(transform.position.x + islandXWidth, 0, transform.position.z + islandZWidth),
                                                        new Vector3(transform.position.x + islandXWidth, 0, transform.position.z - islandZWidth),
                                                        new Vector3(transform.position.x - islandXWidth, 0, transform.position.z + islandZWidth),
                                                        new Vector3(transform.position.x - islandXWidth, 0, transform.position.z - islandZWidth),};
        float furthestDstIn = -1;
        Vector3 bestFortPos = Vector3.zero;
        foreach (Vector3 corner in corners)
        {
            if (NoLandOverCorner(corner))
            {
                Vector3 fortTryPos;
                float distIn = DistanceBeforeLandTowardsCenter(corner, out fortTryPos);

                if ((furthestDstIn == -1) || (distIn > furthestDstIn))
                {
                    furthestDstIn = distIn;
                    bestFortPos = fortTryPos;
                }
            }
        }
        if (furthestDstIn != -1)
        {
            GameObject fort = GameObject.Instantiate(fortPrefab, new Vector3(bestFortPos.x, 0.1f, bestFortPos.z), Quaternion.identity);
            fort.transform.parent = transform;
            fort.GetComponent<HealthController>().camera = Camera.main;
        }
    }
    private bool NoLandOverCorner(Vector3 corner)
    {
        corner.y = 50;
        RaycastHit[] hits = Physics.RaycastAll(corner, Vector3.down, 50);
        foreach (RaycastHit hit in hits)
            if (hit.collider.CompareTag("Land")) return false;
        return true;
    }
    private float DistanceBeforeLandTowardsCenter(Vector3 outerPos, out Vector3 fortTryPos)
    {
        Vector3 dirToCenter = center - outerPos;
        float maxToCenter = dirToCenter.magnitude;
        dirToCenter = dirToCenter.normalized;
        RaycastHit[] hits = Physics.RaycastAll(outerPos, dirToCenter, maxToCenter);
        float closestLandDst = -1;
        Vector3 closestLandPos = Vector3.zero;
        foreach (RaycastHit hit in hits)
            if (hit.collider.CompareTag("Land"))
                if ((closestLandDst == -1) || (hit.distance < closestLandDst))
                {
                    closestLandDst = hit.distance;
                    closestLandPos = hit.point;
                }
        Vector3 normDir = (outerPos - closestLandPos).normalized;
        normDir.y = 0;
        closestLandPos += (2 * normDir);
        fortTryPos = closestLandPos;
        return closestLandDst;
    }
}