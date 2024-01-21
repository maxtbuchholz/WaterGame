using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavTouch : MonoBehaviour
{
    [SerializeField] GameObject centralThrottle;
    [SerializeField] Transform throttleBar;
    [SerializeField] RectTransform canvasRect;
    [SerializeField] Camera camera;
    private BoxCollider2D throttleCollider;
    private int throttleTouchId = -1;
    private float navWidth = 164;
    private float navHeight = 164;
    private float widthOffset;
    private float heightOffset;
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
        if(touchIndex != -1)                                                    //update throttle                   
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
        Debug.Log(pos);
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
