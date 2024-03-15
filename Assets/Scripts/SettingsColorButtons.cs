using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsColorButtons : MonoBehaviour
{
    [SerializeField] List<Image> colorImages;       //number of teams
    [SerializeField] List<Button> colorButtons;     //number of teams -1, where -1 is index 0
    private Dictionary<int, Color> teamColors;
    public void InitColors()
    {
        teamColors = TeamsController.Instance.GetAllColors();
        for(int i = 0; i < colorImages.Count; i++)
        {
            colorImages[i].color = teamColors[i];
        }
    }
    //void Awake()
    //{
    //    for(int i = 0; i < colorButtons.Count; i++)
    //    {
    //        colorButtons[i].onClick.AddListener(() => ColorButtonClicked(i + 1));
    //    }
    //}
    //private void ColorButtonClicked(int pressedColorID)
    //{
    //    Color temp = teamColors[pressedColorID];
    //    teamColors[pressedColorID] = teamColors[0];
    //    teamColors[0] = temp;
    //    TeamsController.Instance.SetAllColors(teamColors);
    //    InitColors();
    //}
    public void ColorButtonClicked(GameObject button)
    {
        int pressedColorID = int.Parse(button.name);
        Color temp = teamColors[pressedColorID];
        teamColors[pressedColorID] = teamColors[0];
        teamColors[0] = temp;
        TeamsController.Instance.SetAllColors(teamColors);
        InitColors();
    }

}
