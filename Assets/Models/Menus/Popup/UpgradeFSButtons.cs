using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeFSButtons : MonoBehaviour
{
    [SerializeField] RectTransform fortButtonRect;
    [SerializeField] RectTransform shipButtonRect;
    // Start is called before the first frame update
    void Start()
    {
        ButtonCollisionTracker.Instance.AddUIButton(fortButtonRect, 0);
        ButtonCollisionTracker.Instance.AddUIButton(shipButtonRect, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Touch[] touches = Input.touches;
        bool inProperButton = true;
        foreach (Touch touch in touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                if (!GetWorldRect(fortButtonRect).Contains(touch.position) && !GetWorldRect(shipButtonRect).Contains(touch.position))
                    inProperButton = false;
            }
        }
        if (!inProperButton)
        {
            PopupManager.Instance.EndAskShipFortUpgrade(key);
        }
    }
    public void FortUpgradePressed()
    {
        PopupManager.Instance.EndAskShipFortUpgrade(key);
        PopupManager.Instance.SummonFortUpgradeScreen();
    }
    public void ShipUpgradePressed()
    {
        PopupManager.Instance.EndAskShipFortUpgrade(key);
        PopupManager.Instance.SummonShipUpgradeScreen();
    }
    private int key;
    public void SetKey(int key)
    {
        this.key = key;
    }
    private Rect GetWorldRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        // Get the bottom left corner.
        Vector3 position = corners[0];

        Vector2 size = new Vector2(
            rectTransform.lossyScale.x * rectTransform.rect.size.x,
            rectTransform.lossyScale.y * rectTransform.rect.size.y);

        return new Rect(position, size);
    }
}
