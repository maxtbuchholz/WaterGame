using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerFireControl : MonoBehaviour
{
    [SerializeField] public List<TurretController> turrets;
    [SerializeField] Camera camera;
    [SerializeField] GameObject debugBall;
    // Start is called before the first frame update
    private FindTargetController findtarget;
    private List<float> turretTimeAbleFire = new();
    private ReloadAnimationController reloadAnimationController;
    void Start()
    {
        findtarget = FindTargetController.Instance;
        findtarget.ModifyTargetable(this.gameObject, 0, FindTargetController.targetType.ship, FindTargetController.targetContition.targetable);
        reloadAnimationController = ReloadAnimationController.Instance;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        Vector3 mousePosition = Input.mousePosition;
    //        RequestShot(mousePosition);
    //        //Ray ray = camera.ScreenPointToRay(mousePosition);
    //        //RaycastHit[] hits = Physics.RaycastAll(ray);
    //        //for (int i = 0; i < hits.Length; i++)
    //        //{
    //        //    if (hits[i].transform.gameObject.tag != "IslandOuterCollider")          //found suitable target
    //        //    {
    //        //        foreach (TurretController tC in turrets)
    //        //            tC.ShootProjectile(hits[i].point);
    //        //        i = hits.Length;
    //        //    }
    //        //}
    //    }
    //}
    public void RequestShot(Vector3 touchPos)
    {
        if (teamId == -1) return;
        Ray ray = camera.ScreenPointToRay(touchPos);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        Vector3 currentBestShot = new();
        float bestShotDistance = -1;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.gameObject.tag != "IslandOuterCollider")          //found suitable target
            {
                float tempShotTagDistance = Vector3.Distance(hits[i].point, camera.transform.position);
                if ((tempShotTagDistance < bestShotDistance) || (bestShotDistance == -1))
                {
                    bestShotDistance = tempShotTagDistance;
                    currentBestShot = hits[i].point;
                }
            }
        }
        if(bestShotDistance != -1)
        {
            List<int> ableTurretIndexes = new();
            List<Vector3> turretNormalVec = new();
            TurretController.ShootAbility shootAbility;
            for (int i = 0; i < turrets.Count; i++)
            {
                Vector3 normalVec = turrets[i].RequestShot(currentBestShot, out shootAbility);
                if (turretTimeAbleFire.Count <= i) turretTimeAbleFire.Add(0);
                if (turretTimeAbleFire[i] > 0) shootAbility = TurretController.ShootAbility.reloading;
                if (shootAbility == TurretController.ShootAbility.able)
                {
                    ableTurretIndexes.Add(i);
                    turretNormalVec.Add(normalVec);
                }
            }
            StartCoroutine(FireTurrets(ableTurretIndexes, turretNormalVec));
            //for (int i = 0; i < ableTurretIndexes.Count; i++)
            //{
            //    turretTimeAbleFire[ableTurretIndexes[i]] = turrets[ableTurretIndexes[i]].ShootProjectile(turretNormalVec[i], teamId);
            //    StartCoroutine(ReloadTurret(ableTurretIndexes[i]));
            //}
            //foreach (TurretController tC in turrets)
            //    tC.RequestShot(currentBestShot);
            //debugBall.transform.position = currentBestShot;
        }
    }
    private IEnumerator FireTurrets(List<int> turretIndexs, List<Vector3> Vectors)
    {
        float fireMaxDiff = 0.015f;
        int[] fireOrder = new int[turretIndexs.Count];
        for (int i = 0; i < turretIndexs.Count; i++)
            fireOrder[i] = i;
        fireOrder = Shuffle(fireOrder);
        for(int i = 0; i < fireOrder.Length; i++)
        {
            turretTimeAbleFire[turretIndexs[fireOrder[i]]] = turrets[turretIndexs[fireOrder[i]]].ShootProjectile(Vectors[fireOrder[i]] + new Vector3(Random.Range(-fireMaxDiff, fireMaxDiff), Random.Range(-fireMaxDiff, fireMaxDiff), Random.Range(-fireMaxDiff, fireMaxDiff)), teamId);
            StartCoroutine(ReloadTurret(turretIndexs[fireOrder[i]]));
            yield return new WaitForSeconds(Random.Range(0.0f, 0.1f));
        }
    }
    int[] Shuffle(int[] texts)
    {
        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int t = 0; t < texts.Length; t++)
        {
            int tmp = texts[t];
            int r = Random.Range(t, texts.Length);
            texts[t] = texts[r];
            texts[r] = tmp;
        }
        return texts;
    }
    int prevPlayerTurretCount = 0;
    private void Update()
    {
        if(turrets.Count != prevPlayerTurretCount)
        {
            prevPlayerTurretCount = turrets.Count;
            reloadAnimationController.UpdateTurretAmount(turrets.Count);
        }
    }
    private IEnumerator ReloadTurret(int turretIndex)
    {
        float origTime = turretTimeAbleFire[turretIndex];
        while (turretTimeAbleFire[turretIndex] > 0)
        {
            yield return null;
            turretTimeAbleFire[turretIndex] -= Time.deltaTime;
            reloadAnimationController.UpdateTurretReloadPercentageDone((turretTimeAbleFire[turretIndex] / origTime) / origTime, turretIndex);
        }
        reloadAnimationController.UpdateTurretReloadPercentageDone(0, turretIndex);
        turretTimeAbleFire[turretIndex] = 0;
    }
    private int teamId = -1;
    public void SetTeamId(int teamId)
    {
        //Debug.Log(teamId);
        this.teamId = teamId;
    }
}
