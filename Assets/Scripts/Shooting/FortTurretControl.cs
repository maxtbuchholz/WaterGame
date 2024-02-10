using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FortTurretControl : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] List<TurretController> turrets;
    float time = 0.0f;
    private void Update()
    {
        time += Time.deltaTime;
        if(time >= 1)
        {
            time %= 1;
            if (target.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                List<int> ableTurretIndexes = new();
                List<Vector3> turretNormalVec = new();
                TurretController.ShootAbility shootAbility;
                for (int i = 0; i < turrets.Count; i++)
                {
                    Vector3 normalVec = turrets[i].RequestShot(new Vector2(target.position.x, target.position.z), new Vector2(rb.velocity.x, rb.velocity.z), out shootAbility);
                    if(shootAbility == TurretController.ShootAbility.able)
                    {
                        ableTurretIndexes.Add(i);
                        turretNormalVec.Add(normalVec);
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
    }
    IEnumerator FireProjectiles(Queue<KeyValuePair<int, Vector3>> turretShootQueue)
    {
        while(turretShootQueue.Count > 0)
        {
            KeyValuePair<int, Vector3> turNor = turretShootQueue.Dequeue();
            turrets[turNor.Key].ShootProjectile(turNor.Value);
            yield return new WaitForSeconds(Random.Range(0.02f, 0.15f));
        }
    }
}
