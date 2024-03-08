using System;
using System.Collections.Generic;
using UnityEngine;

public class CubeExtrusion : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How tall the walls are")]
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

    private GameObject parent;

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (parent != null)
        {
            Destroy(parent);
        }

        parent = new GameObject("ExtrapolatedShape3D");
        Calculate();
    }

    private void Calculate()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite sprite = spriteRenderer.sprite;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Color> colors = new List<Color>();

        List<Vector2> pixels = getVertexList(sprite);
        Debug.Log("Number of cubes: " + pixels.Count);

        foreach (var group in GroupPixels(pixels))
        {
            CreateCube(group, parent);
        }

        parent.transform.Rotate(90, 0, 0);
        parent.transform.localScale = new Vector3(1, 1, zScale);
    }

    private List<Vector2> getVertexList(Sprite sprite)
    {
        List<Vector2> pixels = new List<Vector2>();

        for (int x = 0; x < sprite.texture.width; x++)
        {
            for (int y = 0; y < sprite.texture.height; y++)
            {

                Color pixelColor = sprite.texture.GetPixel(x, y);
                if (ColorWithinBounds(pixelColor, this.targetColor, this.hardness))
                {
                    pixels.Add(new Vector2(x, y));
                }
            }
        }

        return pixels;
    }

    private bool ColorWithinBounds(Color color, Color targetColor, float hardness = 1)
    {
        return Mathf.Abs(color.r - targetColor.r) <= hardness &&
             Mathf.Abs(color.g - targetColor.g) <= hardness &&
             Mathf.Abs(color.b - targetColor.b) <= hardness;
    }

    private IEnumerable<List<Vector2>> GroupPixels(List<Vector2> pixels)
    {
        // Sort pixels by Y, then X for grouping
        pixels.Sort((a, b) => a.y == b.y ? a.x.CompareTo(b.x) : a.y.CompareTo(b.y));

        List<Vector2> currentGroup = new List<Vector2>();
        Vector2? lastPixel = null;

        foreach (Vector2 pixel in pixels)
        {
            if (lastPixel != null && (pixel.y != lastPixel.Value.y || pixel.x != lastPixel.Value.x + 1))
            {
                if (currentGroup.Count > 0)
                {
                    yield return currentGroup;
                    currentGroup = new List<Vector2>();
                }
            }

            currentGroup.Add(pixel);
            lastPixel = pixel;
        }

        if (currentGroup.Count > 0)
        {
            yield return currentGroup;
        }
    }

    private void CreateCube(List<Vector2> pixels, GameObject parent)
    {
        float minY = pixels[0].y;
        float minX = float.MaxValue;
        float maxX = float.MinValue;

        foreach (Vector2 pixel in pixels)
        {
            if (pixel.x < minX) minX = pixel.x;
            if (pixel.x > maxX) maxX = pixel.x;
        }

        // Instantiate cube
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = parent.transform;
        cube.transform.position = transform.position + new Vector3(minX + (maxX - minX) / 2, minY, 0); // Center of the pixel group
        cube.transform.localScale = new Vector3(maxX - minX + 1, 1, 1); // Scale based on the group width
        cube.name = "ExtrapolatedShape3D";
    }
    public void Update()
    {
        if (recalculate)
        {
            recalculate = false;
            Initialize();
        }
    }
}
