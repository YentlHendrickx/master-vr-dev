using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshExtruder : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The sprite to extrapolate.")]
    public Sprite sprite;
    [Tooltip("How much extrusion")]
    public float zScale = 10f;
    [Range(0f, 1f)]
    [Tooltip("How lenient the color matching is. 0 is exact match and 1 is any color.")]
    public float hardness = 0.1f;
    [Tooltip("The color to match.")]
    public Color targetColor = Color.black;
    [Tooltip("The material to use for the mesh.")]
    public Material material;
    [Header("Debug")]
    [Tooltip("Recalculate the 3D shape, careful with large sprites.")]
    public bool recalculate = false;
    [Tooltip("Offset the mesh in the x direction.")]
    public float xOffset, yOffset, zOffet = 0.5f;

    // Parent object to hold all the meshes
    private GameObject parent;

    void Start()
    {
        if (sprite != null)
        {
            Initialize();
        }
    }

    private void Initialize()
    {
        if (parent != null)
        {
            Destroy(parent);
        }

        parent = new GameObject("MeshParent");
        Calculate();

        // Based on mesh size, move x left by half and y down by half
        float newX = -sprite.texture.width / 2;
        float newY = -sprite.texture.height / 1.5f;
        parent.transform.position = new Vector3(newX + xOffset, newY + yOffset, 0 + zOffet);
    }

    private void Calculate()
    {
        Sprite sprite = this.sprite;

        List<MeshData> meshes = new List<MeshData>();
        MeshData currentMeshData = new MeshData(parent, material, meshes.Count);
        meshes.Add(currentMeshData);

        // Iterate through all pixels
        for (int y = 0; y < sprite.texture.height; y++)
        {
            for (int x = 0; x < sprite.texture.width; x++)
            {
                Color pixelColor = sprite.texture.GetPixel(x, y);
                if (ColorWithinBounds(pixelColor, targetColor, hardness))
                {
                    if (currentMeshData.IsFull())
                    {
                        currentMeshData.CreateMesh();
                        currentMeshData = new MeshData(parent, material, meshes.Count);
                        meshes.Add(currentMeshData);
                    }

                    // Define vertices for the square
                    Vector3 v1 = new Vector3(x, y, 0);
                    Vector3 v2 = new Vector3(x + 1, y, 0);
                    Vector3 v3 = new Vector3(x, y + 1, 0);
                    Vector3 v4 = new Vector3(x + 1, y + 1, 0);

                    // Define top vertices of the 'pixel' square
                    Vector3 v5 = new Vector3(x, y, zScale);
                    Vector3 v6 = new Vector3(x + 1, y, zScale);
                    Vector3 v7 = new Vector3(x, y + 1, zScale);
                    Vector3 v8 = new Vector3(x + 1, y + 1, zScale);

                    // Add top and bottom faces
                    currentMeshData.AddQuad(v1, v3, v2, v4, pixelColor); // Bottom face
                    currentMeshData.AddQuad(v5, v6, v7, v8, pixelColor); // Top face

                    // Add side faces
                    currentMeshData.AddQuad(v1, v2, v5, v6, pixelColor); // Front face
                    currentMeshData.AddQuad(v3, v7, v4, v8, pixelColor); // Back face
                    currentMeshData.AddQuad(v1, v5, v3, v7, pixelColor); // Left face
                    currentMeshData.AddQuad(v2, v4, v6, v8, pixelColor); // Right face
                }
            }
        }

        // Finalize the last mesh
        if (currentMeshData.vertices.Count > 0)
        {
            currentMeshData.CreateMesh();
        }

        parent.transform.Rotate(90, 0, 0);
        parent.transform.localScale = new Vector3(1, 1, zScale);
    }

    private bool ColorWithinBounds(Color color, Color targetColor, float hardness = 1)
    {
        return Mathf.Abs(color.r - targetColor.r) <= hardness &&
               Mathf.Abs(color.g - targetColor.g) <= hardness &&
               Mathf.Abs(color.b - targetColor.b) <= hardness;
    }

    public void Update()
    {
        if (recalculate && sprite != null)
        {
            recalculate = false;
            Initialize();
        }
    }
}
