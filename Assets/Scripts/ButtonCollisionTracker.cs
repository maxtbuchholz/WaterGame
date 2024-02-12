using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class ButtonCollisionTracker : MonoBehaviour
{
    private static ButtonCollisionTracker instance;
    private Dictionary<Collider, int> WorldButons = new();    //world colliders to levels, 0 is infront of 1, etc, 0 might mean the camera ui and 1 -> inf is world buttons
    [SerializeField] Camera camera;
    [SerializeField] Transform DebugBall;
    public static ButtonCollisionTracker Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindAnyObjectByType<ButtonCollisionTracker>();
            }
            return instance;
        }
    }
    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        instance.Setup();
    }
    public enum touchPhase
    {
        start,
        end
    }
    public void AddWorldButton(Collider button, int level)
    {
        WorldButons.TryAdd(button, level);
    }
    private Dictionary<int, Collider> fingerToButton = new();
    public bool IsntTouchingButton(int fingerId, Vector2 touchPos, touchPhase phase)    //returns true if the click is not touching any clickable ui items, false if it is
    {
        RemoveNullButtons();
        Vector3 worldPos = new Vector3(touchPos.x, touchPos.y);
        Ray ray = camera.ScreenPointToRay(worldPos);
        //Physics.RaycastAll(ray, 100f);
        //worldPos = camera.ScreenPointToRay(worldPos);
        //DebugBall.position = camera.transform.position + (worldPos * 10);
        //Debug.Log(DebugBall.position);
        RaycastHit[] hits = Physics.RaycastAll(ray, 100f);
        Debug.Log("hits: " + hits.Length);
        int bestClickLayer = -1;
        Collider bestClickCol = null;
        foreach (RaycastHit hit in hits)
            if (WorldButons.ContainsKey(hit.collider))
            {
                if((bestClickLayer == -1) || (WorldButons[hit.collider] < bestClickLayer))
                {
                    bestClickLayer = WorldButons[hit.collider];
                    bestClickCol = hit.collider;
                }
            }
        if(bestClickLayer != -1)
        {
            if (fingerToButton.ContainsKey(fingerId))
                fingerToButton[fingerId] = bestClickCol;
            else fingerToButton.Add(fingerId, bestClickCol);
            if (bestClickCol.gameObject.TryGetComponent<CaptureButton>(out CaptureButton captureButton))
                if (!captureButton.StartPress())
                {
                    fingerToButton.Remove(fingerId);
                    return true;
                }
            return false;
        }
        return true;
    }
    private void RemoveNullButtons()
    {
        List<int> fingersToRemove = new();
        foreach (KeyValuePair<int, Collider> pair in fingerToButton)
        {
            if (pair.Value == null)
                fingersToRemove.Add(pair.Key);
        }
        foreach (int p in fingersToRemove)
        {
            fingerToButton.Remove(p);
        }
        foreach (var key in WorldButons.Keys.ToArray())
        {
            if (key == null)
            {
                WorldButons.Remove(key);
            }
        }
    }
    public void EndTouchingButton(int fingerId, touchPhase phase)    //returns true if the click is not touching any clickable ui items, false if it is
    {
        RemoveNullButtons();
        if (fingerToButton.ContainsKey(fingerId))
        {
            if (fingerToButton[fingerId] != null)
            {
                if (fingerToButton[fingerId].gameObject.TryGetComponent<CaptureButton>(out CaptureButton captureButton))
                    captureButton.EndPress();
            }
            fingerToButton.Remove(fingerId);
        }
    }
    private void Setup()
    {

    }
}
