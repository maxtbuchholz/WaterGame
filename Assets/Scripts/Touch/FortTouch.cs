using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class FortTouch : MonoBehaviour
{
    [SerializeField] LocalTeamController locatTeamController;
    [SerializeField] List<Transform> addMaterialTo;
    [SerializeField] Material topGlowMaterial;
    void Start()
    {
        ButtonCollisionTracker.Instance.AddWorldButton(GetComponent<Collider>(), 2);
        SetupColors();
    }
    public void SetupColors()
    {
        ButtonCollisionTracker.Instance.AddWorldButton(GetComponent<Collider>(), 2);
        for (int i = 0; i < addMaterialTo.Count; i++)
        {
            originalBaseColor.Add(new Dictionary<Material, Color>());
            List<Material> obMat = addMaterialTo[i].GetComponent<Renderer>().materials.ToList();
            foreach (Material mat in obMat)
            {
                if (!originalBaseColor[i].ContainsKey(mat))
                    originalBaseColor[i].Add(mat, mat.GetColor("_BaseColor"));
                else
                    originalBaseColor[i][mat] = mat.GetColor("_BaseColor");
            }
        }
    }
    List<Dictionary<Material, Color>> originalBaseColor = new();
    bool isTouching = false;
    public bool TouchStart()
    {
        if (!PointToPlayer.Instance.GetMainCanvas().GetCanvasEnabled()) return false;
        if (locatTeamController.GetTeam() != 0) return false;
        if (Vector3.Distance(transform.position, PointToPlayer.Instance.GetFocalPoint().transform.position) > 20) return false;
        //if (isTouching) return false;
        //isTouching = true;
        for (int i = 0; i < addMaterialTo.Count; i++)
        {
            List<Material> obMat = addMaterialTo[i].GetComponent<Renderer>().materials.ToList();
            foreach (Material mat in obMat)
            {
                Color col = mat.GetColor("_BaseColor");
                if(col != Color.white) originalBaseColor[i][mat] = col;
                mat.SetColor("_BaseColor", Color.white);
            }
        }
        PlayerPrefsController.Instance.SetTapFortHelp(true);
        return true;
    }
    public void TouchEnd()
    {
        for (int i = 0; i < addMaterialTo.Count; i++)
        {
            List<Material> obMat = addMaterialTo[i].GetComponent<Renderer>().materials.ToList();
            foreach (Material mat in obMat)
            {
                Color col = originalBaseColor[i][mat];
                mat.SetColor("_BaseColor", col);
            }
        }
        string key = GetComponent<FortLevel>().GetKey();
        Vector2 fortScreenPos = PointToPlayer.Instance.GetCamera().WorldToScreenPoint(transform.position);
        PopupManager.Instance.SummonAskShipFortUpgrade(fortScreenPos, key);
        PointToPlayer.Instance.ResetTouch();
    }
}