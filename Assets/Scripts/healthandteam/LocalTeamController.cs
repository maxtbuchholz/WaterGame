using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocalTeamController : MonoBehaviour
{
    [SerializeField] int teamId = -1;
    [SerializeField] List<GameObject> coloredObjects;
    private Color teamColor;
    // Start is called before the first frame update
    void Start()
    {
        teamId = Random.Range(1, 7);
        teamColor = TeamsController.Instance.GetTeamColor(teamId);
        SetGameObjectColors();
    }
    void SetGameObjectColors()
    {
        foreach(GameObject ob in coloredObjects)
        {
            List<Material> obMat = ob.GetComponent<Renderer>().materials.ToList();
            foreach(Material mat in obMat)
            {
                if(mat.name == "fortbrick (Instance)")
                {
                    mat.SetColor("_BaseColor", teamColor);
                }
                else if (mat.name == "TeamColor (Instance)")
                {
                    mat.SetColor("_BaseColor", teamColor);
                }
            }
        }
    }
    public void AddObjectToColored(GameObject go)
    {
        coloredObjects.Add(go);
        SetGameObjectColors();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
