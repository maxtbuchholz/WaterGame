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
    private float maxRadius = 200;
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
        }
    }
    void UpdateThrottlePos(Vector2 pos)
    {
        pos.x -= transform.position.x;
        pos.y -= transform.position.y;
        float magnitude = pos.magnitude;
        Vector2 normal = pos.normalized;
        magnitude = Mathf.Min(magnitude, maxRadius);
        pos = normal * magnitude;
        PlayerInput.Instance.SetVertical(magnitude / maxRadius);
        PlayerInput.Instance.SetHorizontal(pos.x / maxRadius);
        //Debug.Log("vert:" + magnitude / maxRadius + " hor: " + pos.x / maxRadius);
        pos.x += transform.position.x;
        pos.y += transform.position.y;
        centralThrottle.transform.position = pos;
    }
}
