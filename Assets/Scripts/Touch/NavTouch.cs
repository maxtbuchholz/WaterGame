using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] CompasUpdate compasUpdate;
    private BoxCollider2D throttleCollider;
    private int throttleTouchId = -1;
    private float navWidth = 164;
    private float navHeight = 164;
    private float widthOffset;
    private float heightOffset;
    private float zoomDstMult = 0.5f;
    private float maxZoomDst = -18;
    private float minZoomDst = -6;
    private Dictionary<int, float> touchTimes = new();
    private List<int> uITouchFinderIds = new();
    private ButtonCollisionTracker buttonCollisionTracker;
    // Start is called before the first frame update
    PlayerInput playerInput;
    public void ResetTouch()
    {
        UpdateThrottlePos(new Vector2(transform.position.x, transform.position.y));
        uITouchFinderIds.Clear();
        throttleTouchId = -1;
    }
    void Start()
    {
        playerInput = PlayerInput.Instance;
        buttonCollisionTracker = ButtonCollisionTracker.Instance;
        throttleCollider = centralThrottle.GetComponent<BoxCollider2D>();
        widthOffset = (canvasRect.rect.width - navWidth) / 2;
        heightOffset = (canvasRect.rect.height - navHeight) / 2;
    }
    public void RotateToNorth()
    {
        StartCoroutine(RotateToNorthCRT());
    }
    private IEnumerator RotateToNorthCRT()
    {
        float timeToNorth = 0.5f;
        float currRotation = focalPoint.rotation.eulerAngles.y;
        int flipped = 1;
        if(currRotation > 180)
        {
            currRotation = 360 - currRotation;
            flipped = -1;
        }
        float currTime = 0;
        while(currTime < timeToNorth)
        {
            currTime += Time.deltaTime;
            Vector3 incRotation = focalPoint.rotation.eulerAngles;
            incRotation.y -= ((currRotation * Time.deltaTime) / timeToNorth) * flipped;
            focalPoint.rotation = Quaternion.Euler(incRotation);
            compasUpdate.UpdateCompas(incRotation.y);
            yield return null;
        }
    }
    // Update is called once per frame
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
                    if (!uITouchFinderIds.Contains(touch.fingerId)) uITouchFinderIds.Add(touch.fingerId);
            }
        }
        if (throttleTouchId == -1)                                              //set finger id if said finger is touching throttle
        {
            foreach (Touch touch in touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    if (throttleCollider.bounds.Contains(touch.position))
                        throttleTouchId = touch.fingerId;
                }
            }
        }
        int touchIndex = -1;
        for (int i = 0; i < touches.Length; i++)                                 //get index from finger id
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
        for (int i = 0; i < touches.Length; i++)                     //check for short touch for fire weapons
        {
            if (!uITouchFinderIds.Contains(touches[i].fingerId))
                if (i != touchIndex)
                {
                    if (touches[i].phase == TouchPhase.Ended)
                    {
                        if (touchTimes.ContainsKey(touches[i].fingerId))
                            if (touchTimes[touches[i].fingerId] < 0.2f)
                            {
                                playerFireControl.RequestShot(touches[i].position);
                            }
                    }
                }
        }
        int zoomPanTouchCount = touches.Length;                     //get touch count for deciding whether to zoom or pan
        if (touchIndex != -1) zoomPanTouchCount -= 1;               //remove one from touch cound if a figner is on the nav wheel
        if (zoomPanTouchCount == 1)                                         //pan around focused on the ship
        {
            for (int i = 0; i < touches.Length; i++)
            {                                           //make sure touch isnt nav touch
                if (!uITouchFinderIds.Contains(touches[i].fingerId))
                    if (i != touchIndex)
                    {
                        float horDif = touches[i].deltaPosition.x / Screen.width;
                        float verDif = touches[i].deltaPosition.y / Screen.height;
                        Vector3 prevRotation = focalPoint.rotation.eulerAngles;
                        horDif *= 220;
                        verDif *= -80;
                        float rotY = (horDif + prevRotation.y) % 360;
                        focalPoint.rotation = Quaternion.Euler(Mathf.Max(Mathf.Min((verDif + prevRotation.x) % 360, 45), 5), rotY, 0);
                        compasUpdate.UpdateCompas(rotY);
                        //Debug.Log(focalPoint.rotation.eulerAngles);
                    }
            }
        }
        else if (zoomPanTouchCount > 1)                                     //zoom in or out focused on the ship
        {
            float screenSizeing = (Screen.width + Screen.height) / 2;
            for (int i = 0; i < touches.Length; i++)
            {
                if (!uITouchFinderIds.Contains(touches[i].fingerId))
                    if (i != touchIndex)                                 //if not nav bar touch
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
        RaycastHit[] hits = Physics.RaycastAll(focalPoint.position, (cameraTransform.position - focalPoint.position).normalized, Mathf.Abs(requestedCamZPos) + 2);
        for (int k = hits.Length - 1; k >= 0; k--)
        {
            if (!hits[k].collider.CompareTag("Ship") && !hits[k].collider.CompareTag("ShipDrive") && !hits[k].collider.CompareTag("Projectile") && !hits[k].collider.CompareTag("IslandOuterCollider") && !hits[k].collider.CompareTag("UI"))
            {
                requestedCamZPos = Mathf.Max((-1 * Vector3.Distance(focalPoint.position, hits[k].point)) + 2.0f, requestedCamZPos);
            }
        }
        //Debug.Log(requestedCamZPos);
        //Debug.DrawLine(focalPoint.position, focalPoint.position + (-requestedCamZPos * (cameraTransform.position - focalPoint.position).normalized), Color.red);
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
        if (pos.x > 0) pos.x = Mathf.Min(pos.x, navWidth);
        else pos.x = Mathf.Max(pos.x, -navWidth);
        if (pos.y > 0) pos.y = Mathf.Min(pos.y, navHeight);
        else pos.y = Mathf.Max(pos.y, -navHeight);
        Vector2 offsetPos = pos;
        //offsetPos.x -= navWidth;
        //offsetPos.x += widthOffset;
        //offsetPos.y -= navHeight;
        //offsetPos.y += heightOffset;
        playerInput.SetVertical((offsetPos.y / navHeight) * Mathf.Sign(offsetPos.y) * Mathf.Sign(offsetPos.y));
        playerInput.SetHorizontal((offsetPos.x / navWidth));
        pos.x += transform.position.x;
        pos.y += transform.position.y;
        centralThrottle.transform.position = pos;
        throttleBar.position = pos;
    }
}
