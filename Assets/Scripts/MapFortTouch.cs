using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MapFortTouch : MonoBehaviour
{
    public void Start()
    {
        ButtonCollisionTracker.Instance.AddUIButton(GetComponent<RectTransform>(), 1);
        clickedImage.enabled = false;
        selectedImage.enabled = false;
    }
    [SerializeField] RectTransform unClickedRect;
    [SerializeField] RectTransform clickedRect;
    [SerializeField] RectTransform selectedRect;
    [SerializeField] RawImage unClickedImage;
    [SerializeField] RawImage clickedImage;
    [SerializeField] RawImage selectedImage;
    [SerializeField] MapCanvas mapCanvas;
    [SerializeField] MainCanvas mainCanvas;
    public void SetSize(float w, float h)
    {
        unClickedRect.sizeDelta = new Vector2(w,h);
        clickedRect.sizeDelta = new Vector2(w, h);
        selectedRect.sizeDelta = new Vector2(w, h);
    }
    public void TouchStart()
    {
        if (ButtonCollisionTracker.Instance.GetTypicalButtonBlocked()) return;
        unClickedImage.enabled = false;
        clickedImage.enabled = true;
        selectedImage.enabled = false;
    }
    private string key;
    public void SetFortKey(string key)
    {
        this.key = key;
    }
    public void SetCanvasValues(MapCanvas mapCanvas, MainCanvas mainCanvas)
    {
        this.mapCanvas = mapCanvas;
        this.mainCanvas = mainCanvas;
    }
    public async void TouchEnd()
    {
        if (ButtonCollisionTracker.Instance.GetTypicalButtonBlocked()) return;
        unClickedImage.enabled = false;
        clickedImage.enabled = false;
        selectedImage.enabled = true;
        int popKey = PopupManager.Instance.SummonBinaryPopup("Travel To Fort?");
        bool result = await PopupManager.Instance.AwaitUserBinaryInput(popKey);
        Debug.Log(result);
        PopupManager.Instance.EndBinaryPopup(popKey);
        if (result)                                     //begin fast travel
        {
            SaveData saveData = SaveData.Instance;
            if (saveData.FortExists(key))
            {
                Vector3 fortLocation = saveData.GetFortPos(key);
                fortLocation.y = 0;
                await WaitFastTravel(fortLocation);
            }
        }
    }
    public void SetColor(Color color)
    {
        unClickedImage.color = color;
        clickedImage.color = new Color(color.r + 0.4f, color.g + 0.4f, color.b + 0.4f);
        selectedImage.color = new Color(color.r + 0.2f, color.g + 0.2f, color.b + 0.2f);
    }
    private Vector2[] spawnChecks = new Vector2[] { new Vector2(20, 0), new Vector2(0, 20), new Vector2(-20, 0), new Vector2(0, -20),
                                                    new Vector2(15, 15), new Vector2(-15, 15), new Vector2(-15, -15), new Vector2(15, -15)};
    public async Task WaitFastTravel(Vector3 pos)
    {
        PopupManager.Instance.SummonLoadingScreen();
        mapCanvas.MoveToHide();
        GameObject tempFocalFollow = new GameObject();
        tempFocalFollow.transform.position = pos;
        PointToPlayer.Instance.GetFocalPoint().GetComponent<FocalPointFollow>().SetFollowTransform(tempFocalFollow.transform);
        PointToPlayer.Instance.ResetTouch();
        await Task.Delay(500);
        List<Vector3> OkaySpawnPoints = new();
        foreach(Vector2 check in spawnChecks)
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
                while(currCheckAngle < 360)
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
        if(OkaySpawnPoints.Count > 0)                                   //okay to teleport player to one of these points
        {
            int toIndex = Random.Range(0, OkaySpawnPoints.Count);
            PointToPlayer.Instance.GetPlayerShip().position = OkaySpawnPoints[toIndex];
        }
        PointToPlayer.Instance.GetFocalPoint().GetComponent<FocalPointFollow>().SetFollowTransform(PointToPlayer.Instance.GetPlayerShip());
        await Task.Delay(500);
        mainCanvas.EnableCanvas();
        PopupManager.Instance.EndLoadingScreen();
        Destroy(tempFocalFollow);
    }
}
