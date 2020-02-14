using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TileMap : MonoBehaviour
{
    public short sizeX;
    public short sizeZ;
    public float tileSize;

    private void Start()
    {

        BuildMesh();
    }

    void BuildMesh()
    {
        int numTiles = sizeX * sizeZ;
        int numTriangles = numTiles * 2;

        int vsizeX = sizeX + 1;
        int vsizeZ = sizeZ + 1;
        int numVertices = vsizeX * vsizeZ;

        Vector3[] vertices = new Vector3[numVertices];
        Vector3[] normals = new Vector3[numVertices];
        Vector2[] uv = new Vector2[numVertices];

        int[] triangles = new int[numTriangles * 3];

        int x, z, t;
        for (z = 0; z < sizeZ; z++)
        {
            for (x = 0; x < sizeX; x++)
            {
                t = z * vsizeX + x;
                vertices[t] = new Vector3(x * tileSize, 0, z* tileSize);
                normals[t] = Vector3.up;
                uv[t] = new Vector2((float)x / vsizeX,(float) z / vsizeZ);
            }
        }

        for (z = 0; z < sizeZ; z++)
        {
            for (x = 0; x < sizeX; x++)
            {
                int triOffset = (z * sizeX + x) * 6;
                t = z * vsizeX + x;
                triangles[triOffset + 0] = t + 0;
                triangles[triOffset + 1] = t + vsizeX + 0;
                triangles[triOffset + 2] = t + vsizeX + 1;

                triangles[triOffset + 3] = t + 0;
                triangles[triOffset + 4] = t + vsizeX + 1;
                triangles[triOffset + 5] = t +1;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = meshFilter.sharedMesh;
    }
}
