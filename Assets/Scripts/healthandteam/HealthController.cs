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
    [SerializeField] public Camera camera;
    [SerializeField] LocalTeamController localTeamController;
    [SerializeField] bool isFort = true;
    [SerializeField] bool isMortar = false;
    [SerializeField] FortTurretControl fortTurretControl;
    [SerializeField] Renderer fortRenderer;
    [SerializeField] GameObject fortCaptureCircle;
    [SerializeField] GameObject captureButtonPrefab;
    [SerializeField] GameObject fortExplosion;
    [SerializeField] GameObject fortCapturedParticle;
    [SerializeField] GameObject damageSmokePrefab;
    private HealthBarVisability healthBarVisability;
    [SerializeField]public bool isPlayer = false;
    private GameObject healthBar;
    private Transform MoveBar;
    private bool healthRefilingDuringCapture = false;
    private FindTargetController findTarget;
    private TeamsController teamsController;
    private void Start()
    {
        teamsController = TeamsController.Instance;
        findTarget = FindTargetController.Instance;
        currentHealth = maxHealth;
        if (!isPlayer)
        {
            InstanciateHealthBar();
        }
        else
        {
            SetTeam(0);
        }
    }
    private void InstanciateHealthBar()
    {
        healthBar = GameObject.Instantiate(healthBarPrefab);
        healthBar.transform.parent = transform;
        healthBar.transform.localPosition = new Vector3(0, 3, 0);
        healthBarVisability = healthBar.GetComponent<HealthBarVisability>();
        healthBarVisability.SetBarColor(teamsController.GetTeamColor(teamId, localTeamController));
        MoveBar = healthBarVisability.GetMoveBar().transform;
        localTeamController.AddObjectToColored(MoveBar.gameObject);
    }
    private float timeWithoutDamage = 10;
    private float startHealTime = 10;
    private float healAmountPerSec = 20;
    private bool healthBarAppear = true;
    public void Update()
    {
            if (healthBarAppear && !isPlayer)
            {
                if (camera == null) camera = Camera.main;
                if (healthBar == null) InstanciateHealthBar();
                healthBar.transform.forward = camera.transform.forward;
                healthBar.transform.localRotation = Quaternion.Euler(healthBar.transform.localRotation.eulerAngles.x, healthBar.transform.localRotation.eulerAngles.y, -90);
            }
            timeWithoutDamage += Time.deltaTime;
            if (!healthRefilingDuringCapture)
            {
                if (timeWithoutDamage > startHealTime)
                {
                    if (currentHealth < maxHealth)       //health bar still appear
                    {
                        currentHealth += (healAmountPerSec * Time.deltaTime);
                        currentHealth = Mathf.Min(currentHealth, maxHealth);
                        SetHealthSize(currentHealth / maxHealth);
                    }
                    else
                    {                                 //health bar eventually dissapear
                        if(!isPlayer)
                            SetHealthBarAppear(false);
                    }
                }
            }
    }
    public void SetHealthBarAppear(bool appear)
    {
        if((healthBar == null) || (MoveBar == null)) return;
        if(appear != healthBarAppear)
        {
            healthBarAppear = appear;
            healthBar.GetComponent<HealthBarVisability>().SetAppear(appear);
        }
    }
    public void UnloadShip()
    {
        findTarget.ModifyTargetable(this.gameObject, teamId, isFort ? FindTargetController.targetType.fort : FindTargetController.targetType.ship, FindTargetController.targetContition.destoyed);
    }
    private int teamId = -1;
    private bool teamChangedStopHealthRefilMortar = false;
    public void SetTeam(int newTeamId)
    {
        if(isMortar && healthRefilingDuringCapture)
            teamChangedStopHealthRefilMortar = true;
        if (findTarget == null) findTarget = FindTargetController.Instance;
        if (findTarget == null) return;
        findTarget.ModifyTargetable(this.gameObject, teamId, isFort ? FindTargetController.targetType.fort : FindTargetController.targetType.ship, FindTargetController.targetContition.destoyed);
        this.teamId = newTeamId;
        findTarget.ModifyTargetable(this.gameObject, newTeamId, isFort ? FindTargetController.targetType.fort : FindTargetController.targetType.ship, FindTargetController.targetContition.targetable);
        if (didStart)
        {
            if(!isPlayer)
                healthBarVisability.SetBarColor(teamsController.GetTeamColor(teamId, localTeamController));
        }
    }
    public void SetMaxHealth(float health)
    {
        currentHealth = health;
        maxHealth = health;
        SetHealthSize(currentHealth / maxHealth);
    }
    private void SetHealthSize(float perc)
    {
        if (isPlayer)
        {
            PlayerHealthBar.Instance.SethealthPercent(perc);
        }
        else
        {
            if (MoveBar == null) return;
            Rect rect = MoveBar.GetComponent<RectTransform>().rect;
            MoveBar.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.width, perc);
            //MoveBar.GetComponent<RectTransform>().rect
            //MoveBar.localPosition = new Vector3(0, perc * MoveBar.GetComponent<RectTransform>().rect.height, 0);
        }
    }
    public void EffectHealth(float change, int attackerTeamId)
    {
        //Debug.Log("HIT!!!" + " max:c" + maxHealth + " curr: " + currentHealth + " teamID: " + teamId + " attackID: " + attackerTeamId);
        if (teamId == -1) return;
        if (teamId == attackerTeamId) return;
        if (healthRefilingDuringCapture) return;
        timeWithoutDamage = 0;
        SetHealthBarAppear(true);
        currentHealth = Mathf.Min(currentHealth + change, maxHealth);
        currentHealth = Mathf.Max(currentHealth, 0);
        SetHealthSize(currentHealth / maxHealth);
        if(currentHealth == 0)
        {
            if (isFort && !isMortar)
            {
                GameObject.Instantiate(fortExplosion, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                healthRefilingDuringCapture = true;
                if (attackerTeamId == 0)
                    StartCoroutine(FortRefilHealthOrCapture(30.0f, currentHealth));
                else
                {
                    FortCaptuerdByAI(attackerTeamId);
                    GameObject fC = GameObject.Instantiate(fortCapturedParticle, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                    fC.GetComponent<VisualEffect>().SetVector4("_TeamColor", teamsController.GetTeamColor(attackerTeamId, localTeamController));
                }
            }
            else if (isMortar)      //different
            {
                GameObject.Instantiate(fortExplosion, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                StartCoroutine(MortarRefilHealthFromZero(20.0f, currentHealth));
            }
            else if(!isPlayer)
            {           //destroy ship
                if(attackerTeamId == 0)
                    PlayerMoneyController.Instance.AddMoney(((GetComponent<ShipValueControl>().shipModel + 1) * 100) + (Random.Range(0, 4) * 25));
                UnloadShip();
                GameObject fC = GameObject.Instantiate(fortCapturedParticle, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                fC.GetComponent<VisualEffect>().SetVector4("_TeamColor", teamsController.GetTeamColor(teamId, localTeamController));
                GameObject.Instantiate(fortExplosion, transform.position, Quaternion.identity);
                ShipSpawner.Instance.DestroyShip(this.gameObject);
                Destroy(GetComponent<ShipValueControl>().ship_drive.gameObject);
                Destroy(this.gameObject);
                findTarget.ModifyTargetable(this.gameObject, teamId, FindTargetController.targetType.ship, FindTargetController.targetContition.destoyed);
            }
        }
    }
    private void FortCaptuerdByAI(int attackerTeamId)
    {
        findTarget.ModifyTargetable(this.gameObject, teamId, FindTargetController.targetType.fort, FindTargetController.targetContition.destoyed);
        localTeamController.ForceChangeTeam(attackerTeamId);
        findTarget.ModifyTargetable(this.gameObject, attackerTeamId, FindTargetController.targetType.fort , FindTargetController.targetContition.targetable);
        currentHealth = maxHealth;
        SetHealthSize(currentHealth / maxHealth);
        healthRefilingDuringCapture = false;
        this.teamId = attackerTeamId;
        if (healthBar == null) InstanciateHealthBar();
        healthBarVisability.SetBarColor(teamsController.GetTeamColor(teamId, localTeamController));
        SaveData.Instance.SetFortTeam(GetComponent<FortValues>().GetKey(), attackerTeamId);

    }
    public void PlayerCaptureBase()
    {
        findTarget.ModifyTargetable(this.gameObject, teamId, FindTargetController.targetType.fort, FindTargetController.targetContition.destoyed);
        findTarget.ModifyTargetable(this.gameObject, 0, FindTargetController.targetType.fort, FindTargetController.targetContition.targetable);
        localTeamController.ForceChangeTeam(0);
        baseCaptured = true;
        healthRefilingDuringCapture = false;
        if (healthBar == null) InstanciateHealthBar();
        PlayerMoneyController.Instance.AddMoney(300 + (Random.Range(0, 4) * 25));
        SaveData.Instance.SetFortTeam(GetComponent<FortValues>().GetKey(), 0);
    }
    private bool baseCaptured = false;
    IEnumerator MortarRefilHealthFromZero(float timeToFill, float curMin)
    {
        GameObject smoke = GameObject.Instantiate(damageSmokePrefab);
        smoke.transform.position = transform.position;// + new Vector3(0, 1, 0);
        findTarget.ModifyTargetable(this.gameObject, teamId, FindTargetController.targetType.fort, FindTargetController.targetContition.invulnerable);
        fortTurretControl.SetEnabled(false);
        healthRefilingDuringCapture = true;
        float currTime = 0.0f;
        float alphaCycleTime = 1.0f;
        float healthPerSecond = (maxHealth - curMin) / timeToFill;
        Color barCol = healthBarVisability.GetBarColor();
        Material[] fortMaterals = fortRenderer.materials;
        Material forMat = null;
        Color forCol = Color.gray;
        foreach (Material mat in fortMaterals)
        {
            if (mat.name == "fortbrick (Instance)")
            {
                forMat = mat;
                forCol = mat.GetColor("_BaseColor");
            }
        }
        if(forMat != null)
            forMat.SetColor("_BaseColor", Color.Lerp(forCol, Color.gray, 0.75f));
        while (currentHealth < maxHealth)
        {
            currTime += Time.deltaTime;
            currTime %= alphaCycleTime;
            if (!teamChangedStopHealthRefilMortar)
            {
                currentHealth = Mathf.Min(maxHealth, currentHealth + Time.deltaTime * healthPerSecond);
                SetHealthSize(currentHealth / maxHealth);
                barCol.a = (((Mathf.Sin((currTime / alphaCycleTime) * Mathf.PI * 2) + 1) / 2) * 0.6f) + 0.4f;
                healthBarVisability.SetBarColor(barCol);
                yield return null;
            }
            else
                currentHealth = maxHealth;
        }
        currentHealth = maxHealth;
        SetHealthSize(currentHealth / maxHealth);
        if (!teamChangedStopHealthRefilMortar)
        {
            if (forMat != null)
                forMat.SetColor("_BaseColor", forCol);
            barCol.a = 1;
            healthBarVisability.SetBarColor(barCol);
        }
        teamChangedStopHealthRefilMortar = false;
        fortTurretControl.SetEnabled(true);
        findTarget.ModifyTargetable(this.gameObject, teamId, FindTargetController.targetType.fort, FindTargetController.targetContition.targetable);
        healthRefilingDuringCapture = false;
        Destroy(smoke);
    }
    IEnumerator FortRefilHealthOrCapture(float timeToFill, float curMin)        //starts play cycle for a player to capture a fort
    {
        GameObject smoke = GameObject.Instantiate(damageSmokePrefab);
        smoke.transform.position = transform.position;
        smoke.transform.localScale *= 2;
        GameObject captureButton = GameObject.Instantiate(captureButtonPrefab);
        captureButton.GetComponent<CaptureButton>().inFrontOf = transform.position + new Vector3(0, 1.0f, 0);
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
        Color barCol = healthBarVisability.GetBarColor();
        Material[] fortMaterals = fortRenderer.materials;
        Material forMat = null;
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
            SetHealthSize(currentHealth / maxHealth);
            barCol.a = (((Mathf.Sin((currTime / alphaCycleTime) * Mathf.PI * 2) + 1) / 2) * 0.6f) + 0.4f; ;
            healthBarVisability.SetBarColor(barCol);
            if (baseCaptured)
                currentHealth = maxHealth;
            yield return null;
        }
        if (!baseCaptured)              //base back to normal
        {
            forMat.SetColor("_BaseColor", forCol);
            barCol.a = 1;
            healthBarVisability.SetBarColor(barCol);
        }
        else
        {                           //base captured by player
            localTeamController.ForceChangeTeam(0);
            currentHealth = maxHealth;
            SetHealthSize(currentHealth / maxHealth);
            Color color = TeamsController.Instance.GetTeamColor(0, localTeamController);
            forMat.SetColor("_BaseColor", color);
            healthBarVisability.SetBarColor(color);
            barCol.a = 1;
            GameObject fC = GameObject.Instantiate(fortCapturedParticle, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            fC.GetComponent<VisualEffect>().SetVector4("_TeamColor", teamsController.GetTeamColor(0, localTeamController));
        }
        healthRefilingDuringCapture = false;
        fortTurretControl.SetEnabled(true);
        findTarget.ModifyTargetable(this.gameObject, teamId, FindTargetController.targetType.fort, FindTargetController.targetContition.targetable);
        Destroy(captureButton);
        Destroy(captureCircle);
        baseCaptured = false;
        Destroy(smoke);
    }
}
