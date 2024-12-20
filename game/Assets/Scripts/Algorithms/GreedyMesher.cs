using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.AI;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GreedyMesher : MonoBehaviour
{
    struct MesherAnalyser {
        public Vector3 position;
        public Vector3 size;
    };

    private MesherAnalyser mesherAnalyser;
    private Color[] possibleColors;

    Color[] GenerateIntenseColors(int count)
    {
        Color[] colors = new Color[count];
        for (int i = 0; i < count; i++)
        {
            float hue = i / (float)count; // Spread hues evenly between 0 and 1
            float saturation = 1.0f; // Maximum saturation
            float value = 1.0f; // Maximum brightness
            colors[i] = Color.HSVToRGB(hue, saturation, value);
        }
        return colors;
    }

    [SerializeField] private float scalar = .2f;
    [SerializeField] private int size = 20;
    [SerializeField] private float seconds = .1f;
    [SerializeField] private bool twoD = false;
    [SerializeField] private bool primitives = false;
    [SerializeField] private bool greedy = false;

    private MeshFilter meshFilter;
    private Mesh mesh;

    private int[,] quad_data;
    private int[,,] block_data;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> indices = new List<int>();
    private List<Color> colors = new List<Color>();
    private List<Vector3> normals = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();

    void Start()
    {
        possibleColors = GenerateIntenseColors(100);

        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();

        quad_data = new int[size, size];
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float noise = Perlin.Noise2D(x * scalar, y * scalar);
                noise += 1.0f;
                noise /= 2.0f;
                quad_data[x, y] = Mathf.RoundToInt(noise);
            }


        block_data = new int[size, size, size];
        for (int y = 0; y < size; y++)
            for (int z = 0; z < size; z++)
                for (int x = 0; x < size; x++)
                {
                    float noise = Perlin.Noise3D(x * scalar, y * scalar, z * scalar);
                    noise += 1.0f;
                    noise /= 2.0f;

                    if (y == 0)
                        noise = 1.0f;

                    block_data[x, y, z] = Mathf.RoundToInt(noise);
                }
        
        if (twoD)
            StartCoroutine(greedyMesh());
        else
        {
            if (primitives)
            {
                StartCoroutine(primitive3D());
            }
            else
            {
                if (!greedy)
                    StartCoroutine(greedyMesh3D());
                else 
                    StartCoroutine(realgreedyMesh3D());
            }
        }
    }

    IEnumerator greedyMesh() {
        int verts = 0;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                mesherAnalyser.position = new Vector3(x, 0, y);
                mesherAnalyser.size = Vector3.one;

                if (quad_data[x, y] == 0)
                    continue;

                int width = 0;
                int height = 0;



                for (int vertical = y; vertical < size && quad_data[x, vertical] == 1; vertical++)
                {
                    height++;
                    mesherAnalyser.size.z = height;
                    yield return new WaitForSeconds(seconds/2);
                }

                bool isValid = true;
                for (int horizontal = x; horizontal < size && isValid; horizontal++)
                {
                    for (int vertical = y; vertical < y + height; vertical++)
                    {
                        if (quad_data[horizontal, vertical] == 0)
                        {
                            isValid = false;
                            break;
                        }
                    }

                    if (isValid)
                        width++;

                    mesherAnalyser.size.x = width;
                    yield return new WaitForSeconds(seconds/2);
                }

                for (int hx = x; hx < x + width; hx++)
                {
                    for (int hy = y; hy < y + height; hy++)
                    {
                        quad_data[hx, hy] = 0; 
                    }
                }

                vertices.Add(new Vector3(x, 0, y));
                vertices.Add(new Vector3(x, 0, y + height));
                vertices.Add(new Vector3(x + width, 0, y));
                vertices.Add(new Vector3(x + width, 0, y + height));

                indices.Add(verts + 0);
                indices.Add(verts + 1);
                indices.Add(verts + 2);
                        
                indices.Add(verts + 1);
                indices.Add(verts + 3);
                indices.Add(verts + 2);
                
                   
                verts += 4;
                Color color = possibleColors[(int)Random.Range(0, possibleColors.Length)];                colors.AddRange(new Color[] {color, color, color, color});
                mesh.vertices = vertices.ToArray();
                mesh.triangles = indices.ToArray();
                mesh.SetColors(colors.ToArray());
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                meshFilter.mesh = mesh;

                yield return new WaitForSeconds(seconds);
            }
        }

        mesherAnalyser.size = Vector3.zero;
    }

    IEnumerator greedyMesh3D() {
        Vector3Int[] directions = new Vector3Int[] {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, -1)
        };

        for (int y = 0; y < size; y++)
        {
            for (int z = 0; z < size; z++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (block_data[x, y, z] == 0)
                        continue;

                    foreach (var direction in directions)
                    {
                        if (isFaceVisible(x, y, z, size, direction))
                        {
                            GenerateFace(x, y, z, direction, vertices, indices, normals, uvs, 1);
                        }
                    }
                    yield return new WaitForSeconds(seconds);
                }
            }
        }
    }

    IEnumerator realgreedyMesh3D() {
        yield return new WaitForSeconds(seconds);
    }

    IEnumerator primitive3D() {
        for (int y = 0; y < size; y++)
        {
            for (int z = 0; z < size; z++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (block_data[x, y, z] == 0)
                        continue;
                
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(x, y, z);
                    yield return new WaitForSeconds(seconds);
                }
            }
        }
    }

    void GenerateFace(int x, int y, int z, Vector3Int direction, List<Vector3> vertices, List<int> triangles, List<Vector3> normals, List<Vector2> uvs, int blockType)
    {
        Vector3[] faceVertices;
        Vector3 faceNormal = direction;
        Vector3 basePosition = new Vector3(x, y, z);
        
        if (direction == new Vector3Int(1, 0, 0)) // Right
        {
            faceVertices = new Vector3[]
            {
                basePosition + new Vector3(1, 0, 0),
                basePosition + new Vector3(1, 1, 0),
                basePosition + new Vector3(1, 1, 1),
                basePosition + new Vector3(1, 0, 1)
            };
        }
        else if (direction == new Vector3Int(-1, 0, 0)) // Left
        {
            faceVertices = new Vector3[]
            {
                basePosition + new Vector3(0, 0, 1),
                basePosition + new Vector3(0, 1, 1),
                basePosition + new Vector3(0, 1, 0),
                basePosition + new Vector3(0, 0, 0)
            };
        }
        else if (direction == new Vector3Int(0, 1, 0)) // Up
        {
            faceVertices = new Vector3[]
            {
            
                basePosition + new Vector3(0, 1, 1),
                basePosition + new Vector3(1, 1, 1),
                basePosition + new Vector3(1, 1, 0),
                basePosition + new Vector3(0, 1, 0)
            };
        }
        else if (direction == new Vector3Int(0, -1, 0)) // Down
        {
            faceVertices = new Vector3[]
            {
                
                basePosition + new Vector3(0, 0, 0),
                basePosition + new Vector3(1, 0, 0),
                basePosition + new Vector3(1, 0, 1),
                basePosition + new Vector3(0, 0, 1)
            };
        }
        else if (direction == new Vector3Int(0, 0, 1)) // Forward
        {
            faceVertices = new Vector3[]
            {
                basePosition + new Vector3(0, 0, 1),
                basePosition + new Vector3(1, 0, 1),
                basePosition + new Vector3(1, 1, 1),
                basePosition + new Vector3(0, 1, 1)
            };
        }
        else // Backward
        {
            faceVertices = new Vector3[]
            {
                basePosition + new Vector3(1, 0, 0),
                basePosition + new Vector3(0, 0, 0),
                basePosition + new Vector3(0, 1, 0),
                basePosition + new Vector3(1, 1, 0)
            };
        }

        int baseIndex = vertices.Count;
        vertices.AddRange(faceVertices);

        triangles.Add(baseIndex);
        triangles.Add(baseIndex + 1);
        triangles.Add(baseIndex + 2);
        triangles.Add(baseIndex);
        triangles.Add(baseIndex + 2);
        triangles.Add(baseIndex + 3);

        normals.AddRange(new Vector3[] { faceNormal, faceNormal, faceNormal, faceNormal });
        uvs.AddRange(new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) });

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateBounds();
        meshFilter.mesh = mesh;
    }

    bool isFaceVisible(int x, int y, int z, int size, Vector3Int direction)
    {
        int nx = x + direction.x;
        int ny = y + direction.y;
        int nz = z + direction.z;

        if (nx < 0 || nx >= size || ny < 0 || ny >= size || nz < 0 || nz >= size)
            return true;
        
        return block_data[nx, ny, nz] == 0;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.white;

        if (twoD)
        {
            Gizmos.DrawWireCube(new Vector3(size/2, .5f, size/2), new Vector3(1 * size, 1f, 1 * size));
            Gizmos.DrawWireCube(mesherAnalyser.position + (mesherAnalyser.size / 2), mesherAnalyser.size);
        }
        else {
            Gizmos.DrawWireCube(new Vector3(size/2, size/2, size/2), new Vector3(size, size, size));
        }
    }
}