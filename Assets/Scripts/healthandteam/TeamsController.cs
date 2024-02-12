using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamsController : MonoBehaviour
{
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
        teamColor = new Dictionary<int, Color>
        {
            { 0, Color.blue },
            { 1, Color.red },
            { 2, Color.green },
            { 3, Color.yellow },
            { 4, Color.magenta },
            { 5, Color.cyan }
        };
    }
    public Color GetTeamColor(int teamId)
    {
        if (teamColor.ContainsKey(teamId))
            return teamColor[teamId];
        return Color.red;
    }
}
