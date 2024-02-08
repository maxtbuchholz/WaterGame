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
                foreach (TurretController turret in turrets)
                    turret.ShootProjectile(new Vector2(target.position.x, target.position.z), new Vector2(rb.velocity.x, rb.velocity.z));
            }
        }
    }
}
