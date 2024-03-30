using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindTargetController : MonoBehaviour
{
    private static FindTargetController instance;

    public static FindTargetController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindAnyObjectByType<FindTargetController>();
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
        instance.Setup();
        instance.StartCoroutine(instance.FixDicts());
    }
    public enum targetContition
    {
        targetable,
        invulnerable,
        destoyed,
    }
    public enum targetType
    {
        ship,
        fort,
    }
    public IEnumerator FixDicts()
    {
        while(true)
        {
            CheckDictsForNull();
            yield return new WaitForSeconds(1);
        }
    }
    public Transform GetTarget(Vector3 origin, int originTeamId, targetType originType)
    {
        GameObject bestTarget = null;
        float bestDistance = -1;
        foreach(KeyValuePair<int, Dictionary<GameObject, targetContition>> teamList in teamShips)       //go through ships
        {
            if(teamList.Key != originTeamId)
                foreach(KeyValuePair<GameObject, targetContition> ship in teamList.Value)       //go through ships
                {
                    if (ship.Key != null)
                        if (ship.Value == targetContition.targetable)
                    {
                        Vector3 nor = (ship.Key.transform.position) - origin;
                        float tempDst = nor.magnitude;
                        nor = nor.normalized;
                        RaycastHit[] hits = Physics.RaycastAll(origin, nor, tempDst);
                        bool stillViewable = true;
                        foreach(RaycastHit hit in hits)
                            stillViewable &= !hit.collider.gameObject.CompareTag("Land");
                        if(stillViewable && ((bestDistance == -1) || (tempDst < bestDistance)))
                        {
                            bestTarget = ship.Key;
                            bestDistance = tempDst;
                        }
                    }
                }
        }
        origin.y = 0;
        if(originType != targetType.fort)
        foreach (KeyValuePair<int, Dictionary<GameObject, targetContition>> teamList in teamForts)       //go through ships
        {
            if (teamList.Key != originTeamId)
                foreach (KeyValuePair<GameObject, targetContition> fort in teamList.Value)       //go through ships
                {
                    if(fort.Key != null)
                    if (fort.Value == targetContition.targetable)
                    {
                        Vector3 targetPos = fort.Key.transform.position;
                        targetPos.y = 0;
                        Vector3 nor = targetPos - origin;
                        float tempDst = nor.magnitude;
                        nor = nor.normalized;
                        RaycastHit[] hits = Physics.RaycastAll(origin, nor, tempDst);
                        ///Debug.DrawRay(origin, nor * tempDst, Color.red, 1.0f);
                        bool stillViewable = true;
                        foreach (RaycastHit hit in hits)
                            stillViewable &= !hit.collider.gameObject.CompareTag("Land");
                        if (stillViewable && ((bestDistance == -1) || (tempDst < bestDistance)))
                        {
                            bestTarget = fort.Key;
                            bestDistance = tempDst;
                        }
                    }
                }
        }
        if (bestTarget == null) return null;
        return bestTarget.transform;
    }
    private void CheckDictsForNull()
    {
        foreach (KeyValuePair<int, Dictionary<GameObject, targetContition>> teamList in teamShips)       //go through ships
        {
            teamShips[teamList.Key] = RemoveNullKeys(teamList.Value);
        }
        foreach (KeyValuePair<int, Dictionary<GameObject, targetContition>> teamList in teamForts)       //go through ships
        {
            teamShips[teamList.Key] = RemoveNullKeys(teamList.Value);
        }
    }
    private Dictionary<GameObject, targetContition> RemoveNullKeys(Dictionary<GameObject, targetContition> dictionary)
    {
        Dictionary<GameObject, targetContition> newDict = new();
        KeyValuePair<GameObject, targetContition>[] dArray = dictionary.ToArray();

        foreach (var key in dArray)
        {
            if (key.Key != null)
            {
                newDict.Add(key.Key, key.Value);
            }
        }
        return newDict;
    }
    public void ModifyTargetable(GameObject ob, int teamId, targetType type, targetContition condition)
    {
        if(type == targetType.ship)     //if ship
        {
            if (condition == targetContition.targetable)
            {
                if (!teamShips.ContainsKey(teamId)) teamShips.Add(teamId, new Dictionary<GameObject, targetContition>());
                if (teamShips[teamId].ContainsKey(ob)) teamShips[teamId][ob] = condition;
                else teamShips[teamId].Add(ob, condition);
            }
            else
            {
                if (teamShips.ContainsKey(teamId))
                    if (teamShips[teamId].ContainsKey(ob))
                        teamShips[teamId].Remove(ob);
            }
        }
        else
        {                               //if fort
            if (condition == targetContition.targetable)
            {
                if (!teamForts.ContainsKey(teamId)) teamForts.Add(teamId, new Dictionary<GameObject, targetContition>());
                if (teamForts[teamId].ContainsKey(ob)) teamForts[teamId][ob] = condition;
                else teamForts[teamId].Add(ob, condition);
            }
            else
            {
                if (teamForts.ContainsKey(teamId))
                    if (teamForts[teamId].ContainsKey(ob))
                        teamForts[teamId].Remove(ob);
            }
        }
    }
    private Dictionary<int, Dictionary<GameObject, targetContition>> teamShips = new();
    private Dictionary<int, Dictionary<GameObject, targetContition>> teamForts = new();
    private void Setup()
    {

    }
}
