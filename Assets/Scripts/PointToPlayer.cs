using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointToPlayer : MonoBehaviour
{
    [SerializeField] Transform focalPoint;
    [SerializeField] Transform playerShip;
    private static PointToPlayer instance;
    public static PointToPlayer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindAnyObjectByType<PointToPlayer>();
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
    public Transform GetFocalPoint()
    {
        return focalPoint;
    }
    public Transform GetPlayerShip()
    {
        return playerShip;
    }
}
