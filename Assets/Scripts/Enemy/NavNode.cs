using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

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
    private bool destroyed = false;

    //Constructor only allows NavNodes with locations
    public NavNode(Vector2Int location)
    {
        this.location = location;
        neighbors = new List<NavNode>();
        id = tracker++;
        position = TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(location.x, location.y));
    }
    //Other constructor requires location, but lets you put in position. For use with multithreading.
    public NavNode(Vector2Int location, Vector3 position)
    {
        this.location = location;
        neighbors = new List<NavNode>();
        id = tracker++;
        this.position = position;
    }

    //Determines whether two nodes are the same
    public bool Equals(NavNode other)
    {
        return id == other.id;
    }

    //Clear out connections to allow the garbage collector to tell this is useless
    public void Destroy()
    {
        //Prevent infinite loops everybody
        if (!destroyed)
        {
            destroyed = true;
            foreach (NavNode n in neighbors)
            {
                Task.Factory.StartNew(() => n.Destroy());
            }
            neighbors = null;
        }
    }
}
