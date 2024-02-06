using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class IslandGenerator : MonoBehaviour
{
    Mesh meshIsland;
    Vector3[] verticies;
    int[] triangles;
    //Color[] vertColors;

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
    [SerializeField] Material clearIslandOuterColliderMaterial;
    async void Start()
    {
        WorldXWidth = xWidth;
        WorldZWidth = zWidth;
        xSize = Mathf.RoundToInt((WorldXWidth / 10.0f) * vertsPerTenWidth);
        zSize = Mathf.RoundToInt((WorldZWidth / 10.0f) * vertsPerTenWidth);
        await Generate(transform.position);
        //await Task.Run(() =>
        //{
        //    Generate();
        //});
    }
    private async Task Generate(Vector3 currPos)
    {
        if (meshIsland != null) meshIsland.Clear();
        meshIsland = new Mesh();
        GetComponent<MeshFilter>().mesh = meshIsland;

        VertAndTri vertAndTri = await Task.Run(() => CreateShape(currPos));
        UpdateMesh(vertAndTri);
        GetComponent<MeshCollider>().sharedMesh = meshIsland;
        GetComponent<MeshCollider>().sharedMesh.RecalculateNormals();

        Vector3 botLeft = new Vector3(currPos.x - (WorldXWidth / 2), 100, currPos.z - (WorldZWidth / 2));
        Vector3 topRight = new Vector3(currPos.x + (WorldXWidth / 2), 100, currPos.z + (WorldZWidth / 2)); //new Vector3((((float)xWidth / (float)xSize) * WorldXWidth) - (WorldXWidth / 2) + transform.position.x, 100, (((float)zWidth / (float)zSize) * WorldZWidth) - (WorldZWidth / 2) + transform.position.z);
        //SetUpIslandColliders(await Task.Run(() => CreateIslandCollider(botLeft, topRight)));
        SetUpIslandColliders(CreateIslandCollider(botLeft, topRight, vertAndTri.foundColliderEdgePlaces));
        //foreach(Vector2 v2 in vertAndTri.foundColliderEdgePlaces)
        //{
        //    Debug.DrawLine(new Vector3(v2.x, -10, v2.y), new Vector3(v2.x, 10, v2.y), Color.red);
        //}
        //Debug.Break();
        return;
    }
    private void SetUpIslandColliders(List<Mesh> meshes)
    {
        foreach (Mesh mesh in meshes)
        {
            GameObject obMesh = new GameObject();
            obMesh.transform.parent = transform;
            MeshFilter mFil = obMesh.AddComponent<MeshFilter>();
            MeshRenderer meRen = obMesh.AddComponent<MeshRenderer>();
            meRen.material = clearIslandOuterColliderMaterial;
            MeshCollider mCol = obMesh.AddComponent<MeshCollider>();
            mCol.sharedMesh = mesh;
            mFil.mesh = mesh;
            obMesh.tag = "IslandOuterCollider";
            obMesh.layer = 9;
        }
    }
    private class VertAndTri
    {
        public Vector3[] verticies;
        public int[] triangles;
        public List<Vector2> foundColliderEdgePlaces;
        public VertAndTri(Vector3[] verticies, int[] triangles, List<Vector2> foundColliderEdgePlaces)
        {
            this.verticies = verticies;
            this.triangles = triangles;
            this.foundColliderEdgePlaces = foundColliderEdgePlaces;
        }
    }
    private float islandYCollider = 0;
    VertAndTri CreateShape(Vector3 currTransform)
    {
        Vector3[] verticies = new Vector3[(xSize + 1) * (zSize + 1)];
        //vertColors = new Color[(xSize + 1) * (zSize + 1)];
        int i = 0;
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float fX = (((float)x / (float)xSize) * WorldXWidth) - (WorldXWidth / 2);
                float fZ = (((float)z / (float)zSize) * WorldZWidth) - (WorldZWidth / 2);
                //float y = Noise.GroundHeight(fX, fZ, lacunarity, persistance, octaves);
                float y = Noise.ConformToIslandShape(Noise.IslandEdgeCircleFilter(Noise.GroundHeight(fX + currTransform.x, fZ + currTransform.z, lacunarity, persistance, octaves), x, z, xSize, zSize));
                y = SetTerrainShader.SetIslandHeightBelowStarts(y);
                verticies[i] = new Vector3(fX, y, fZ);
                //vertColors[i] = Noise.GetIslandColorForHeight(y, y, y);
                i++;
            }
        }
        int[] triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;
        List<Vector2> foundColliderSport = new();
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                FindEgdes(vert + 0, vert + xSize + 1, vert + 1);
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;
                FindEgdes(vert + 1, vert + xSize + 1, vert + xSize + 2);

                vert++;
                tris += 6;
            }
            vert++;
        }
        void FindEgdes(int a, int b, int c)
        {
            Vector2 tempV2;
            if (verticies[a].y == islandYCollider)                                      //check for verticies equaling the target, probably not needed
            {
                tempV2 = new Vector2(verticies[a].x, verticies[a].z);
                if (!foundColliderSport.Contains(tempV2)) foundColliderSport.Add(tempV2);
            }
            if (verticies[b].y == islandYCollider)
            {
                tempV2 = new Vector2(verticies[b].x, verticies[b].z);
                if (!foundColliderSport.Contains(tempV2)) foundColliderSport.Add(tempV2);
            }
            if (verticies[c].y == islandYCollider)
            {
                tempV2 = new Vector2(verticies[c].x, verticies[c].z);
                if (!foundColliderSport.Contains(tempV2)) foundColliderSport.Add(tempV2);
            }
            if(verticies[a].y > islandYCollider)
            {
                if(verticies[b].y < islandYCollider)
                {
                    float top = verticies[a].y - islandYCollider;
                    float bottom = islandYCollider - verticies[b].y;
                    Vector3 tempV3 = Vector3.Lerp(verticies[b], verticies[a], bottom / (top + bottom));
                    tempV2 = new Vector2(tempV3.x, tempV3.z);
                    if (!foundColliderSport.Contains(tempV2)) foundColliderSport.Add(tempV2);
                }
                if (verticies[c].y < islandYCollider)
                {
                    float top = verticies[a].y - islandYCollider;
                    float bottom = islandYCollider - verticies[c].y;
                    Vector3 tempV3 = Vector3.Lerp(verticies[c], verticies[a], bottom / (top + bottom));
                    tempV2 = new Vector2(tempV3.x, tempV3.z);
                    if (!foundColliderSport.Contains(tempV2)) foundColliderSport.Add(tempV2);
                }
            }
            if (verticies[b].y > islandYCollider)
            {
                if (verticies[c].y < islandYCollider)
                {
                    float top = verticies[b].y - islandYCollider;
                    float bottom = islandYCollider - verticies[c].y;
                    Vector3 tempV3 = Vector3.Lerp(verticies[c], verticies[b], bottom / (top + bottom));
                    tempV2 = new Vector2(tempV3.x, tempV3.z);
                    if (!foundColliderSport.Contains(tempV2)) foundColliderSport.Add(tempV2);
                }
                if (verticies[a].y < islandYCollider)
                {
                    float top = verticies[b].y - islandYCollider;
                    float bottom = islandYCollider - verticies[a].y;
                    Vector3 tempV3 = Vector3.Lerp(verticies[a], verticies[b], bottom / (top + bottom));
                    tempV2 = new Vector2(tempV3.x, tempV3.z);
                    if (!foundColliderSport.Contains(tempV2)) foundColliderSport.Add(tempV2);
                }
            }
            if (verticies[c].y > islandYCollider)
            {
                if (verticies[a].y < islandYCollider)
                {
                    float top = verticies[c].y - islandYCollider;
                    float bottom = islandYCollider - verticies[a].y;
                    Vector3 tempV3 = Vector3.Lerp(verticies[a], verticies[c], bottom / (top + bottom));
                    tempV2 = new Vector2(tempV3.x, tempV3.z);
                    if (!foundColliderSport.Contains(tempV2)) foundColliderSport.Add(tempV2);
                }
                if (verticies[b].y < islandYCollider)
                {
                    float top = verticies[c].y - islandYCollider;
                    float bottom = islandYCollider - verticies[b].y;
                    Vector3 tempV3 = Vector3.Lerp(verticies[b], verticies[c], bottom / (top + bottom));
                    tempV2 = new Vector2(tempV3.x, tempV3.z);
                    if (!foundColliderSport.Contains(tempV2)) foundColliderSport.Add(tempV2);
                }
            }
        }
        return new VertAndTri(verticies, triangles, foundColliderSport);
    }
    //void CreateHardEdgeShape()
    //{
    //    float fXDistForOne = ((1.0f / (float)xSize) * WorldXWidth);
    //    float fZDistForOne = ((1.0f / (float)zSize) * WorldZWidth);
    //    float xOffset = -(WorldXWidth / 2);
    //    float zOffset = -(WorldZWidth / 2);
    //    List<Vector3> vertList = new();
    //    List<int> trisList = new();
    //    List<Color> ColorsList = new();
    //    for (int z = 0; z < zSize; z++)
    //    {
    //        for (int x = 0; x < xSize; x++)
    //        {
    //            float currX = (x * fXDistForOne) + xOffset;                          //bottom left
    //            float currZ = (z * fZDistForOne) + zOffset;
    //            float currXPlus = currX + fXDistForOne;
    //            float currZPlus = currZ + fZDistForOne;
    //            float tPosX = transform.position.x;
    //            float tPosZ = transform.position.z;
    //            float heightBL = Noise.ConformToIslandShape(Noise.IslandEdgeFilter(Noise.GroundHeight(currX + tPosX, currZ + tPosZ, lacunarity, persistance, octaves), x, z, xSize, zSize));
    //            float heightTL = Noise.ConformToIslandShape(Noise.IslandEdgeFilter(Noise.GroundHeight(currX + tPosX, currZPlus + tPosZ, lacunarity, persistance, octaves), x, z + 1, xSize, zSize));
    //            float heightBR = Noise.ConformToIslandShape(Noise.IslandEdgeFilter(Noise.GroundHeight(currXPlus + tPosX, currZ + tPosZ, lacunarity, persistance, octaves), x + 1, z, xSize, zSize));
    //            float heightTR = Noise.ConformToIslandShape(Noise.IslandEdgeFilter(Noise.GroundHeight(currXPlus + tPosX, currZPlus + tPosZ, lacunarity, persistance, octaves), x + 1, z + 1, xSize, zSize));

    //            vertList.Add(new Vector3(currX, heightBL, currZ));      //bottom left       first triangle
    //            vertList.Add(new Vector3(currX, heightTL, currZPlus));  //top left
    //            vertList.Add(new Vector3(currXPlus, heightTR, currZPlus));  //top right
    //            trisList.Add(vertList.Count - 3);
    //            trisList.Add(vertList.Count - 2);
    //            trisList.Add(vertList.Count - 1);
    //            float centerHeight = (heightBL + heightTL + heightTR) / 3;
    //            Color centerColor = Noise.GetIslandColorForHeight(heightBL, heightTL, heightTR);
    //            ColorsList.Add(centerColor);
    //            ColorsList.Add(centerColor);
    //            ColorsList.Add(centerColor);
    //            vertList.Add(new Vector3(currX, heightBL, currZ));      //bottom left       second triangle
    //            vertList.Add(new Vector3(currXPlus, heightBR, currZ));  //bottom right
    //            vertList.Add(new Vector3(currXPlus, heightTR, currZPlus));  //top right
    //            trisList.Add(vertList.Count - 1);
    //            trisList.Add(vertList.Count - 2);
    //            trisList.Add(vertList.Count - 3);
    //            //centerHeight = (heightBL + heightBR + heightTR) / 3;
    //            centerColor = Noise.GetIslandColorForHeight(heightBL, heightBR, heightTR);
    //            ColorsList.Add(centerColor);
    //            ColorsList.Add(centerColor);
    //            ColorsList.Add(centerColor);
    //        }
    //    }
    //    vertColors = ColorsList.ToArray();
    //    verticies = vertList.ToArray();
    //    triangles = trisList.ToArray();
    //}
    void UpdateMesh(VertAndTri vT)
    {
        meshIsland.Clear();

        meshIsland.vertices = vT.verticies;
        meshIsland.triangles = vT.triangles;
        //mesh.colors = vertColors;

        meshIsland.RecalculateNormals();
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
    float colliderGridCheckOffset = 2.0f;
    float groundCheckHeight = 0.5f;
    float areaCheckOffset = 30;
    Vector3 halfOuterBox;
    List<Mesh> CreateIslandCollider(Vector3 botLeft, Vector3 topRight, List<Vector2> foundEdges)
    {
        List<Mesh> returnMeshes = new();
        halfOuterBox = new Vector3(areaCheckOffset / 2, areaCheckOffset / 2, areaCheckOffset / 2);
        //float xDst = topRight.x - botLeft.x;
        //float zDst = topRight.z - botLeft.z;
        //botLeft.x += (xDst / 1);
        //botLeft.z += (zDst / 1);
        //topRight.x -= (xDst / 1);
        //topRight.z -= (zDst / 1);
        bool zFound = false;
        groundCheckHeight = botLeft.y - groundCheckHeight;
        Debug.DrawLine(botLeft, topRight);
        Vector3 currentCheck = botLeft;
        HashSet<Vector2> FoundIslandPositions = new();
        float halfBoxCheck = colliderGridCheckOffset / 2;
        while (!zFound)
        {
            if (currentCheck.z < topRight.z)
            {
                bool xFound = false;
                while (!xFound)
                {
                    if (currentCheck.x < topRight.x)
                    {
                        bool outerHitGround = false;
                        //Debug.DrawLine(Vector3.zero, currentCheck, Color.blue);
                        RaycastHit[] outerRayHits = Physics.BoxCastAll(currentCheck, halfOuterBox, Vector3.down, Quaternion.identity, 100 - (areaCheckOffset / 2));
                        foreach (RaycastHit rayHit in outerRayHits)
                        {
                            outerHitGround |= (rayHit.transform.gameObject.CompareTag("Land"));
                            //Debug.DrawLine(rayHit.point, currentCheck, Color.red);
                        }
                        if (outerHitGround)
                        {
                            bool innerZFound = false;
                            Vector3 innerBotLeft = currentCheck - new Vector3(areaCheckOffset / 2, 0, areaCheckOffset / 2);
                            Vector3 innerTopRight = currentCheck + new Vector3(areaCheckOffset / 2, 0, areaCheckOffset / 2);
                            Vector3 currentInnerCheck = innerBotLeft;
                            currentInnerCheck.y = halfBoxCheck;
                            while (!innerZFound)
                            {
                                if (currentInnerCheck.z < innerTopRight.z)
                                {
                                    bool innerXFound = false;

                                    while (!innerXFound)
                                    {
                                        if (currentInnerCheck.x < innerTopRight.x)
                                        {
                                            bool hitGound = false;
                                            currentInnerCheck.y = 100;
                                            RaycastHit[] rayHits = Physics.SphereCastAll(currentInnerCheck, halfBoxCheck, Vector3.down, groundCheckHeight);
                                            foreach (RaycastHit rayHit in rayHits)
                                            {
                                                hitGound |= (rayHit.transform.gameObject.CompareTag("Land"));
                                                Debug.DrawLine(rayHit.point, Vector3.zero, Color.magenta);
                                            }
                                            if (hitGound)
                                            {
                                                FoundIslandPositions.Add(new Vector2(currentInnerCheck.x, currentInnerCheck.z));
                                            }
                                            currentInnerCheck.x += colliderGridCheckOffset;
                                        }
                                        else
                                        {
                                            innerXFound = true;
                                        }
                                    }
                                    currentInnerCheck.x = innerBotLeft.x;
                                    currentInnerCheck.z += colliderGridCheckOffset;
                                }
                                else
                                {
                                    innerZFound = true;
                                }
                            }
                        }
                        currentCheck.x += areaCheckOffset;
                    }
                    else
                    {
                        xFound = true;
                    }
                }
                currentCheck.x = botLeft.x;
                currentCheck.z += areaCheckOffset;
            }
            else
            {
                zFound = true;
            }
        }
        //Debug.Log(FoundIslandPositions.Count);
        HashSet<Vector2> edgeCoords = new();
        Vector2[] cross_neighbors = new Vector2[] { new Vector2(colliderGridCheckOffset, 0), new Vector2(-colliderGridCheckOffset, 0), new Vector2(0, colliderGridCheckOffset), new Vector2(0, -colliderGridCheckOffset) };
        Vector2[] square_neighbors = new Vector2[] { new Vector2(-colliderGridCheckOffset, 0), new Vector2(-colliderGridCheckOffset, colliderGridCheckOffset), new Vector2(0, colliderGridCheckOffset), new Vector2(colliderGridCheckOffset, colliderGridCheckOffset), new Vector2(colliderGridCheckOffset, 0), new Vector2(colliderGridCheckOffset, -colliderGridCheckOffset), new Vector2(0, -colliderGridCheckOffset), new Vector2(-colliderGridCheckOffset, -colliderGridCheckOffset) };
        //foreach (Vector2 coord in FoundIslandPositions)             //get only outside coords
        //{
        //    int neighborCount = 0;
        //    foreach(Vector2 neighbor in cross_neighbors)
        //    {
        //        if (FoundIslandPositions.Contains(coord + neighbor))
        //            neighborCount++;
        //    }
        //    if (neighborCount < 4)
        //        edgeCoords.Add(coord);
        //}
        //Debug.Log(edgeCoords.Count);
        List<List<Vector2>> islandVectorSets = new();
        foreach (Vector2 coord in FoundIslandPositions)                   //seperate islands into seperate hash tables
        {
            List<int> islandSetsNeighboringCoordIndeds = new(); //temp keeps track of which sets are neighboring a given coord
            for(int i = 0; i < islandVectorSets.Count; i++)
            {
                for(int j = 0; j < square_neighbors.Length; j++)
                {
                    if (islandVectorSets[i].Contains(coord + square_neighbors[j]))
                    {
                        islandSetsNeighboringCoordIndeds.Add(i);
                        j = square_neighbors.Length;
                    }
                }
            }
            if(islandSetsNeighboringCoordIndeds.Count == 0) //create new hashset with this coord
            {
                islandVectorSets.Add(new List<Vector2>() { coord });
            }
            else if(islandSetsNeighboringCoordIndeds.Count == 1)        //one set neighbors coord
            {
                islandVectorSets[islandSetsNeighboringCoordIndeds[0]].Add(coord);
            }
            else
            {                                               //multiple sets neighbor coord, have to add sets together
                for(int l = islandSetsNeighboringCoordIndeds.Count - 1; l > 0; l--)
                {
                    islandVectorSets[islandSetsNeighboringCoordIndeds[0]].AddRange(islandVectorSets[islandSetsNeighboringCoordIndeds[l]]);
                    islandVectorSets.RemoveAt(islandSetsNeighboringCoordIndeds[l]);
                }            
            }
        }
        int saveForDebugColliderNumber = 0;
        foreach(List<Vector2> innerSet in islandVectorSets)
        {
            List<Vector2> outerSet = new();
            foreach(Vector2 point in innerSet)
            {
                foreach(Vector2 neighbor in square_neighbors)
                {
                    if (!outerSet.Contains(point + neighbor) && !innerSet.Contains(point + neighbor))
                        outerSet.Add(point + neighbor);
                }
            }
            Vector2 centerDebug = GetCenterCoord(outerSet);

            ////////now we have our outer points, lets make the specal neighbor objects so we can figure out the edge
            List<EdgeCoordNeighboors> edge = new();
            for (int i = 0; i < outerSet.Count; i++)
            {
                edge.Add(new EdgeCoordNeighboors(i));
                foreach(Vector2 neighbor in cross_neighbors)
                {
                    int tryIdx = outerSet.IndexOf(outerSet[i] + neighbor);
                    if (tryIdx != -1)
                        edge[^1].neighboors.Add(tryIdx);
                }
            }
            //////////////////now we have the objects with edge coords and neighboors of those coords
            List<int> edgeOrder = new();
            for(int i = 0; i < edge.Count; i++)                                                         //start a edge test at every coord just to be sure, we will only use the first that works
            {
                List<int> li = new() { edge[i].index };
                List<List<int>> testsReturn = RecTestEdgeDir(li.ToList<int>());
                int maxCount = 0;
                int maxIndex = 0;
                for(int j = 0; j < testsReturn.Count; j++)
                {
                    if (testsReturn[j].Count > maxCount)
                    {
                        maxCount = testsReturn[j].Count;
                        maxIndex = j;
                    }
                }
                if(maxCount > 0.8f * edge.Count)        //we found a suitable edge
                {
                    edgeOrder = testsReturn[maxIndex];
                    i = edge.Count;
                }
            }
            List<List<int>> RecTestEdgeDir(List<int> li)
            {
                List<List<int>> retLis = new();
                bool head = true;
                foreach (int nei in edge[li[^1]].neighboors)
                {
                    if (!li.Contains(nei))
                    {
                        head = false;
                        retLis.Add(li.ToList());
                        retLis[^1].Add(nei);
                        retLis.AddRange(RecTestEdgeDir(retLis[^1]));
                    }
                }
                if (head)
                    retLis.Add(li.ToList());
                return retLis;
            }
            ////////////////////////////////////next we remove all points that are in the middle of a straight line or inside a 90 degree angle
            List<Vector2> sortedEdgeOrder = new();
            foreach (int index in edgeOrder)
                sortedEdgeOrder.Add(outerSet[index]);
            List<int> indexesToRemove = new();
            for (int i = sortedEdgeOrder.Count - 1; i > 1; i--)
            {
                if (MiddleOfCorner(i - 2, i - 1, i))
                {
                    indexesToRemove.Add(i - 1);
                }
            }
            if (sortedEdgeOrder.Count > 2)
            {
                if (MiddleOfCorner(sortedEdgeOrder.Count - 1, 0, 1))
                {
                    indexesToRemove.Add(0);
                }
            }
            if (edgeOrder.Count > 2)
            {
                if (MiddleOfCorner(sortedEdgeOrder.Count - 2, sortedEdgeOrder.Count - 1, 0))
                {
                    indexesToRemove.Add(sortedEdgeOrder.Count - 1);
                }
            }

            indexesToRemove.Sort();
            for (int i = indexesToRemove.Count - 1; i >= 0; i--)
            {
                try
                {
                    //int index = edgeOrder.IndexOf(indexesToRemove[i]);
                    //edgeOrder[index] = -1;
                    sortedEdgeOrder.RemoveAt(indexesToRemove[i]);
                    //outerSet.RemoveAt(index);
                }
                catch(System.Exception e)
                {
                    Debug.Log(e.Message);
                }
            }
            indexesToRemove.Clear();
            for (int i = sortedEdgeOrder.Count - 1; i > 1; i--)
            {
                if (MiddleOfStraightLine(i - 2, i - 1, i))
                {
                    indexesToRemove.Add(i - 1);
                }
            }
            if (sortedEdgeOrder.Count > 2)
            {
                if (MiddleOfStraightLine(sortedEdgeOrder.Count - 1, 0, 1))
                {
                    indexesToRemove.Add(0);
                }
            }
            if (edgeOrder.Count > 2)
            {
                if (MiddleOfStraightLine(sortedEdgeOrder.Count - 2, sortedEdgeOrder.Count - 1, 0))
                {
                    indexesToRemove.Add(sortedEdgeOrder.Count - 1);
                }
            }
            indexesToRemove.Sort();
            for (int i = indexesToRemove.Count - 1; i >= 0; i--)
            {
                try
                {
                    //int index = edgeOrder.IndexOf(indexesToRemove[i]);
                    //edgeOrder[index] = -1;
                    sortedEdgeOrder.RemoveAt(indexesToRemove[i]);
                    //outerSet.RemoveAt(index);
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                }
            }

            bool MiddleOfStraightLine(int first, int middle, int end)
            {
                if ((sortedEdgeOrder[first].x == sortedEdgeOrder[middle].x) && (sortedEdgeOrder[middle].x == sortedEdgeOrder[end].x))
                    return true;
                if ((sortedEdgeOrder[first].y == sortedEdgeOrder[middle].y) && (sortedEdgeOrder[middle].y == sortedEdgeOrder[end].y))
                    return true;
                return false;
            }
            bool MiddleOfCorner(int first, int middle, int end)
            {
                if ((sortedEdgeOrder[first].x == sortedEdgeOrder[middle].x) && (sortedEdgeOrder[middle].y == sortedEdgeOrder[end].y))
                    return true;
                if ((sortedEdgeOrder[first].y == sortedEdgeOrder[middle].y) && (sortedEdgeOrder[middle].x == sortedEdgeOrder[end].x))
                    return true;
                return false;
            }




            for (int i = 1; i < sortedEdgeOrder.Count; i++)
            {
                Debug.DrawLine(new Vector3(sortedEdgeOrder[i].x, 10, sortedEdgeOrder[i].y), new Vector3(sortedEdgeOrder[i - 1].x, 10, sortedEdgeOrder[i - 1].y), Color.red);
            }
            for(int i = 0; i < sortedEdgeOrder.Count; i++)
                Debug.DrawLine(new Vector3(centerDebug.x, 10, centerDebug.y), new Vector3(sortedEdgeOrder[i].x, 10, sortedEdgeOrder[i].y), Color.blue);

            ///////////////////////////////////////////////////////////generateMesh from points
            Mesh mesh = new();
            List<Vector3> verticies = new();
            List<int> triangles = new();
            for(int i = 0; i < sortedEdgeOrder.Count; i++)
            {
                verticies.Add(new Vector3(sortedEdgeOrder[i].x, -10, sortedEdgeOrder[i].y));
                verticies.Add(new Vector3(sortedEdgeOrder[i].x, 10, sortedEdgeOrder[i].y));
            }
            for(int i = 3; i < verticies.Count; i+=2)
            {
                triangles.Add(i - 1);
                triangles.Add(i - 2);
                triangles.Add(i - 3);

                triangles.Add(i - 2);
                triangles.Add(i - 1);
                triangles.Add(i);
            }
            if (verticies.Count > 3)             //add 0 to final verticies triangles
            {
                triangles.Add(0);
                triangles.Add(1);
                triangles.Add(verticies.Count - 1);

                triangles.Add(0);
                triangles.Add(verticies.Count - 1);
                triangles.Add(verticies.Count - 2);
            }
            verticies.Add(new Vector3(centerDebug.x, -10, centerDebug.y));
            verticies.Add(new Vector3(centerDebug.x, 10, centerDebug.y));
            for (int i = 3; i < verticies.Count; i += 2)
            {
                triangles.Add(i - 3);
                triangles.Add(i - 1);
                triangles.Add(verticies.Count - 2);

                triangles.Add(i - 2);
                triangles.Add(i);
                triangles.Add(verticies.Count - 1);
            }
            mesh.vertices = verticies.ToArray();
            mesh.triangles = triangles.ToArray();
            //mesh.triangles = mesh.triangles.Reverse().ToArray();
            mesh.RecalculateNormals();
            Vector3[] normals = mesh.normals;
            mesh.SetNormals(normals);
                //GameObject obMesh = new GameObject();
                //obMesh.transform.parent = transform;
                //MeshFilter mFil = obMesh.AddComponent<MeshFilter>();
                //MeshRenderer meRen = obMesh.AddComponent<MeshRenderer>();
                //meRen.material = clearIslandOuterColliderMaterial;
                //MeshCollider mCol = obMesh.AddComponent<MeshCollider>();
                //mCol.sharedMesh = mesh;
                //mFil.mesh = mesh;
                //obMesh.tag = "IslandOuterCollider";
                //obMesh.layer = 9;
            //AssetDatabase.CreateAsset( mesh, "Assets/SaveMeshes/island_collider_" + saveForDebugColliderNumber + ".asset");
            //saveForDebugColliderNumber++;
            returnMeshes.Add(mesh);
        }
        //AssetDatabase.SaveAssets();
        return returnMeshes;
    }
    private Vector2 GetCenterCoord(List<Vector2> lst)
    {
        Vector2 ret = Vector2.zero;
        foreach (Vector2 point in lst)
            ret += point;
        ret /= lst.Count;
        return ret;
    }
    private class HelpIslandPos
    {
        public int crossNeighorCount = 0;
        public int[] neighborIndexes = new int[] { -1, -1, -1, -1, -1, -1, -1, -1 };
        public HelpIslandPos(Vector2 coord)
        {

        }
    }
    private class EdgeCoordNeighboors
    {
        public int index;
        public List<int> neighboors;
        public EdgeCoordNeighboors(int index)
        {
            this.index = index;
            neighboors = new();
        }
    }
}
