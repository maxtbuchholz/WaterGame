using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadedObjects : MonoBehaviour
{
    private static LoadedObjects instance;
    public static LoadedObjects Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindAnyObjectByType<LoadedObjects>();
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
    private Dictionary<string, GameObject> loadedForts = new();
    public void AddLoadedFort(string key, GameObject fort)
    {
        if (loadedForts.ContainsKey(key)) loadedForts[key] = fort;
        else loadedForts.Add(key, fort);
    }
    public GameObject GetLoadedFort(string key)
    {
        if (loadedForts.ContainsKey(key)) return loadedForts[key];
        return null;
    }
    private Dictionary<string, GameObject> loadedMortars = new();
    public void AddLoadedMortar(string key, GameObject mortar)
    {
        RemoveUnloadedForts();
        if (loadedMortars.ContainsKey(key)) loadedMortars[key] = mortar;
        else loadedMortars.Add(key, mortar);
    }
    public GameObject GetLoadedMortar(string key)
    {
        RemoveUnloadedMortars();
        if (loadedMortars.ContainsKey(key)) return loadedMortars[key];
        return null;
    }
    private void RemoveUnloadedForts()
    {
        List<string> unload = new();
        foreach (KeyValuePair<string, GameObject> pair in loadedForts)
        {
            if (pair.Value == null) unload.Add(pair.Key);
        }
        foreach (string key in unload)
        {
            loadedForts.Remove(key);
        }
    }
    private void RemoveUnloadedMortars()
    {
        List<string> unload = new();
        foreach(KeyValuePair<string, GameObject> pair in loadedMortars)
        {
            if (pair.Value == null) unload.Add(pair.Key);
        }
        foreach(string key in unload)
        {
            loadedMortars.Remove(key);
        }
    }
}
