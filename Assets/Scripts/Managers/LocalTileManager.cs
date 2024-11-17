using UnityEngine;
using UnityEngine.Tilemaps;

//TileManagerValueHolder holds values to be passed into the TileManager when loading into a scene
public class LocalTileManager : MonoBehaviour
{
    //Tilemaps
    [SerializeField] Tilemap BlockerTilemap;
    [SerializeField] Tilemap TraversableTilemap;

    //Base tiles
    [SerializeField] TileBase[] blockerTilesBase;
    [SerializeField] TileBase[] traversableTilesBase;
    [SerializeField] TileBase[] blockerResourceTilesBase;
    [SerializeField] TileBase[] traversableResourceTilesBase;

    //White outline tiles
    [SerializeField] TileBase[] whiteBlockerTiles;
    [SerializeField] TileBase[] whiteTraversableTiles;
    [SerializeField] TileBase[] whiteBlockerResourceTiles;
    [SerializeField] TileBase[] whiteTraversableResourceTiles;

    //Black outline tiles
    [SerializeField] TileBase[] blackBlockerTiles;
    [SerializeField] TileBase[] blackTraversableTiles;
    [SerializeField] TileBase[] blackBlockerResourceTiles;
    [SerializeField] TileBase[] blackTraversableResourceTiles;

    //Thick white outline tiles
    [SerializeField] TileBase[] thickWhiteBlockerTiles;
    [SerializeField] TileBase[] thickWhiteTraversableTiles;
    [SerializeField] TileBase[] thickWhiteBlockerResourceTiles;
    [SerializeField] TileBase[] thickWhiteTraversableResourceTiles;

    //Thick black outline tiles
    [SerializeField] TileBase[] thickBlackBlockerTiles;
    [SerializeField] TileBase[] thickBlackTraversableTiles;
    [SerializeField] TileBase[] thickBlackBlockerResourceTiles;
    [SerializeField] TileBase[] thickBlackTraversableResourceTiles;

    // Start is called before the first frame update
    void Start()
    {
        //Pass tilemaps
        TileManager.Instance.BlockerTilemap = BlockerTilemap;
        TileManager.Instance.TraversableTilemap = TraversableTilemap;

        //Pass tiles
        switch(GameManager.Instance.outlineType)
        {
            //No outline
            case 0:
                {
                    TileManager.Instance.blockerTiles = blockerTilesBase;
                    TileManager.Instance.blockerResourceTiles = blockerResourceTilesBase;
                    TileManager.Instance.traversableTiles = traversableTilesBase;
                    TileManager.Instance.traversableResourceTiles = traversableResourceTilesBase;
                    break;
                }
                //Black outline
            case 1:
                {
                    TileManager.Instance.blockerTiles = blackBlockerTiles;
                    TileManager.Instance.blockerResourceTiles = blackBlockerResourceTiles;
                    TileManager.Instance.traversableTiles = blackTraversableTiles;
                    TileManager.Instance.traversableResourceTiles = blackTraversableResourceTiles;
                    break;
                }
                //White outline
            case 2:
                {
                    TileManager.Instance.blockerTiles = whiteBlockerTiles;
                    TileManager.Instance.blockerResourceTiles = whiteBlockerResourceTiles;
                    TileManager.Instance.traversableTiles = whiteTraversableTiles;
                    TileManager.Instance.traversableResourceTiles = whiteTraversableResourceTiles;
                    break;
                }
                //Thick black outline
            case 3:
                {
                    TileManager.Instance.blockerTiles = thickBlackBlockerTiles;
                    TileManager.Instance.blockerResourceTiles = thickBlackBlockerResourceTiles;
                    TileManager.Instance.traversableTiles = thickBlackTraversableTiles;
                    TileManager.Instance.traversableResourceTiles = thickBlackTraversableResourceTiles;
                    break;
                }
                //Thick white outline
            case 4:
                {
                    TileManager.Instance.blockerTiles = thickWhiteBlockerTiles;
                    TileManager.Instance.blockerResourceTiles = thickWhiteBlockerResourceTiles;
                    TileManager.Instance.traversableTiles = thickWhiteTraversableTiles;
                    TileManager.Instance.traversableResourceTiles = thickWhiteTraversableResourceTiles;
                    break;
                }
        }
    }
}
