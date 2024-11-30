using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MarchingCubes : MonoBehaviour
{

    [SerializeField] private GizmoDemo noiseDemo;
    [SerializeField] private float seconds = .1f;
    private Vector3 marchCubePos;


    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();

    private MeshFilter meshFilter;
    private Mesh mesh;

    void Start() {
        meshFilter = this.GetComponent<MeshFilter>();
        StartCoroutine(march(Vector3.one * noiseDemo.size));
    }

    private IEnumerator march(Vector3 size) {
        mesh = new Mesh();
        for (int y = 0; y < size.y - 1; y++)
            for (int z = 0; z < size.z - 1; z++)
                for (int x = 0; x < size.x - 1; x++)
                {   
                    marchCubePos = new Vector3(x, y, z);
                    int index = 0;

                    float[] cubeCorners = new float[8];
                    for (int i = 0; i < 8; i++)
                    {
                        Vector3Int corner = new Vector3Int(x, y, z) + MarchingTable.Corners[i];
                        cubeCorners[i] = noise(corner.x, corner.y, corner.z);
                        if (!(cubeCorners[i] <= noiseDemo.threshold))
                        {
                            index |= 1 << i;
                        }
                    }

                    int edge = 0;
                    for (int triangle = 0; triangle < 5; triangle++)
                        for (int vert = 0; vert < 3; vert++)
                        {
                            int tri = MarchingTable.Triangles[index, edge];
                            if (tri == -1)
                                break;
                            
                            Vector3 edgeStart = marchCubePos + MarchingTable.Edges[tri, 0];
                            Vector3 edgeEnd = marchCubePos + MarchingTable.Edges[tri, 1];

                            Vector3 vertex = (edgeStart + edgeEnd) / 2;
                            vertices.Add(vertex);
                            triangles.Add(vertices.Count - 1);

                            edge++;
                        }

                    
                    mesh.vertices = vertices.ToArray();
                    mesh.triangles = triangles.ToArray();
                    mesh.RecalculateBounds();
                    mesh.RecalculateNormals();
                    meshFilter.mesh = mesh;
                    yield return new WaitForSeconds(seconds);
                }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }


    private float noise(float x, float y, float z)
    {
        int octaves = noiseDemo.octaves;
        float scalar = noiseDemo.scalar;

        float perlinValue = 0;
        float scale = 1.0f;
        for (int o = 0; o < (octaves + 1); o++)
        {
            perlinValue += scale * Perlin.Noise3D(x * scalar * (float)o, y * scalar * (float)o, z * scalar * (float)o);
            scale /= 2.0f;
        }
        perlinValue += 1.0f;
        perlinValue /= 2.0f;
        return perlinValue;
    }


    void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(Vector3.one * ((noiseDemo.size)/2 - .5f), Vector3.one * (noiseDemo.size - 1));

        if (marchCubePos.x == (noiseDemo.size - 2) && marchCubePos.y == (noiseDemo.size - 2) && marchCubePos.z == (noiseDemo.size - 2))
            return;

        Gizmos.DrawWireCube(marchCubePos + Vector3.one * .5f, Vector3.one);
    }
}