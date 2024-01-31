using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontTrail : MonoBehaviour
{
    Queue<PosNormTime> posQueue = new();
    float maxTime = 2.0f;
    float minResreshTime = 0.025f;
    float cycleTime = 0f;
    [SerializeField] Transform wakeTrans;
    private TrailRenderer wake;
    [SerializeField] Transform wakeTransLeft;
    [SerializeField] ShipMovement shipMovement;
    private TrailRenderer wakeLeft;
    private void Start()
    {
        wake = wakeTrans.GetComponent<TrailRenderer>();
        wakeLeft = wakeTransLeft.GetComponent<TrailRenderer>();
    }
    void FixedUpdate()
    {
        cycleTime += Time.fixedDeltaTime;
        minResreshTime = Time.fixedDeltaTime;
        List<Vector3> newTrailPos = new();
        List<Vector3> newTrailPosLeft = new();
        if (cycleTime >= minResreshTime)
        {
            Debug.DrawLine(transform.position, transform.position + (transform.forward * 10), Color.red);
            posQueue.Enqueue(new PosNormTime(transform.position, transform.forward, shipMovement.currSpeedPercentage));

            Queue<PosNormTime> tempQueue = new Queue<PosNormTime>();
            PosNormTime tempPNT;
            wake.Clear();
            wakeLeft.Clear();
            while (posQueue.Count > 0)
            {
                tempPNT = posQueue.Dequeue();
                tempPNT.time += cycleTime;
                if(tempPNT.time < maxTime)
                {
                    tempQueue.Enqueue(tempPNT);
                    int negMult = tempPNT.speedPer >= 0 ? 1 : 0;
                    Vector3 rightAngle = Quaternion.AngleAxis(90, Vector3.up) * tempPNT.norm * (tempPNT.time / maxTime) * 3 * Mathf.Abs(tempPNT.speedPer) * negMult;
                    Vector3 leftAngle = Quaternion.AngleAxis(-90, Vector3.up) * tempPNT.norm * (tempPNT.time / maxTime) * 3 * Mathf.Abs(tempPNT.speedPer) * negMult;
                    Vector3 newPos = new Vector3(tempPNT.pos.x, 0.02f, tempPNT.pos.z);
                    //Debug.DrawLine(wakeTrans.position + (newPos.x), newPos, Color.red);
                    newTrailPos.Add(newPos + rightAngle);
                    newTrailPosLeft.Add(newPos + leftAngle);
                }
            }
            //if(newTrailPos.Count > 0)
            //{
            //    int curOkCheck = 0;
            //    List<int> removeIndexes = new();
            //    float minDist = 0.2f;
            //    for(int i = 1; i < newTrailPos.Count; i++)
            //    {
            //        if (Vector3.Distance(newTrailPos[curOkCheck], newTrailPos[i]) < minDist) removeIndexes.Add(i);
            //        else curOkCheck = i;
            //    }
            //    for (int i = removeIndexes.Count - 1; i >= 0; i--)
            //        newTrailPos.RemoveAt(removeIndexes[i]);
            //}
            //for(int i = newTrailPos.Count - 1; i >= 0 ; i--)
            //{
            //    if (i % 2 == 1)
            //        newTrailPos.RemoveAt(i);
            //}
            wake.AddPositions(newTrailPos.ToArray());
            wakeLeft.AddPositions(newTrailPosLeft.ToArray());
            posQueue = tempQueue;
            //Debug.Log(posQueue.Count + newPos);



            cycleTime = 0;



            //foreach (PosNormTime pNT in posQueue) pNT.time += Time.deltaTime;
            //bool foundAllOutOfTime = false;
            //while ((posQueue.Count > 0) && !foundAllOutOfTime)
            //{
            //    if (posQueue.Peek().time > maxTime)
            //    {
            //        posQueue.Dequeue();
            //    }
            //    else foundAllOutOfTime = true;
            //}
            ////float wakeTime = wake.time;
            ////wake.time = 0;
            //wake.Clear();
            ////wake.time = wakeTime;
            ////Debug.Log("____" + Time.time);
            //Debug.Log(posQueue.Count);
            //foreach (PosNormTime pNT in posQueue)
            //{
            //    Vector3 rightAngle = Quaternion.AngleAxis(90, Vector3.up) * pNT.norm;
            //    //Debug.Log(pNT.pos);
            //    wakeTrans.position = new Vector3(pNT.pos.x, 0.02f, pNT.pos.z);
            //}
            //Debug.Log("____" + Time.time + "_2");
            //wakeTrans.position = new Vector3(transform.position.x, 0.02f, transform.position.z);
        }
    }
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        foreach(PosNormTime pNT in posQueue)
         Gizmos.DrawSphere(pNT.pos, 0.5f);
    }
    class PosNormTime
    {
        public Vector3 pos;
        public Vector3 norm;
        public float time;
        public float speedPer;
        public PosNormTime(Vector3 pos, Vector3 norm, float speedPer)
        {
            this.pos = pos;
            this.norm = norm;
            time = 0;
            this.speedPer = speedPer;
        }
    }
}
