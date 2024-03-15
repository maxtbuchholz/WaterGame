using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FortTurretControl : MonoBehaviour
{
    private Transform target;
    [SerializeField] List<TurretController> turrets;
    private FindTargetController findTargetController;

    float time = 0.0f;
    private bool gunsEnabled = true;
    private void Start()
    {
        findTargetController = FindTargetController.Instance;
    }
    public void SetDamage(float damage)
    {
        foreach(TurretController turret in turrets)
        {
            turret.SetDamage(damage);
        }
    }
    private void Update()
    {
        if (!gunsEnabled) return;
        if (teamId == -1) return;
        time += Time.deltaTime;
        if(time >= 1)
        {
            time %= 1;
            target = findTargetController.GetTarget(transform.position + new Vector3(0, 5, 0), teamId, FindTargetController.targetType.fort);
            if (target == null) return;
            List<int> ableTurretIndexes = new();
            List<Vector3> turretNormalVec = new();
            TurretController.ShootAbility shootAbility;
            Rigidbody rb = null;
            //bool forceStraight = false;
            //if ((target.position - transform.position).magnitude < 10) forceStraight = true;
            float distanceMissAdd = Vector3.Distance(transform.position, target.position) / 20;
            Vector3 targetPos = new Vector3(target.position.x + Random.Range(-0.5f - distanceMissAdd, 0.5f + distanceMissAdd), 0, target.position.z + Random.Range(-1.0f - distanceMissAdd, 1.0f + distanceMissAdd));
            if(target.TryGetComponent<ShipValueControl>(out ShipValueControl sVC))
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
            Queue<KeyValuePair<int, Vector3>> turretShootQueue = new();
            for (int i = 0; i < ableTurretIndexes.Count; i++)
            {
                turretShootQueue.Enqueue(new KeyValuePair<int, Vector3>(ableTurretIndexes[i], turretNormalVec[i]));
            }
            StartCoroutine(FireProjectiles(turretShootQueue));
        }
    }
    IEnumerator FireProjectiles(Queue<KeyValuePair<int, Vector3>> turretShootQueue)
    {
        float fireMaxDiff = 0.02f;
        while (turretShootQueue.Count > 0)
        {
            KeyValuePair<int, Vector3> turNor = turretShootQueue.Dequeue();
            turrets[turNor.Key].ShootProjectile(turNor.Value + new Vector3(Random.Range(-fireMaxDiff, fireMaxDiff), Random.Range(-fireMaxDiff, fireMaxDiff), Random.Range(-fireMaxDiff, fireMaxDiff)), teamId);
            yield return new WaitForSeconds(Random.Range(0.02f, 0.15f));
        }
    }
    public void SetEnabled(bool enabled)
    {
        gunsEnabled = enabled;
    }
    private int teamId = -1;
    public void SetTeam(int teamId)
    {
        this.teamId = teamId;
        SaveData saveData = SaveData.Instance;
        string fortId = saveData.GetFortKeyFromPos(transform.position);
        if (fortId != "")
        {
            saveData.SetFortTeam(fortId, teamId);
        }
    }
}
