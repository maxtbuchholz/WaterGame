using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    [SerializeField] Transform overlayTransform;
    [SerializeField] GameObject binaryPopupPrefab;
    [SerializeField] GameObject backroundUIPrefab;
    [SerializeField] GameObject loadingScreenPrefab;
    private static PopupManager instance;
    public static PopupManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindAnyObjectByType<PopupManager>();
            }
            return instance;
        }
    }
    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    //private async void Start()
    //{
    //    int popKey = PopupManager.Instance.SummonBinaryPopup("clicked");
    //    bool result = await PopupManager.Instance.AwaitUserBinaryInput(popKey);
    //    Debug.Log(result);
    //    if(result) PopupManager.Instance.EndBinaryPopup(popKey);
    //}
    Dictionary<int, GameObject> popups = new();
    int popupsKeyIncriment = 0;
    public int SummonBinaryPopup(string text)
    {
        ButtonCollisionTracker.Instance.AddTypicalButtonBlocker();
        GameObject uiRect = GameObject.Instantiate(backroundUIPrefab);
        GameObject popup = GameObject.Instantiate(binaryPopupPrefab);
        popup.transform.parent = uiRect.transform;
        popup.transform.localPosition = Vector3.zero;
        popup.GetComponent<BinaryPopupResize>().SetKey(popupsKeyIncriment);
        popup.GetComponent<BinaryPopupResize>().SetText(text);
        uiRect.name = "popupBack";
        uiRect.transform.parent = overlayTransform;
        uiRect.transform.SetSiblingIndex(0);
        popups.Add(popupsKeyIncriment, uiRect);
        popupsKeyIncriment++;
        return popupsKeyIncriment - 1;
    }
    Dictionary<int, bool> awaitedResults = new();
    public async Task<bool> AwaitUserBinaryInput(int key)
    {
        while (true)
        {
            if (awaitedResults.ContainsKey(key))
            {
                bool result = awaitedResults[key];
                awaitedResults.Remove(key);
                return result;
            }
            await Task.Delay(10);
        }
    }
    public void SignalEndBinary(int key, bool result)
    {
        awaitedResults.Add(key, result);
    }
    public void EndBinaryPopup(int key)
    {
        ButtonCollisionTracker.Instance.RemoveTypicalButtonBlocker();
        GameObject pop = popups[key];
        Destroy(pop);
        popups.Remove(key);
    }
    Transform loadingScreen;
    public void SummonLoadingScreen()
    {
        GameObject loadSc = GameObject.Instantiate(loadingScreenPrefab);
        loadSc.transform.parent = overlayTransform;
        loadSc.GetComponent<ResizeScreenImage>().ResizeImage();
        loadingScreen = loadSc.transform;
        loadSc.transform.SetSiblingIndex(0);
    }
    public void EndLoadingScreen()
    {
        StartCoroutine(FadeLoadingScreen());
    }
    float loadingScreenFadeTime = 0.5f;
    private IEnumerator FadeLoadingScreen()
    {
        ResizeScreenImage rSI = loadingScreen.GetComponent<ResizeScreenImage>();

        float currTime = 0;
        while (currTime < loadingScreenFadeTime)
        {
            rSI.SetOpacity(1 - (currTime / loadingScreenFadeTime));
            currTime += Time.deltaTime;
            Debug.Log(currTime);
            yield return null;
        }
        rSI.SetOpacity(0);
        Destroy(loadingScreen.gameObject);
    }
}
