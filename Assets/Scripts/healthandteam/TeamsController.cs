using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamsController : MonoBehaviour
{
    // create a private reference to T instance
    private static TeamsController instance;

    public static TeamsController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindAnyObjectByType<TeamsController>();
            }
            return instance;
        }
    }

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        instance.Setup();
    }
    private Dictionary<int, Color> teamColor;         //team id to color reperenenting team, color per change may change based on player settings
    private void Setup()
    {
        teamColor = new Dictionary<int, Color>();
        teamColor.Add(0, Color.blue);
        teamColor.Add(1, Color.red);
        teamColor.Add(2, Color.green);
        teamColor.Add(3, Color.yellow);
        teamColor.Add(4, Color.magenta);
        teamColor.Add(5, Color.cyan);
    }
    public Color GetTeamColor(int teamId)
    {
        if (teamColor.ContainsKey(teamId))
            return teamColor[teamId];
        return Color.red;
    }
}
