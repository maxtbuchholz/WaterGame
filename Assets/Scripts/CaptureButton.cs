using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CaptureButton : MonoBehaviour
{
    Camera camera;
    public Vector3 inFrontOf = Vector3.zero;
    private ButtonCollisionTracker buttonCollisionTracker;
    [SerializeField] Collider collider;
    [SerializeField] List<SpriteRenderer> spriteRenderers;
    [SerializeField] TextMeshProUGUI text;
    private TeamsController teamsController;
    public HealthController healthController = null;
    public Transform playerShip = null;
    // Start is called before the first frame update
    void Start()
    {
        teamsController = TeamsController.Instance;
        camera = Camera.main;
        buttonCollisionTracker = ButtonCollisionTracker.Instance;
        buttonCollisionTracker.AddWorldButton(collider, 1);
        SetColor(Color.gray);
    }
    float captureMaxRange = 10f;
    bool withinRange = false;
    private void Update()
    {
        Vector3 norm = (camera.transform.position - inFrontOf).normalized;
        transform.position = inFrontOf + (norm * 5);

        if (playerShip == null)
            playerShip = FindObjectOfType<PlayerFireControl>().transform;
        if(playerShip != null)
        {
            float tempDist = Vector3.Distance(healthController.transform.position, playerShip.position);
            if((tempDist <= captureMaxRange) && !withinRange)
            {
                withinRange = true;
                SetColor(Color.white);
            }
            else if ((tempDist > captureMaxRange) && withinRange)
            {
                withinRange = false;
                SetColor(Color.gray);
            }
        }
    }
    public bool StartPress()
    {
        if (!withinRange) return false;
        Color color = teamsController.GetTeamColor(0);
        SetColor(color);
        return true;
    }
    public void EndPress()
    {
        SetColor(Color.white);
        if(healthController != null)
        {
            healthController.PlayerCaptureBase();
        }
    }
    private void SetColor(Color color)
    {
        foreach(SpriteRenderer sr in spriteRenderers)
        {
            sr.material.color = color;
            text.color = color;
        }
    }
}
