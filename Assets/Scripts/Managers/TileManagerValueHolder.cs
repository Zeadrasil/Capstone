using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManagerValueHolder : MonoBehaviour
{
    [SerializeField] Tilemap BlockerTilemap;
    [SerializeField] Tilemap TraversableTilemap;
    [SerializeField] TileBase blockerTile;
    [SerializeField] TileBase traversableTile;
    [SerializeField] TileBase blockerResourceTile;
    [SerializeField] TileBase traversableResourceTile;
    // Start is called before the first frame update
    void Start()
    {
        TileManager.Instance.BlockerTilemap = BlockerTilemap;
        TileManager.Instance.TraversableTilemap = TraversableTilemap;
        TileManager.Instance.blockerTile = blockerTile;
        TileManager.Instance.traversableTile = traversableTile;
        TileManager.Instance.blockerResourceTile = blockerResourceTile;
        TileManager.Instance.traversableResourceTile = traversableResourceTile;
    }
}
