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
        { 0, new Dictionary<int, float>(){ { 0, 15 }, { 1, 20 }, { 2, 25 }, { 3, 30 }, { 4, 40 }, } },
        { 1, new Dictionary<int, float>(){ { 0, 18 }, { 1, 23 }, { 2, 28 }, { 3, 33 }, { 4, 38 }, } },
        { 2, new Dictionary<int, float>(){ { 0, 20 }, { 1, 25 }, { 2, 30 }, { 3, 35 }, { 4, 40 }, } },
    };
    public static Dictionary<int, Dictionary<int, float>> hea_level_effect = new()
    {
        { 0, new Dictionary<int, float>(){ { 0, 150 }, { 1, 175 }, { 2, 200 }, { 3, 225 }, { 4, 250 }, { 5, 300 } } },
        { 1, new Dictionary<int, float>(){ { 0, 200 }, { 1, 225 }, { 2, 250 }, { 3, 300 }, { 4, 325 }, { 5, 350 } } },
        { 2, new Dictionary<int, float>(){ { 0, 300 }, { 1, 325 }, { 2, 350 }, { 3, 400 }, { 4, 450 }, { 5, 500 } } },
    };
    public static Dictionary<int, Dictionary<int, float>> spe_level_effect = new()
    {
        { 0, new Dictionary<int, float>(){ { 0, 2.0f }, { 1, 2.2f }, { 2, 2.4f }, { 3, 2.6f }, } },
        { 1, new Dictionary<int, float>(){ { 0, 2.2f }, { 1, 2.4f }, { 2, 2.6f }, { 3, 2.8f }, } },
        { 2, new Dictionary<int, float>(){ { 0, 2.4f }, { 1, 2.6f }, { 2, 2.8f }, { 3, 3.0f }, } },
    };
    public static Dictionary<int, Dictionary<int, float>> dam_cost_to_next_level = new()
    {
        { 0, new Dictionary<int, float>(){ { 0, 80 }, { 1,  100 }, { 2, 120 }, { 3, 150 } } },
        { 1, new Dictionary<int, float>(){ { 0, 80 }, { 1, 120 }, { 2, 160 }, { 3, 200 } } },
        { 2, new Dictionary<int, float>(){ { 0, 120 }, { 1, 180 }, { 2, 260 }, { 3, 300 } } },
    };
    public static Dictionary<int, Dictionary<int, float>> hea_cost_to_next_level = new()
    {
        { 0, new Dictionary<int, float>(){ { 0, 50 }, { 1, 100 }, { 2, 150 }, { 3, 200 }, { 4, 250 }, { 5, 300 } } },
        { 1, new Dictionary<int, float>(){ { 0, 75 }, { 1, 150 }, { 2, 200 }, { 3, 250 }, { 4, 300 }, { 5, 350 } } },
        { 2, new Dictionary<int, float>(){ { 0, 100 }, { 1, 150 }, { 2, 250 }, { 3, 300 }, { 4, 350 }, { 5, 400 } } },
    };
    public static Dictionary<int, Dictionary<int, float>> spe_cost_to_next_level = new()
    {
        { 0, new Dictionary<int, float>(){ { 0, 25 }, { 1, 50 }, { 2, 75 } } },
        { 1, new Dictionary<int, float>(){ { 0, 50 }, { 1, 80 }, { 2, 125 } } },
        { 2, new Dictionary<int, float>(){ { 0, 75 }, { 1, 125 }, { 2, 200 } } },
    };
}
