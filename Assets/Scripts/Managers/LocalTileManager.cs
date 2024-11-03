using UnityEngine;
using UnityEngine.Tilemaps;

//TileManagerValueHolder holds values to be passed into the TileManager when loading into a scene
public class LocalTileManager : MonoBehaviour
{
    //Tilemaps
    [SerializeField] Tilemap BlockerTilemap;
    [SerializeField] Tilemap TraversableTilemap;

    //Tiles
    [SerializeField] TileBase blockerTile;
    [SerializeField] TileBase traversableTile;
    [SerializeField] TileBase blockerResourceTile;
    [SerializeField] TileBase traversableResourceTile;
    // Start is called before the first frame update
    void Start()
    {
        //Pass tilemaps
        TileManager.Instance.BlockerTilemap = BlockerTilemap;
        TileManager.Instance.TraversableTilemap = TraversableTilemap;

        //Pass tiles
        TileManager.Instance.blockerTile = blockerTile;
        TileManager.Instance.traversableTile = traversableTile;
        TileManager.Instance.blockerResourceTile = blockerResourceTile;
        TileManager.Instance.traversableResourceTile = traversableResourceTile;
    }
}
