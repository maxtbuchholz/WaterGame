using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTerrainShader : MonoBehaviour
{
    static float[] Starts = { 0.71f, 3.64f, 5.19f, 11.9f };  //grass lightGrass mountain snow 
    // Start is called before the first frame update
    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        rend.material.SetFloat("_GrassStart", Starts[0]);
        rend.material.SetFloat("_LightGrassStart", Starts[1]);
        rend.material.SetFloat("_MountainStart", Starts[2]);
        rend.material.SetFloat("_SnowStart", Starts[3]);
    }
    static public float SetIslandHeightBelowStarts(float height)
    {
        foreach(float s in Starts)
        {
            if((height >= s) && (height < s + 0.1f))
            {
                height = s - 0.1f;
            }
        }
        return height;
    }
}
