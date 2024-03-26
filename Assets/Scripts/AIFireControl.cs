using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AIFireControl : MonoBehaviour
{
    [SerializeField] public List<TurretController> turrets;
    float checkWithinLoadDstOfPlayerTime = 0;
    static float checkWithinLoadDstOfPlayerBetween = 5.0f;
    static float loadDstFromPlayer = 500;
    bool withinDstOfPlayer = false;
    PointToPlayer pointToPlayer;
    FindTargetController findTargetController;
    int teamId = 1;
    private void Start()
    {
        pointToPlayer = PointToPlayer.Instance;
        findTargetController = FindTargetController.Instance;
    }
    float time = 0;
    private void Update()
    {
        checkWithinLoadDstOfPlayerTime -= Time.deltaTime;
        if (checkWithinLoadDstOfPlayerTime <= 0)
        {
            withinDstOfPlayer = false || Vector3.Distance(pointToPlayer.GetPlayerShip().position, transform.position) <= loadDstFromPlayer;
            checkWithinLoadDstOfPlayerTime = checkWithinLoadDstOfPlayerBetween + Random.Range(0, 1.0f);
        }
        if (!withinDstOfPlayer) return;
        if (teamId == -1) return;
        time -= Time.deltaTime;
        if (time <= 0)
        {
            time = 2 + Random.Range(0.0f, 1.0f);
            Transform target = findTargetController.GetTarget(transform.position + new Vector3(0, 5, 0), teamId, FindTargetController.targetType.ship);
            if (target == null) return;
            List<int> ableTurretIndexes = new();
            List<Vector3> turretNormalVec = new();
            TurretController.ShootAbility shootAbility;
            Rigidbody rb = null;
            //bool forceStraight = false;
            //if ((target.position - transform.position).magnitude < 10) forceStraight = true;
            float distanceMissAdd = 1;
            Vector3 targetPos = new Vector3(target.position.x + Random.Range(-0.5f - distanceMissAdd, 0.5f + distanceMissAdd), 0, target.position.z + Random.Range(-1.0f - distanceMissAdd, 1.0f + distanceMissAdd));
            if (target.TryGetComponent<ShipValueControl>(out ShipValueControl sVC))
            {
                rb = sVC.ship_drive.GetComponent<Rigidbody>();
                //if (rb.velocity.magnitude < 0.5f) forceStraight = true;
            }
            if (rb != null)        //targeing movable target
            {
                for (int i = 0; i < turrets.Count; i++)
                {
                    Vector3 normalVec = turrets[i].RequestShot(new Vector2(targetPos.x, targetPos.z), new Vector2(rb.velocity.x, rb.velocity.z), out shootAbility);
                    if (shootAbility == TurretController.ShootAbility.able)
                    {
                        ableTurretIndexes.Add(i);
                        turretNormalVec.Add(normalVec);
                    }
                }
            }
            else
            {
                //Vector3 targetPos = target.position;
                targetPos.y = 0;
                for (int i = 0; i < turrets.Count; i++)
                {
                    Vector3 normalVec = turrets[i].RequestShot(targetPos, out shootAbility);
                    if (shootAbility == TurretController.ShootAbility.able)
                    {
                        ableTurretIndexes.Add(i);
                        turretNormalVec.Add(normalVec);
                    }
                }
            }
            //Queue<KeyValuePair<int, Vector3>> turretShootQueue = new();
            //for (int i = 0; i < ableTurretIndexes.Count; i++)
            //{
            //    turretShootQueue.Enqueue(new KeyValuePair<int, Vector3>(ableTurretIndexes[i], turretNormalVec[i]));
            //}
            StartCoroutine(FireTurrets(ableTurretIndexes, turretNormalVec));
        }
    }
    public void SetTeamId(int newId)
    {
        teamId = newId;
    }
    private IEnumerator FireTurrets(List<int> turretIndexs, List<Vector3> Vectors)
    {
        float fireMaxDiff = 0.02f;
        int[] fireOrder = new int[turretIndexs.Count];
        for (int i = 0; i < turretIndexs.Count; i++)
            fireOrder[i] = i;
        fireOrder = Shuffle(fireOrder);
        for (int i = 0; i < fireOrder.Length; i++)
        {
            turrets[turretIndexs[fireOrder[i]]].ShootProjectile(Vectors[fireOrder[i]] + new Vector3(Random.Range(-fireMaxDiff, fireMaxDiff), Random.Range(-fireMaxDiff, fireMaxDiff), Random.Range(-fireMaxDiff, fireMaxDiff)), teamId);
            yield return new WaitForSeconds(Random.Range(0.02f, 0.125f));
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
}
