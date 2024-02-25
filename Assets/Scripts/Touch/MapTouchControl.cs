using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTouchControl : MonoBehaviour
{
    [SerializeField] RectTransform mapRect;
    [SerializeField] LoadMap loadMap;
    private List<int> uITouchFinderIds = new();
    Dictionary<int, float> touchTimes = new();
    private ButtonCollisionTracker buttonCollisionTracker;
    private float zoomDstMult = 8f;
    private float maxZoomDst = 24;
    private float minZoomDst = 4;
    private void Start()
    {
        buttonCollisionTracker = ButtonCollisionTracker.Instance;
        zoomDstMult = 8;
    }
    public void ResetStart()
    {
        zoomDstMult = 8;
    }
    void Update()
    {
        Touch[] touches = Input.touches;
        List<int> removeDictKeys = new();
        foreach (KeyValuePair<int, float> entry in touchTimes)               //remove touches that are no longer active
        {
            bool inclded = false;
            foreach (Touch touch in touches)
            {
                if (touch.fingerId == entry.Key)
                    inclded = true;
            }
            if (!inclded)
            {
                removeDictKeys.Add(entry.Key);
                if (uITouchFinderIds.Contains(entry.Key))
                {
                    uITouchFinderIds.Remove(entry.Key);
                    buttonCollisionTracker.EndTouchingButton(entry.Key, ButtonCollisionTracker.touchPhase.end);
                }
            }
        }
        foreach (int key in removeDictKeys)                         //actaually remove the keys and values from now non existing touch
            touchTimes.Remove(key);
        foreach (Touch touch in touches)                                 //add time to active touches
        {
            if (touchTimes.ContainsKey(touch.fingerId))
                touchTimes[touch.fingerId] += Time.deltaTime;
            else
            {
                touchTimes.Add(touch.fingerId, 0);
                if (!buttonCollisionTracker.IsntTouchingButton(touch.fingerId, touch.position, ButtonCollisionTracker.touchPhase.start))
                {
                    if (!uITouchFinderIds.Contains(touch.fingerId)) uITouchFinderIds.Add(touch.fingerId);
                    Debug.Log("UI touch added");
                }
            }
        }
        int zoomPanTouchCount = touches.Length - uITouchFinderIds.Count;                     //get touch count for deciding whether to zoom or pan
        if (zoomPanTouchCount > 0)                                         //pan around focused on the ship
        {
            float horDif = 0;
            float verDif = 0;
            for (int i = 0; i < touches.Length; i++)
            {                                           //make sure touch isnt nav touch
                if (!uITouchFinderIds.Contains(touches[i].fingerId))
                {
                    if (!uITouchFinderIds.Contains(touches[i].fingerId))
                    {
                        horDif += touches[i].deltaPosition.x;
                        verDif += touches[i].deltaPosition.y;
                    }
                    horDif /= zoomPanTouchCount;
                    verDif /= zoomPanTouchCount;
                    Vector2 panOfffset = new Vector2(horDif, verDif);
                    loadMap.AddPan(panOfffset);
                }
            }
        }
        if (zoomPanTouchCount > 1)                                     //zoom in or out focused on the ship
        {
            float screenSizeing = (Screen.width + Screen.height) / 2;
            for (int i = 0; i < touches.Length; i++)
            {
                if (!uITouchFinderIds.Contains(touches[i].fingerId))
                {
                    Vector2 otherFingersPos = Vector2.zero;
                    for (int k = 0; k < touches.Length; k++)
                    {
                        if ((i != k) && (!uITouchFinderIds.Contains(touches[k].fingerId)))
                            otherFingersPos += touches[k].position;
                    }
                    otherFingersPos /= (zoomPanTouchCount - 1);
                    if (!uITouchFinderIds.Contains(touches[i].fingerId))                               //if not nav bar touch
                    {
                        float prevDst = Vector2.Distance(otherFingersPos, touches[i].position);
                        float deltaDst = Vector2.Distance(otherFingersPos, touches[i].position - touches[i].deltaPosition) - prevDst;
                        deltaDst /= screenSizeing;
                        deltaDst *= 50;
                        zoomDstMult += deltaDst;
                        zoomDstMult = Mathf.Min(zoomDstMult, maxZoomDst);
                        zoomDstMult = Mathf.Max(zoomDstMult, minZoomDst);
                    }
                }
            }
        }
        loadMap.SetZoom(zoomDstMult);
        //float requestedCamZPos = Mathf.Lerp(minZoomDst, maxZoomDst, zoomDstMult);
        //RaycastHit[] hits = Physics.RaycastAll(focalPoint.position, (cameraTransform.position - focalPoint.position).normalized, Mathf.Abs(requestedCamZPos) + 2);
        //for (int k = hits.Length - 1; k >= 0; k--)
        //{
        //    if (!hits[k].collider.CompareTag("Ship") && !hits[k].collider.CompareTag("ShipDrive") && !hits[k].collider.CompareTag("Projectile") && !hits[k].collider.CompareTag("IslandOuterCollider") && !hits[k].collider.CompareTag("UI"))
        //    {
        //        requestedCamZPos = Mathf.Max((-1 * Vector3.Distance(focalPoint.position, hits[k].point)) + 2.0f, requestedCamZPos);
        //    }
        //}
    }
}
