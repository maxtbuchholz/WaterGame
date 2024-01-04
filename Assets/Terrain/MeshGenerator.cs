using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector3[] verticies;
    int[] triangles;

    int xSize = 50;
    int zSize = 50;
    public float WorldWidth;
    void Start()
    {
        WorldWidth = 1000;
        Generate();
    }
    private void Generate()
    {
        if(mesh != null) mesh.Clear();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateHardEdgeShape();
        UpdateMesh();
    }
    public void UpdateMesh(int x, int z)
    {
        startX = x;
        startZ = z;
        Generate();
    }
    int startX = 0;
    int startZ = 0;
    void CreateHardEdgeShape()
    {
        float fXDistForOne = ((1.0f / (float)xSize) * WorldWidth);
        float fZDistForOne = ((1.0f / (float)zSize) * WorldWidth);
        float xOffset = startX - (WorldWidth / 2);
        float zOffset = startZ - (WorldWidth / 2);
        List<Vector3> vertList = new();
        List<int> trisList = new();
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                float currX = (x * fXDistForOne) + xOffset;                          //bottom left
                float currZ = (z * fZDistForOne) + zOffset;
                float currXPlus = currX + fXDistForOne;
                float currZPlus = currZ + fZDistForOne;
                float heightBL = Noise.FloorHeight(currX, currZ);
                float heightTL = Noise.FloorHeight(currX, currZPlus);
                float heightBR = Noise.FloorHeight(currXPlus, currZ);
                float heightTR = Noise.FloorHeight(currXPlus, currZPlus);

                vertList.Add(new Vector3(currX, heightBL, currZ));      //bottom left       first triangle
                vertList.Add(new Vector3(currX, heightTL, currZPlus));  //top left
                vertList.Add(new Vector3(currXPlus, heightTR, currZPlus));  //top right
                trisList.Add(vertList.Count - 3);
                trisList.Add(vertList.Count - 2);
                trisList.Add(vertList.Count - 1);
                vertList.Add(new Vector3(currX, heightBL, currZ));      //bottom left       second triangle
                vertList.Add(new Vector3(currXPlus, heightBR, currZ));  //bottom right
                vertList.Add(new Vector3(currXPlus, heightTR, currZPlus));  //top right
                trisList.Add(vertList.Count - 1);
                trisList.Add(vertList.Count - 2);
                trisList.Add(vertList.Count - 3);
            }
        }
        verticies = vertList.ToArray();
        triangles = trisList.ToArray();
    }
    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = verticies;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
    //private void OnDrawGizmos()
    //{
    //    if (verticies == null)
    //        return;
    //    for (int i = 0; i < verticies.Length; i++)
    //    {
    //        Gizmos.DrawSphere(verticies[i], 0.1f);
    //    }
    //}
}
