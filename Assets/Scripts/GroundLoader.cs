using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundLoader : MonoBehaviour
{
    [SerializeField] MeshGenerator meshGenerator;
    [SerializeField] Camera camera;
    float chunkSize;
    private void Start()
    {
        chunkSize = 50;// meshGenerator.WorldWidth;
        Application.targetFrameRate = 60;
    }
    float minUpdateTime = 1f;
    float currTimeSince = 0;
    int pastX = -999999;
    int pastZ = -999999;
    void Update()
    {
        int cameraXChunkStart = Mathf.RoundToInt(camera.transform.position.x / 100) * 100;
        int cameraZChunkStart = Mathf.RoundToInt(camera.transform.position.z / 100) * 100;
        if ((currTimeSince <= 0) && ((pastX != cameraXChunkStart) || (pastZ != cameraZChunkStart)))
        {
            //meshGenerator.gameObject.transform.position = new Vector3(cameraXChunkStart, meshGenerator.gameObject.transform.position.y, cameraZChunkStart);
            meshGenerator.UpdateMesh(cameraXChunkStart, cameraZChunkStart);
            currTimeSince = minUpdateTime;
            pastX = cameraXChunkStart;
            pastZ = cameraZChunkStart;
        }
        else if (currTimeSince <= 0)
            currTimeSince = -1;
        else
            currTimeSince -= Time.deltaTime;
    }
}
