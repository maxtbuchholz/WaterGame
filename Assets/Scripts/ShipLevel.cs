using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShipLevel : MonoBehaviour
{
    private int hea_level = 0;
    private int dam_level = 0;
    private int spe_level = 0;
    private SaveData saveData;
    [SerializeField] FortTurretControl fortTurretControl;
    [SerializeField] HealthController healthControl;
    [SerializeField] GameObject mortarPrefab;
    public void InitFromKey(string key)
    {
        this.key = key;
        saveData = SaveData.Instance;
        LevelsUpdate();
    }
    public void LevelsUpdate()
    {
        
    }
    private string key;
    public string GetKey()
    {
        return key;
    }
    public static Dictionary<int, Dictionary<int, float>> dam_level_effect = new()  //accessed by dam_level_effect[shipId][level]
    {
        { 0, new Dictionary<int, float>(){ { 0, 15 }, { 1, 20 }, { 2, 25 }, { 3, 30 }, } },
        { 1, new Dictionary<int, float>(){ { 0, 18 }, { 1, 23 }, { 2, 28 }, { 3, 33 }, } },
        { 2, new Dictionary<int, float>(){ { 0, 20 }, { 1, 25 }, { 2, 30 }, { 3, 35 }, } },
    };
    public static Dictionary<int, Dictionary<int, float>> hea_level_effect = new()
    {
        { 0, new Dictionary<int, float>(){ { 0, 100 }, { 1, 125 }, { 2, 150 }, { 3, 175 }, } },
        { 1, new Dictionary<int, float>(){ { 0, 150 }, { 1, 175 }, { 2, 200 }, { 3, 225 }, } },
        { 2, new Dictionary<int, float>(){ { 0, 225 }, { 1, 265 }, { 2, 305 }, { 3, 350 }, } },
    };
    public static Dictionary<int, Dictionary<int, float>> spe_level_effect = new()
    {
        { 0, new Dictionary<int, float>(){ { 0, 1.75f }, { 1, 2.0f }, { 2, 2.25f }, { 3, 2.5f }, } },
        { 1, new Dictionary<int, float>(){ { 0, 2.0f }, { 1, 2.2f }, { 2, 2.4f }, { 3, 2.6f }, } },
        { 2, new Dictionary<int, float>(){ { 0, 2.0f }, { 1, 2.3f }, { 2, 2.6f }, { 3, 3.0f }, } },
    };
    public static Dictionary<int, Dictionary<int, float>> dam_cost_to_next_level = new()
    {
        { 0, new Dictionary<int, float>(){ { 0, 50 }, { 1, 100 }, { 2, 150 } } },
        { 1, new Dictionary<int, float>(){ { 0, 75 }, { 1, 150 }, { 2, 200 } } },
        { 2, new Dictionary<int, float>(){ { 0, 100 }, { 1, 150 }, { 2, 250 } } },
    };
    public static Dictionary<int, Dictionary<int, float>> hea_cost_to_next_level = new()
    {
        { 0, new Dictionary<int, float>(){ { 0, 50 }, { 1, 100 }, { 2, 150 } } },
        { 1, new Dictionary<int, float>(){ { 0, 75 }, { 1, 150 }, { 2, 200 } } },
        { 2, new Dictionary<int, float>(){ { 0, 100 }, { 1, 150 }, { 2, 250 } } },
    };
    public static Dictionary<int, Dictionary<int, float>> spe_cost_to_next_level = new()
    {
        { 0, new Dictionary<int, float>(){ { 0, 50 }, { 1, 100 }, { 2, 150 } } },
        { 1, new Dictionary<int, float>(){ { 0, 75 }, { 1, 150 }, { 2, 200 } } },
        { 2, new Dictionary<int, float>(){ { 0, 100 }, { 1, 150 }, { 2, 250 } } },
    };
}
