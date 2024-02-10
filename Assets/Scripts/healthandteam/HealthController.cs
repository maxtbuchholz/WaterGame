using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthController : MonoBehaviour
{
    private float currentHealth;
    private float maxHealth = 100;
    [SerializeField] GameObject healthBarPrefab;
    [SerializeField] Camera camera;
    [SerializeField] LocalTeamController localTeamController;
    private GameObject healthBar;
    private Transform MoveBar;
    private void Start()
    {
        currentHealth = maxHealth;
        healthBar = GameObject.Instantiate(healthBarPrefab);
        healthBar.transform.parent = transform;
        healthBar.transform.localPosition = new Vector3(0, 3, 0);
        foreach (Transform child in healthBar.transform)
        {
            if (child.name == "MoveBar")
            {
                MoveBar = child;
                localTeamController.AddObjectToColored(MoveBar.gameObject);
            }
        }
    }
    public void Update()
    {
        healthBar.transform.forward = camera.transform.forward;
        healthBar.transform.localRotation = Quaternion.Euler(healthBar.transform.localRotation.eulerAngles.x, healthBar.transform.localRotation.eulerAngles.y, -90);
    }
    public void EffectHealth(float change)
    {
        currentHealth = Mathf.Min(currentHealth + change, maxHealth);
        currentHealth = Mathf.Max(currentHealth, 0);
        MoveBar.localPosition = new Vector3(0, -(1 - (currentHealth / maxHealth)), 0);
        Debug.Log(currentHealth);
    }
}
