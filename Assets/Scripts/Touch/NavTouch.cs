using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class NavTouch : MonoBehaviour
{
    [SerializeField] GameObject centralThrottle;
    [SerializeField] Transform throttleBar;
    [SerializeField] RectTransform canvasRect;
    [SerializeField] Camera camera;
    [SerializeField] PlayerFireControl playerFireControl;
    [SerializeField] Transform focalPoint;
    [SerializeField] Transform cameraTransform;
    private BoxCollider2D throttleCollider;
    private int throttleTouchId = -1;
    private float navWidth = 164;
    private float navHeight = 164;
    private float widthOffset;
    private float heightOffset;
    private float zoomDstMult = 0.5f;
    private float maxZoomDst = -8;
    private float minZoomDst = -2;
    private Dictionary<int, float> touchTimes = new();
    // Start is called before the first frame update
    void Start()
    {
        throttleCollider = centralThrottle.GetComponent<BoxCollider2D>();
        widthOffset = (canvasRect.rect.width - navWidth) / 2;
        heightOffset = (canvasRect.rect.height - navHeight) / 2;
    }

    // Update is called once per frame
    void Update()
    {
        Touch[] touches = Input.touches;
        List<int> removeDictKeys = new();
        foreach(KeyValuePair<int, float> entry in touchTimes)               //remove touches that are no longer active
        {
            bool inclded = false;
            foreach (Touch touch in touches)
            {
                if (touch.fingerId == entry.Key)
                    inclded = true;
            }
            if (!inclded)
                removeDictKeys.Add(entry.Key);
        }
        foreach (int key in removeDictKeys)                         //actaually remove the keys and values from now non existing touch
            touchTimes.Remove(key);
        foreach(Touch touch in touches)                                 //add time to active touches
        {
            if (touchTimes.ContainsKey(touch.fingerId))
                touchTimes[touch.fingerId] += Time.deltaTime;
            else
                touchTimes.Add(touch.fingerId, 0);
        }
        if (throttleTouchId == -1)                                              //set finger id if said finger is touching throttle
        {
            foreach (Touch touch in touches)
            {
                if(touch.phase == TouchPhase.Began)
                {
                    if (throttleCollider.bounds.Contains(touch.position))
                        throttleTouchId = touch.fingerId;
                }
            }
        }
        int touchIndex = -1;
        for(int i = 0; i < touches.Length; i++)                                 //get index from finger id
        {
            if (touches[i].fingerId == throttleTouchId)
            {
                touchIndex = i;
                if (touches[i].phase == TouchPhase.Ended)
                {
                    touchIndex = -1;
                    throttleTouchId = -1;
                }
            }
        }
        for(int i = 0; i < touches.Length; i++)                     //check for short touch for fire weapons
        {
            if(i != touchIndex)
            {
                if (touches[i].phase == TouchPhase.Ended)
                {
                    if (touchTimes.ContainsKey(touches[i].fingerId))
                        if (touchTimes[touches[i].fingerId] < 0.2f)
                            playerFireControl.RequestShot(touches[i].position);
                }
            }
        }
        int zoomPanTouchCount = touches.Length;                     //get touch count for deciding whether to zoom or pan
        if (touchIndex != -1) zoomPanTouchCount -= 1;               //remove one from touch cound if a figner is on the nav wheel
        if(zoomPanTouchCount == 1)                                         //pan around focused on the ship
        {
            for (int i = 0; i < touches.Length; i++)
            {                                           //make sure touch isnt nav touch
                if(i != touchIndex)
                {
                    float horDif = touches[i].deltaPosition.x / Screen.width;
                    float verDif = touches[i].deltaPosition.y / Screen.height;
                    Vector3 prevRotation = focalPoint.rotation.eulerAngles;
                    horDif *= 220;
                    verDif *= -80;
                    focalPoint.rotation = Quaternion.Euler(Mathf.Max(Mathf.Min((verDif + prevRotation.x) % 360, 60), 5), (horDif + prevRotation.y) % 360, 0);
                    //Debug.Log(focalPoint.rotation.eulerAngles);
                }
            }
        }
        else if(zoomPanTouchCount > 1)                                     //zoom in or out focused on the ship
        {
            float screenSizeing = (Screen.width + Screen.height) / 2;
            for(int i = 0; i < touches.Length; i++)
            {
                if(i != touchIndex)                                 //if not nav bar touch
                {
                    Vector2 otherFingersPos = Vector2.zero;
                    for (int k = 0; k < touches.Length; k++)        //itterate through rest of applicable touches and get center
                    {
                        if ((k != touchIndex) && (k != i))
                        {
                            otherFingersPos += touches[k].position;
                        }
                    }
                    otherFingersPos /= (zoomPanTouchCount - 1);
                    float prevDst = Vector2.Distance(otherFingersPos, touches[i].position);
                    float deltaDst = Vector2.Distance(otherFingersPos, touches[i].position - touches[i].deltaPosition) - prevDst;
                    deltaDst /= screenSizeing;
                    deltaDst *= 2;
                    zoomDstMult += deltaDst;
                    zoomDstMult = Mathf.Min(zoomDstMult, 1.0f);
                    zoomDstMult = Mathf.Max(zoomDstMult, 0.0f);
                }
            }
        }
        float requestedCamZPos = Mathf.Lerp(minZoomDst, maxZoomDst, zoomDstMult);
        RaycastHit[] hits = Physics.RaycastAll(focalPoint.position, (cameraTransform.position - focalPoint.position).normalized, Mathf.Abs(requestedCamZPos));
        for (int k = hits.Length - 1; k >= 0; k--)
        {
            if (!hits[k].collider.CompareTag("Ship") && !hits[k].collider.CompareTag("ShipDrive") && !hits[k].collider.CompareTag("Projectile") && !hits[k].collider.CompareTag("IslandOuterCollider"))
            {
                requestedCamZPos = Mathf.Max((-1 * Vector3.Distance(focalPoint.position, hits[k].point)) + 0.5f, requestedCamZPos);
            }
        }
        cameraTransform.localPosition = new Vector3(0, 0, requestedCamZPos);
        if (touchIndex != -1)                                                    //update throttle                   
        {
            UpdateThrottlePos(touches[touchIndex].position);
        }
        else
        {                                                                       //set to not touching throttle
            throttleTouchId = -1;
            Vector2 throttlePos = Vector2.zero;
            throttlePos.x = centralThrottle.transform.localPosition.x;
            throttlePos.y = centralThrottle.transform.localPosition.y;
            if (Mathf.Abs((throttlePos.x)) < 75)
            {
                throttlePos.x = 0;
                if (Mathf.Abs(throttlePos.y) < 75)
                {
                    throttlePos.y = 0;
                    throttleBar.position = Vector2.zero;
                }
                throttlePos.x += transform.position.x;
                throttlePos.y += transform.position.y;
                centralThrottle.transform.position = throttlePos;
                UpdateThrottlePos(centralThrottle.transform.position);
            }
        }
    }
    void UpdateThrottlePos(Vector2 pos)
    {
        //Debug.Log(pos);
        pos.x -= transform.position.x;
        pos.y -= transform.position.y;
        if (pos.x  > 0) pos.x = Mathf.Min(pos.x, navWidth);
        else pos.x = Mathf.Max(pos.x, -navWidth);
        if (pos.y > 0) pos.y = Mathf.Min(pos.y, navHeight);
        else pos.y = Mathf.Max(pos.y, -navHeight);
        Vector2 offsetPos = pos;
        //offsetPos.x -= navWidth;
        //offsetPos.x += widthOffset;
        //offsetPos.y -= navHeight;
        //offsetPos.y += heightOffset;
        PlayerInput.Instance.SetVertical((offsetPos.y / navHeight) * Mathf.Sign(offsetPos.y) * Mathf.Sign(offsetPos.y));
        PlayerInput.Instance.SetHorizontal((offsetPos.x / navWidth));
        pos.x += transform.position.x;
        pos.y += transform.position.y;
        centralThrottle.transform.position = pos;
        throttleBar.position = pos;
    }
}
