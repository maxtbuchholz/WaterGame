using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Playables;

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
        string destination = Application.persistentDataPath + "/teamColors.txt";
        FileStream file;
        bool fileNotLoading = false;
        if (File.Exists(destination))
        {
            try
            {
                file = File.OpenRead(destination);
                BinaryFormatter bf = new BinaryFormatter();
                var read = bf.Deserialize(file);
                file.Close();
                teamColor = SemiDeserializeColorDict((Dictionary<int, float[]>)read);
            }
            catch(System.Exception e)
            {
                Debug.Log(e.Message);
                fileNotLoading = true;
            }
        }
        else fileNotLoading = true;
        if(fileNotLoading)
        {
            if (File.Exists(destination)) file = File.OpenWrite(destination);
            else file = File.Create(destination);

            teamColor = new Dictionary<int, Color>
            {
            { 0, Color.blue },
            { 1, Color.red },
            { 2, Color.green },
            { 3, Color.yellow },
            { 4, Color.magenta },
            { 5, Color.cyan }
            };

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, SemiSerializeColorDict(teamColor));
            file.Close();
        }
    }
    public Dictionary<int, Color> GetAllColors()
    {
        return teamColor;
    }
    public void SetAllColors(Dictionary<int, Color> teamColor)
    {
        this.teamColor = teamColor;
        PlayerHealthBar.Instance.SetColor(teamColor[0]);
        for (int i = localTeams.Count - 1; i >= 0; i--)
        {
            if (localTeams[i] == null)
                localTeams.RemoveAt(i);
        }
        string destination = Application.persistentDataPath + "/teamColors.txt";
        FileStream file;
        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);


        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, SemiSerializeColorDict(teamColor));
        file.Close();
        foreach (LocalTeamController localTeam in localTeams)
            localTeam.SetGameObjectColors();
    }
    List<LocalTeamController> localTeams = new();
    public Color GetTeamColor(int teamId, LocalTeamController localTeam)
    {
        if (!localTeams.Contains(localTeam)) localTeams.Add(localTeam);
        if (teamColor.ContainsKey(teamId))
            return teamColor[teamId];
        return Color.red;
    }
    public void AddLocalTeam(LocalTeamController localTeam)
    {
        if (!localTeams.Contains(localTeam)) localTeams.Add(localTeam);
    }
    public Color GetTeamColor(int teamId)
    {
        if (teamColor.ContainsKey(teamId))
            return teamColor[teamId];
        return Color.red;
    }
    private Dictionary<int, float[]> SemiSerializeColorDict(Dictionary<int, Color> dict)
    {
        Dictionary<int, float[]> ser = new();
        foreach (KeyValuePair<int, Color> pair in dict)
        {
            ser.Add(pair.Key, new float[] { pair.Value.r, pair.Value.g, pair.Value.b });
        }
        return ser;
    }
    private Dictionary<int, Color> SemiDeserializeColorDict(Dictionary<int, float[]> dict)
    {
        Dictionary<int, Color> des = new();
        foreach (KeyValuePair<int, float[]> pair in dict)
        {
            des.Add(pair.Key, new Color(pair.Value[0], pair.Value[1], pair.Value[2]));
        }
        return des;
    }
}
