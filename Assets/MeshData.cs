using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Color> colors = new List<Color>();
    public Mesh mesh = new Mesh();
    public GameObject meshObject;

    public MeshData(GameObject parent, Material material, int index)
    {
        meshObject = new GameObject($"MeshPart_{index}");
        meshObject.transform.parent = parent.transform;
        var meshFilter = meshObject.AddComponent<MeshFilter>();
        var meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter.mesh = mesh;
        meshRenderer.material = material;
    }

    public bool IsFull()
    {
        return vertices.Count > 65000; // Keep under Unity's limit
    }

    public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Color c)
    {
        triangles.Add(vertices.Count);
        vertices.Add(v1);
        colors.Add(c);

        triangles.Add(vertices.Count);
        vertices.Add(v2);
        colors.Add(c);

        triangles.Add(vertices.Count);
        vertices.Add(v3);
        colors.Add(c);
    }

    public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Color c)
    {
        // First triangle
        triangles.Add(vertices.Count);
        vertices.Add(v1);
        colors.Add(c);

        triangles.Add(vertices.Count);
        vertices.Add(v2);
        colors.Add(c);

        triangles.Add(vertices.Count);
        vertices.Add(v3);
        colors.Add(c);

        // Second triangle
        triangles.Add(vertices.Count);
        vertices.Add(v3);
        colors.Add(c);

        triangles.Add(vertices.Count);
        vertices.Add(v2);
        colors.Add(c);

        triangles.Add(vertices.Count);
        vertices.Add(v4);
        colors.Add(c);
    }

    public void CreateMesh()
    {
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.colors = colors.ToArray();
        mesh.RecalculateNormals();
    }
}