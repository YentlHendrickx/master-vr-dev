using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExtrapolateShape : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Get sprite
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite sprite = spriteRenderer.sprite;
        List<Vector2> blackPixels = new List<Vector2>();

        // Find black pixels
        for (int x = 0; x < sprite.texture.width; x++)
        {
            for (int y = 0; y < sprite.texture.height; y++)
            {

                Color pixelColor = sprite.texture.GetPixel(x, y);
                if (!IsAlmostWhite(pixelColor))
                {
                    blackPixels.Add(new Vector2(x, y));
                }
            }
        }

        // Parent object
        GameObject parent = new GameObject("ExtrapolatedShape3D");
        float count = 0;
        // Group pixels and create 3D objects
        foreach (var group in GroupPixels(blackPixels))
        {
            CreateCube(group, parent);
            count++;
        }

        Debug.Log("Number of cubes: " + count);

        // Rotate parent 90 degress flat
        parent.transform.Rotate(90, 0, 0);
        // Sacle z up
        parent.transform.localScale = new Vector3(1, 1, 10f);
    }

    private bool IsAlmostWhite(Color color, float offset = 0.1f)
    {
        return color.r >= 1 - offset && color.g >= 1 - offset && color.b >= 1 - offset;
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


    // Update is called once per frame
    void Update()
    {

    }
}
