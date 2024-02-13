using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSurroundFoam : MonoBehaviour
{
    private List<Vector3> rightPoints = new();
    private List<Vector3> leftPoints = new();
    [SerializeField] TrailRenderer backTrailRenderer;
    [SerializeField] Transform front;
    private MeshFilter meshFilter;
    private Material material;
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        lastPositions.Add(Vector2.zero);
        material = GetComponent<Renderer>().material;
    }
    public void SetWakeFoam(List<Vector3> rightPoints, List<Vector3> leftPoints)
    {
        this.rightPoints = rightPoints;
        this.leftPoints = leftPoints;
    }
    List<Vector3> lastPositions = new();
    void Update()
    {
        material.SetVector("_Position", transform.position);
        Mesh mesh = new Mesh();
        Vector3 back = Vector3.zero;
        if (backTrailRenderer.positionCount > 0) back = transform.InverseTransformPoint(backTrailRenderer.GetPosition(0));


        Vector3 farRight = Vector3.zero;
        if (rightPoints.Count > 0) farRight = transform.InverseTransformPoint(rightPoints[0]);
        Vector3 farLeft = Vector3.zero;
        if (leftPoints.Count > 0) farLeft = transform.InverseTransformPoint(leftPoints[0]);

        lastPositions[0] = Vector3.Lerp(lastPositions[0], back, Time.deltaTime * 10);
        List<Vector3> verticiesL = new List<Vector3>   {                                                                                        //for back
                                            Vector3.zero, new Vector3(farLeft.x, 0, farLeft.z), new Vector3(lastPositions[0].x, 0, lastPositions[0].z),
                                            new Vector3(farRight.x, 0, farRight.z)
                                        };
        List<int> trianglesL = new List<int> { 2, 1, 0, 0, 3, 2 };                                                                              //for back
        verticiesL.Add(transform.InverseTransformPoint(front.position));            //setting front vert
        verticiesL[^1] = new Vector3(verticiesL[^1].x, 0, verticiesL[^1].z);        //setting front vert
        int centerTriIndex = 0;
        int frontTriIndex = 5;

        float frontSideTriCount = 4;
        int rightSideTriOffset = Mathf.Max(Mathf.FloorToInt(rightPoints.Count / frontSideTriCount), 1);
        int leftSideTriOffset = Mathf.Max(Mathf.FloorToInt(leftPoints.Count / frontSideTriCount), 1);
        int prevRightIndex = 0;
        int prevRightTriIndex = 3;
        for (int r = rightSideTriOffset; r < rightPoints.Count; r += rightSideTriOffset)    //add right front triangles
        {
            if(prevRightIndex != r)
            {
                verticiesL.Add(transform.InverseTransformPoint(rightPoints[r]));
                trianglesL.Add(0);
                trianglesL.Add(verticiesL.Count - 1);
                trianglesL.Add(prevRightTriIndex);
                prevRightTriIndex = verticiesL.Count - 1;
                prevRightIndex = r;
            }
        }
        //trianglesL.Add(0);                                                               //add final right front connecting to center
        //trianglesL.Add(frontTriIndex);
        //trianglesL.Add(verticiesL.Count - 1);
        int prevLeftIndex = 0;
        int prevLeftTriIndex = 1;
        for (int l = leftSideTriOffset; l < leftPoints.Count; l += leftSideTriOffset)    //add right front triangles
        {
            if (prevLeftIndex != l)
            {
                verticiesL.Add(transform.InverseTransformPoint(leftPoints[l]));
                trianglesL.Add(prevLeftTriIndex);
                trianglesL.Add(verticiesL.Count - 1);
                trianglesL.Add(0);
                prevLeftTriIndex = verticiesL.Count - 1;
                prevLeftIndex = l;
            }
        }


        mesh.vertices = verticiesL.ToArray();
        mesh.triangles = trianglesL.ToArray();
        meshFilter.mesh = mesh;
    }
}
