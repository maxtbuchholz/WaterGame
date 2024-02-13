using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class HealthController : MonoBehaviour
{
    private float currentHealth;
    private float maxHealth = 100;
    [SerializeField] GameObject healthBarPrefab;
    [SerializeField] Camera camera;
    [SerializeField] LocalTeamController localTeamController;
    [SerializeField] bool isFort = true;
    [SerializeField] FortTurretControl fortTurretControl;
    [SerializeField] Renderer fortRenderer;
    [SerializeField] GameObject fortCaptureCircle;
    [SerializeField] GameObject captureButtonPrefab;
    [SerializeField] GameObject fortExplosion;
    [SerializeField] GameObject fortCapturedParticle;
    private GameObject healthBar;
    private Transform MoveBar;
    private bool healthRefiling = false;
    private FindTargetController findTarget;
    private TeamsController teamsController;
    private void Start()
    {
        teamsController = TeamsController.Instance;
        findTarget = FindTargetController.Instance;
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
    private int teamId = -1;
    public void SetTeam(int teamId)
    {
        if (findTarget == null) findTarget = FindTargetController.Instance;
        if (findTarget == null) return;
        if (teamId != -1) findTarget.ModifyTargetable(this.gameObject, teamId, isFort ? FindTargetController.targetType.fort : FindTargetController.targetType.ship, FindTargetController.targetContition.destoyed);
        this.teamId = teamId;
        findTarget.ModifyTargetable(this.gameObject, teamId, isFort ? FindTargetController.targetType.fort : FindTargetController.targetType.ship, FindTargetController.targetContition.targetable);
    }
    public void EffectHealth(float change, int attackerTeamId)
    {
        if (teamId == -1) return;
        if (teamId == attackerTeamId) return;
        if (healthRefiling) return;
        currentHealth = Mathf.Min(currentHealth + change, maxHealth);
        currentHealth = Mathf.Max(currentHealth, 0);
        MoveBar.localPosition = new Vector3(0, -(1 - (currentHealth / maxHealth)), 0);
        if(currentHealth == 0)
        {
            if (isFort)
            {
                GameObject.Instantiate(fortExplosion, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                healthRefiling = true;
                if (attackerTeamId == 0)
                    StartCoroutine(FortRefilHealthOrCapture(20.0f, currentHealth));
                else
                {
                    FortCaptuerdByAI(attackerTeamId);
                    GameObject fC = GameObject.Instantiate(fortCapturedParticle, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                    fC.GetComponent<VisualEffect>().SetVector4("_TeamColor", teamsController.GetTeamColor(attackerTeamId));
                }
            }
            else
            {           //destroy ship
                findTarget.ModifyTargetable(this.gameObject, teamId, FindTargetController.targetType.ship, FindTargetController.targetContition.destoyed);
            }
        }
    }
    private void FortCaptuerdByAI(int attackerTeamId)
    {
        findTarget.ModifyTargetable(this.gameObject, teamId, FindTargetController.targetType.fort, FindTargetController.targetContition.destoyed);
        localTeamController.ForceChangeTeam(attackerTeamId);
        findTarget.ModifyTargetable(this.gameObject, teamId, FindTargetController.targetType.fort , FindTargetController.targetContition.targetable);
        currentHealth = maxHealth;
        MoveBar.localPosition = new Vector3(0, -(1 - (currentHealth / maxHealth)), 0);
    }
    public void PlayerCaptureBase()
    {
        baseCaptured = true;
    }
    private bool baseCaptured = false;
    IEnumerator FortRefilHealthOrCapture(float timeToFill, float curMin)        //starts play cycle for a player to capture a fort
    {
        GameObject captureButton = GameObject.Instantiate(captureButtonPrefab);
        captureButton.GetComponent<CaptureButton>().inFrontOf = transform.position + new Vector3(0, 0.5f, 0);
        captureButton.GetComponent<CaptureButton>().healthController = this;
        captureButton.transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
        GameObject captureCircle = GameObject.Instantiate(fortCaptureCircle);
        captureCircle.transform.position = new Vector3(transform.position.x, 0.05f, transform.position.z);
        Material cirMat = captureCircle.GetComponent<Renderer>().material;
        findTarget.ModifyTargetable(this.gameObject, teamId, FindTargetController.targetType.fort, FindTargetController.targetContition.invulnerable);
        fortTurretControl.SetEnabled(false);
        float alphaCycleTime = 1.0f;
        float currTime = 0.0f;
        float healthPerSecond = (maxHealth - curMin) / timeToFill;
        Material barMat = MoveBar.GetComponent<Renderer>().material;
        Color barCol = barMat.GetColor("_BaseColor");
        Material[] fortMaterals = fortRenderer.materials;
        Material forMat = new(barMat);
        Color forCol = Color.gray;
        foreach (Material mat in fortMaterals)
        {
            if (mat.name == "fortbrick (Instance)")
            {
                forMat = mat;
                forCol = mat.GetColor("_BaseColor");
            }
        }
        cirMat.SetColor("_Color", barCol);
        forMat.SetColor("_BaseColor", Color.Lerp(forCol, Color.gray, 0.75f));
        while ((currentHealth < maxHealth) && !baseCaptured)
        {
            cirMat.SetFloat("_CaptureHealTime", currentHealth / maxHealth);
            currTime += Time.deltaTime;
            currTime %= alphaCycleTime;
            currentHealth = Mathf.Min(maxHealth, currentHealth + Time.deltaTime * healthPerSecond);
            MoveBar.localPosition = new Vector3(0, -(1 - (currentHealth / maxHealth)), 0);
            barCol.a = (((Mathf.Sin((currTime / alphaCycleTime) * Mathf.PI * 2) + 1) / 2) * 0.6f) + 0.4f; ;
            barMat.SetColor("_BaseColor", barCol);
            if(baseCaptured)
                currentHealth = maxHealth;
            yield return null;
        }
        if (!baseCaptured)              //base back to normal
        {
            forMat.SetColor("_BaseColor", forCol);
            barCol.a = 1;
            barMat.SetColor("_BaseColor", barCol);
        }
        else
        {                           //base captured by player
            localTeamController.ForceChangeTeam(0);
            currentHealth = maxHealth;
            MoveBar.localPosition = new Vector3(0, -(1 - (currentHealth / maxHealth)), 0);
            Color color = TeamsController.Instance.GetTeamColor(0);
            forMat.SetColor("_BaseColor", color);
            barCol.a = 1;
            barMat.SetColor("_BaseColor", color);
            GameObject fC = GameObject.Instantiate(fortCapturedParticle, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            fC.GetComponent<VisualEffect>().SetVector4("_TeamColor", teamsController.GetTeamColor(0));
        }
        healthRefiling = false;
        fortTurretControl.SetEnabled(true);
        findTarget.ModifyTargetable(this.gameObject, teamId, FindTargetController.targetType.fort, FindTargetController.targetContition.targetable);
        Destroy(captureButton);
        Destroy(captureCircle);
        baseCaptured = false;
    }
}
