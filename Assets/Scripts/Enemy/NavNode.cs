using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NavNode class is used to generate paths for Enemies and EnemyCheckpoints
public class NavNode
{
    //Navnode data
    public List<NavNode> neighbors;
    public NavNode parent;
    public Vector2Int location;
    public Vector3 position;
    public float cost;
    private static int tracker;
    private int id;

    //Constructor only allows NavNodes with locations
    public NavNode(Vector2Int location)
    {
        this.location = location;
        neighbors = new List<NavNode>();
        id = tracker++;
        position = TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(location.x, location.y));
    }
    public NavNode(Vector2Int location, Vector3 position)
    {
        this.location = location;
        neighbors = new List<NavNode>();
        id = tracker++;
        this.position = position;
    }

    public bool Equals(NavNode other)
    {
        return id == other.id;
    }
}
