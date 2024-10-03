using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGenerator : MonoBehaviour
{
    int xOffset, yOffset;
    [SerializeField] Tilemap tilemap;
    [SerializeField] TileBase blockerTile;
    [SerializeField] TileBase traversableTile;
    int mapScaling = 200000000;
    // Start is called before the first frame update
    void Start()
    {
        for (int x = -100; x <= 100; x++)
        {
            for(int y = -100; y <= 100; y++)
            {
                Generate(x, y);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Generate(int x, int y)
    {
        float actualx = (x + xOffset) / (float)int.MaxValue * mapScaling;
        float actualy = (y + yOffset) / (float)int.MaxValue * mapScaling;
        float tileValue = Mathf.PerlinNoise(actualx, actualy);
        if (tileValue < 0.45f)
        {
            tilemap.SetTile(new Vector3Int(x, y, 0), blockerTile);
        }
        else
        {
            tilemap.SetTile(new Vector3Int(x, y, 0), traversableTile);
        }

    }
}
