using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] Image healthBarImage;
    [SerializeField] HealthBarVisability healthBarVisability;
    [SerializeField] NavTouch navTouch;
    [SerializeField] GameObject destroyedEffect;
    [SerializeField] MainCanvas mainCanvas;
    Transform MoveBar;
    public static PlayerHealthBar Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    public void SetColor(Color color)
    {
        healthBarImage.color = color;
    }
    private void Start()
    {
        healthBarImage.color = TeamsController.Instance.GetTeamColor(0);
        MoveBar = healthBarVisability.GetMoveBar().transform;
    }
    public void SethealthPercent(float perc)
    {
        if (MoveBar == null) return;
        Rect rect = MoveBar.GetComponent<RectTransform>().rect;
        MoveBar.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.width, perc);
        if (perc == 0)
            PlayerDestroyed();
    }
    bool screenUp = false;
    GameObject smoke;
    private void PlayerDestroyed()
    {
        if (screenUp) return;
        mainCanvas.DisableCanvas();
        smoke = GameObject.Instantiate(destroyedEffect);
        smoke.transform.parent = PointToPlayer.Instance.GetPlayerShipValues().gameObject.transform;
        smoke.transform.localPosition = Vector3.zero;
        FindTargetController.Instance.ModifyTargetable(PointToPlayer.Instance.GetPlayerShipValues().gameObject, 0, FindTargetController.targetType.ship, FindTargetController.targetContition.destoyed);
        navTouch.ResetTouch();
        int friendlyFortAmount = SaveData.Instance.GetAmountOfPlayerFortTeams();
        if(friendlyFortAmount > 0)                          //respawn player at friendly fort
        {
            PopupManager.Instance.SummonRespawnScreen();
        }
        else
        {                                                   //start new game
            PopupManager.Instance.SummonNewGameScreen();
        }
        screenUp = true;
    }
    public async void RespawnButtonPressed()
    {
        PopupManager.Instance.EndRespawnScreen();
        string key = SaveData.Instance.GetClosestFriendlyFort(PointToPlayer.Instance.GetPlayerShip().transform.position);
        PointToPlayer.Instance.GetPlayerShipValues().GetComponent<HealthController>().EffectHealth(999999, 0);
        await WaitFastTravel(SaveData.Instance.GetFortPos(key));
        FindTargetController.Instance.ModifyTargetable(PointToPlayer.Instance.GetPlayerShipValues().gameObject, 0, FindTargetController.targetType.ship, FindTargetController.targetContition.targetable);
        screenUp = false;
        if (smoke != null)
            Destroy(smoke);
        mainCanvas.EnableCanvas();
    }
    public void NewGameButtonPressed()
    {
        PopupManager.Instance.EndNewGameScreen();
        FindTargetController.Instance.ModifyTargetable(PointToPlayer.Instance.GetPlayerShipValues().gameObject, 0, FindTargetController.targetType.ship, FindTargetController.targetContition.targetable);
        NewGame();
        screenUp = false;
        if (smoke != null)
            Destroy(smoke);
        mainCanvas.EnableCanvas();
    }
    public async void NewGame()
    {
        DeleteFilesRecur(Application.persistentDataPath + "/");
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        PopupManager.Instance.SummonLoadingScreen();
        await SceneManager.LoadSceneAsync(sceneIndex);
        PopupManager.Instance.EndLoadingScreen();
    }
    private void DeleteFilesRecur(string pathName)
    {
        string[] dirs = Directory.GetDirectories(pathName);
        foreach (string path in dirs)
            DeleteFilesRecur(path + "/");
        string[] files = Directory.GetFiles(pathName);
        for (int i = 0; i < files.Length; i++)
        {
            File.Delete(files[i]);
        }
    }
    private Vector2[] spawnChecks = new Vector2[] { new Vector2(20, 0), new Vector2(0, 20), new Vector2(-20, 0), new Vector2(0, -20),
                                                    new Vector2(15, 15), new Vector2(-15, 15), new Vector2(-15, -15), new Vector2(15, -15)};
    public async Task WaitFastTravel(Vector3 pos)
    {
        PopupManager.Instance.SummonLoadingScreen();
        GameObject tempFocalFollow = new GameObject();
        tempFocalFollow.transform.position = pos;
        PointToPlayer.Instance.GetFocalPoint().GetComponent<FocalPointFollow>().SetFollowTransform(tempFocalFollow.transform);
        PointToPlayer.Instance.ResetTouch();
        await Task.Delay(500);
        List<Vector3> OkaySpawnPoints = new();
        foreach (Vector2 check in spawnChecks)
        {
            bool nearPointOk = true;
            Vector3 testPos = new Vector3(pos.x + check.x, 20, pos.z + check.y);
            RaycastHit[] hits = Physics.RaycastAll(new Vector3(pos.x + check.x, 20, pos.z + check.y), Vector3.down, 20.5f);
            testPos.y = 0;
            foreach (RaycastHit hit in hits)
                if (hit.collider.CompareTag("Land"))
                    nearPointOk = false;
            if (nearPointOk)
            {
                float checkPointsPerTSD = 6;
                float angleIncriment = 360 / checkPointsPerTSD;
                float currCheckAngle = 0;
                while (currCheckAngle < 360)
                {
                    float xAdd = Mathf.Sin(currCheckAngle * Mathf.Rad2Deg);
                    float zAdd = Mathf.Cos(currCheckAngle * Mathf.Rad2Deg);
                    Vector3 dir = new Vector3(xAdd, 0, zAdd);
                    hits = Physics.RaycastAll(testPos, dir, 5);
                    foreach (RaycastHit hit in hits)
                        if (hit.collider.CompareTag("Land"))
                            nearPointOk = false;
                    currCheckAngle += angleIncriment;
                }

                if (nearPointOk)
                {
                    checkPointsPerTSD = 8;
                    angleIncriment = 360 / checkPointsPerTSD;
                    currCheckAngle = 0;
                    bool atLeastOneDstOkFar = false;
                    while (currCheckAngle < 360)
                    {
                        bool curLineOk = true;
                        float xAdd = Mathf.Sin(currCheckAngle * Mathf.Rad2Deg);
                        float zAdd = Mathf.Cos(currCheckAngle * Mathf.Rad2Deg);
                        Vector3 dir = new Vector3(xAdd, 0, zAdd);
                        hits = Physics.RaycastAll(testPos, dir, 5);
                        foreach (RaycastHit hit in hits)
                            if (hit.collider.CompareTag("Land"))
                                curLineOk = false;
                        if (curLineOk)
                        {
                            atLeastOneDstOkFar = true;
                            currCheckAngle = 360;
                        }
                        currCheckAngle += angleIncriment;
                    }
                    if (atLeastOneDstOkFar)                             //found ok place to spawn
                    {
                        OkaySpawnPoints.Add(new Vector3(pos.x + check.x, 0, pos.z + check.y));
                    }
                }
            }
        }
        if (OkaySpawnPoints.Count > 0)                                   //okay to teleport player to one of these points
        {
            int toIndex = Random.Range(0, OkaySpawnPoints.Count);
            PointToPlayer.Instance.GetPlayerShip().position = OkaySpawnPoints[toIndex];
        }
        PointToPlayer.Instance.GetFocalPoint().GetComponent<FocalPointFollow>().SetFollowTransform(PointToPlayer.Instance.GetPlayerShip());
        await Task.Delay(500);
        PopupManager.Instance.EndLoadingScreen();
        Destroy(tempFocalFollow);
    }
}
