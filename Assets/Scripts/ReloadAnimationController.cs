using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadAnimationController : MonoBehaviour
{
    [SerializeField] GameObject reloadAnimationPrefab;
    private static ReloadAnimationController instance;
    public static ReloadAnimationController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindAnyObjectByType<ReloadAnimationController>();
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
    float time = 0;
    //public void Update()
    //{
    //    time += Time.deltaTime;
    //    time %= 5;
    //    UpdateTurretAmount((int)time);
    //}
    public void UpdateTurretAmount(int amount)
    {
        if(amount > reloadObj.Count)
        {
            for(int i = reloadObj.Count; i < amount; i++)
            {
                GameObject rA = GameObject.Instantiate(reloadAnimationPrefab);
                rA.transform.parent = transform;
                rA.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0.5f);
                rA.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f);
                rA.GetComponent<RectTransform>().anchoredPosition = new Vector2((reloadObj.Count * -100) - 100, -150);
                reloadObj.Add(rA);
            }
            UpdateParentPos();
        }
        else if(reloadObj.Count > amount)
        {
            for(int i = reloadObj.Count; i > amount; i--)
            {
                Destroy(reloadObj[i - 1]);
                reloadObj.RemoveAt(i - 1);
            }
            UpdateParentPos();
        }
    }
    public void UpdateTurretReloadPercentageDone(float percentage, int turretId)  //0 = just fired, 1 = ready to fire
    {
        if (reloadObj.Count > turretId)
            if (reloadObj[turretId] != null)
                reloadObj[turretId].GetComponent<ReloadLocal>().SetReladPer(percentage);

    }
    private void UpdateParentPos()
    {
        float xPos = -GetComponent<RectTransform>().rect.width / 2;
        xPos += (reloadObj.Count * 100) + 150;
        GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, 70);

    }
    private List<GameObject> reloadObj = new();

}
