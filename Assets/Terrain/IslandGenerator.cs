using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector3[] verticies;
    int[] triangles;
    Color[] vertColors;

    int xSize = 100;
    int zSize = 100;
    private float WorldXWidth;
    private float WorldZWidth;
    static int vertsPerTenWidth = 4;
    [SerializeField] float lacunarity = 2;
    [SerializeField] float persistance = 0.5f;
    [SerializeField] int octaves = 6;
    [SerializeField] int xWidth = 400;
    [SerializeField] int zWidth = 400;
    void Start()
    {
        WorldXWidth = xWidth;
        WorldZWidth = zWidth;
        xSize = Mathf.RoundToInt((WorldXWidth / 10.0f) * vertsPerTenWidth);
        zSize = Mathf.RoundToInt((WorldZWidth / 10.0f) * vertsPerTenWidth);
        Generate();
    }
    private void Generate()
    {
        if(mesh != null) mesh.Clear();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }
    void CreateShape()
    {
        verticies = new Vector3[(xSize + 1) * (zSize + 1)];
        vertColors = new Color[(xSize + 1) * (zSize + 1)];
        int i = 0;
        for(int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float fX = (((float)x / (float)xSize) * WorldXWidth) - (WorldXWidth / 2);
                float fZ = (((float)z / (float)zSize) * WorldZWidth) - (WorldZWidth / 2);
                //float y = Noise.GroundHeight(fX, fZ, lacunarity, persistance, octaves);
                float y = Noise.ConformToIslandShape(Noise.IslandEdgeCircleFilter(Noise.GroundHeight(fX + transform.position.x, fZ + transform.position.z, lacunarity, persistance, octaves), x, z, xSize, zSize));
                y = SetTerrainShader.SetIslandHeightBelowStarts(y);
                verticies[i] = new Vector3(fX, y, fZ);
                vertColors[i] = Noise.GetIslandColorForHeight(y, y, y);
                i++;
            }
        }
        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;
        for(int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }
    void CreateHardEdgeShape()
    {
        float fXDistForOne = ((1.0f / (float)xSize) * WorldXWidth);
        float fZDistForOne = ((1.0f / (float)zSize) * WorldZWidth);
        float xOffset = -(WorldXWidth / 2);
        float zOffset = -(WorldZWidth / 2);
        List<Vector3> vertList = new();
        List<int> trisList = new();
        List<Color> ColorsList = new();
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                float currX = (x * fXDistForOne) + xOffset;                          //bottom left
                float currZ = (z * fZDistForOne) + zOffset;
                float currXPlus = currX + fXDistForOne;
                float currZPlus = currZ + fZDistForOne;
                float tPosX = transform.position.x;
                float tPosZ = transform.position.z;
                float heightBL = Noise.ConformToIslandShape(Noise.IslandEdgeFilter(Noise.GroundHeight(currX + tPosX, currZ + tPosZ, lacunarity, persistance, octaves), x, z, xSize, zSize));
                float heightTL = Noise.ConformToIslandShape(Noise.IslandEdgeFilter(Noise.GroundHeight(currX + tPosX, currZPlus + tPosZ, lacunarity, persistance, octaves), x, z + 1, xSize, zSize));
                float heightBR = Noise.ConformToIslandShape(Noise.IslandEdgeFilter(Noise.GroundHeight(currXPlus + tPosX, currZ + tPosZ, lacunarity, persistance, octaves), x + 1, z, xSize, zSize));
                float heightTR = Noise.ConformToIslandShape(Noise.IslandEdgeFilter(Noise.GroundHeight(currXPlus + tPosX, currZPlus + tPosZ, lacunarity, persistance, octaves), x + 1, z + 1, xSize, zSize));

                vertList.Add(new Vector3(currX, heightBL, currZ));      //bottom left       first triangle
                vertList.Add(new Vector3(currX, heightTL, currZPlus));  //top left
                vertList.Add(new Vector3(currXPlus, heightTR, currZPlus));  //top right
                trisList.Add(vertList.Count - 3);
                trisList.Add(vertList.Count - 2);
                trisList.Add(vertList.Count - 1);
                float centerHeight = (heightBL + heightTL + heightTR) / 3;
                Color centerColor = Noise.GetIslandColorForHeight(heightBL, heightTL, heightTR);
                ColorsList.Add(centerColor);
                ColorsList.Add(centerColor);
                ColorsList.Add(centerColor);
                vertList.Add(new Vector3(currX, heightBL, currZ));      //bottom left       second triangle
                vertList.Add(new Vector3(currXPlus, heightBR, currZ));  //bottom right
                vertList.Add(new Vector3(currXPlus, heightTR, currZPlus));  //top right
                trisList.Add(vertList.Count - 1);
                trisList.Add(vertList.Count - 2);
                trisList.Add(vertList.Count - 3);
                //centerHeight = (heightBL + heightBR + heightTR) / 3;
                centerColor = Noise.GetIslandColorForHeight(heightBL, heightBR, heightTR);
                ColorsList.Add(centerColor);
                ColorsList.Add(centerColor);
                ColorsList.Add(centerColor);
            }
        }
        vertColors = ColorsList.ToArray();
        verticies = vertList.ToArray();
        triangles = trisList.ToArray();
    }
    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = verticies;
        mesh.triangles = triangles;
        mesh.colors = vertColors;

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
