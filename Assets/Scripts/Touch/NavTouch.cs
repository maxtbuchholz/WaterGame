using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavTouch : MonoBehaviour
{
    [SerializeField] GameObject centralThrottle;
    [SerializeField] RectTransform canvasRect;
    [SerializeField] Camera camera;
    private BoxCollider2D throttleCollider;
    private int throttleTouchId = -1;
    private float navWidth = 200;
    private float navHeight = 200;
    // Start is called before the first frame update
    void Start()
    {
        throttleCollider = centralThrottle.GetComponent<BoxCollider2D>();
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
            if (Mathf.Abs((throttlePos.x) - navWidth) < 50)
            {
                throttlePos.x = navWidth;
                if (Mathf.Abs(throttlePos.y - navHeight) < 50)
                {
                    throttlePos.y = navHeight;
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
        if (pos.x  > navWidth) pos.x = Mathf.Min(pos.x, 2 * navWidth);
        else pos.x = Mathf.Max(pos.x, 0);
        if (pos.y > navWidth) pos.y = Mathf.Min(pos.y, 2 * navHeight);
        else pos.y = Mathf.Max(pos.y, 0);
        Vector2 offsetPos = pos;
        offsetPos.x -= navWidth;
        offsetPos.y -= navHeight;
        PlayerInput.Instance.SetVertical((offsetPos.y / navHeight) * Mathf.Sign(offsetPos.y) * Mathf.Sign(offsetPos.y));
        PlayerInput.Instance.SetHorizontal((offsetPos.x / navWidth) * Mathf.Sign(offsetPos.y));
        pos.x += transform.position.x;
        pos.y += transform.position.y;
        centralThrottle.transform.position = pos;
    }
}
