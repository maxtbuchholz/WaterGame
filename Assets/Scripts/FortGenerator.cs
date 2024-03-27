using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class FortGenerator : MonoBehaviour
{
    [SerializeField] GameObject fortPrefab;
    Vector3 center = Vector3.zero;
    private SaveData saveData;
    public string fortKey;
    public async void LoadFort(float xPos, float zPos, float islandXWidth, float islandZWidth, string islandKey, int team)
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
            if (team != -1)
            {
                fort.GetComponent<LocalTeamController>().ForceChangeTeam(team);
                if(saveData.FirstTimeLoad)
                    await WaitFastTravel(saveData.GetFortPos(fortKey));
            }
        }
    }
    private Vector2[] spawnChecks = new Vector2[] { new Vector2(20, 0), new Vector2(0, 20), new Vector2(-20, 0), new Vector2(0, -20),
                                                    new Vector2(15, 15), new Vector2(-15, 15), new Vector2(-15, -15), new Vector2(15, -15)};
    public async Task WaitFastTravel(Vector3 pos)
    {
        PopupManager.Instance.SummonLoadingScreen();
        GameObject tempFocalFollow = new GameObject();
        tempFocalFollow.transform.position = pos;
        PointToPlayer.Instance.GetFocalPoint().GetComponent<FocalPointFollow>().SetFollowTransform(tempFocalFollow.transform);
        PointToPlayer.Instance.ResetTouch();
        await Task.Delay(500);
        List<Vector3> OkaySpawnPoints = new();
        foreach (Vector2 check in spawnChecks)
        {
            bool nearPointOk = true;
            Vector3 testPos = new Vector3(pos.x + check.x, 20, pos.z + check.y);
            RaycastHit[] hits = Physics.RaycastAll(new Vector3(pos.x + check.x, 20, pos.z + check.y), Vector3.down, 20.5f);
            testPos.y = 0;
            foreach (RaycastHit hit in hits)
                if (hit.collider.CompareTag("Land"))
                    nearPointOk = false;
            if (nearPointOk)
            {
                float checkPointsPerTSD = 6;
                float angleIncriment = 360 / checkPointsPerTSD;
                float currCheckAngle = 0;
                while (currCheckAngle < 360)
                {
                    float xAdd = Mathf.Sin(currCheckAngle * Mathf.Rad2Deg);
                    float zAdd = Mathf.Cos(currCheckAngle * Mathf.Rad2Deg);
                    Vector3 dir = new Vector3(xAdd, 0, zAdd);
                    hits = Physics.RaycastAll(testPos, dir, 5);
                    foreach (RaycastHit hit in hits)
                        if (hit.collider.CompareTag("Land"))
                            nearPointOk = false;
                    currCheckAngle += angleIncriment;
                }

                if (nearPointOk)
                {
                    checkPointsPerTSD = 8;
                    angleIncriment = 360 / checkPointsPerTSD;
                    currCheckAngle = 0;
                    bool atLeastOneDstOkFar = false;
                    while (currCheckAngle < 360)
                    {
                        bool curLineOk = true;
                        float xAdd = Mathf.Sin(currCheckAngle * Mathf.Rad2Deg);
                        float zAdd = Mathf.Cos(currCheckAngle * Mathf.Rad2Deg);
                        Vector3 dir = new Vector3(xAdd, 0, zAdd);
                        hits = Physics.RaycastAll(testPos, dir, 5);
                        foreach (RaycastHit hit in hits)
                            if (hit.collider.CompareTag("Land"))
                                curLineOk = false;
                        if (curLineOk)
                        {
                            atLeastOneDstOkFar = true;
                            currCheckAngle = 360;
                        }
                        currCheckAngle += angleIncriment;
                    }
                    if (atLeastOneDstOkFar)                             //found ok place to spawn
                    {
                        OkaySpawnPoints.Add(new Vector3(pos.x + check.x, 0, pos.z + check.y));
                    }
                }
            }
        }
        if (OkaySpawnPoints.Count > 0)                                   //okay to teleport player to one of these points
        {
            int toIndex = Random.Range(0, OkaySpawnPoints.Count);
            PointToPlayer.Instance.GetPlayerShip().position = OkaySpawnPoints[toIndex];
        }
        PointToPlayer.Instance.GetFocalPoint().GetComponent<FocalPointFollow>().SetFollowTransform(PointToPlayer.Instance.GetPlayerShip());
        await Task.Delay(500);
        PopupManager.Instance.EndLoadingScreen();
        Destroy(tempFocalFollow);
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