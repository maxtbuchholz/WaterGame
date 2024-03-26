using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;

public class FortGenerator : MonoBehaviour
{
    [SerializeField] GameObject fortPrefab;
    Vector3 center = Vector3.zero;
    private SaveData saveData;
    public string fortKey;
    public void LoadFort(float xPos, float zPos, float islandXWidth, float islandZWidth, string islandKey, int team)
    {
        fortKey = islandKey + "_fort";
        saveData = SaveData.Instance;
        float furthestDstIn = -1;
        Vector3 bestFortPos = Vector3.zero;
        Vector3 bestCentralPos = Vector3.zero;
        int fortTeam = -1;
        if (!saveData.FortExists(fortKey))                      //no fort has been loaded for this island before, load new fort
        {
            islandXWidth /= 2;
            islandZWidth /= 2;
            center = new Vector3(transform.position.x, 0, transform.position.z);
            List<Vector3> corners = new List<Vector3>() {   new Vector3(transform.position.x + islandXWidth, 0, transform.position.z + islandZWidth),
                                                        new Vector3(transform.position.x + islandXWidth, 0, transform.position.z - islandZWidth),
                                                        new Vector3(transform.position.x - islandXWidth, 0, transform.position.z + islandZWidth),
                                                        new Vector3(transform.position.x - islandXWidth, 0, transform.position.z - islandZWidth),};
            foreach (Vector3 corner in corners)
            {
                if (NoLandOverCorner(corner))
                {
                    Vector3 fortTryPos;
                    Vector3 centralTryPos;
                    float distIn = DistanceBeforeLandTowardsCenter(corner, out fortTryPos, out centralTryPos);

                    if ((furthestDstIn == -1) || (distIn > furthestDstIn))
                    {
                        furthestDstIn = distIn;
                        bestFortPos = fortTryPos;
                        bestCentralPos = centralTryPos;
                        bestCentralPos.y = 0;
                    }
                }
            }
            if(furthestDstIn != -1)
            {
                saveData.AddFort(fortKey, bestFortPos);
            }
        }
        else
        {                                                   //island loaded before, load previously saved location
            bestFortPos = saveData.GetFortPos(fortKey);
            fortTeam = saveData.GetFortTeam(fortKey);
            furthestDstIn = 0;
        }
        if (furthestDstIn != -1)
        {
            GameObject fort = GameObject.Instantiate(fortPrefab, new Vector3(bestFortPos.x, 0.1f, bestFortPos.z), Quaternion.identity);
            fort.transform.parent = transform;
            LoadedObjects.Instance.AddLoadedFort(fortKey, fort);
            fort.GetComponent<HealthController>().camera = Camera.main;
            fort.GetComponent<FortLevel>().SetCentralLocation(bestCentralPos);                //set central point has to be before init from key
            fort.GetComponent<FortLevel>().InitFromKey(fortKey);
            if (fortTeam != -1)
                fort.GetComponent<LocalTeamController>().ForceChangeTeam(fortTeam);
            else
            {
                saveData.SetFortTeam(fortKey, fort.GetComponent<LocalTeamController>().GetTeam());
                fort.GetComponent<LocalTeamController>().ForceChangeTeam(fortTeam);
            }
            fort.GetComponent<FortValues>().SetKey(fortKey);
            if(team != -1)
                fort.GetComponent<LocalTeamController>().ForceChangeTeam(team);
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
    private float DistanceBeforeLandTowardsCenter(Vector3 outerPos, out Vector3 fortTryPos, out Vector3 centralTryPos)
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
        centralTryPos = outerPos + (dirToCenter * (closestLandDst * 0.98f));
        return closestLandDst;
    }
}