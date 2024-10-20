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
    public float cost;

    //Constructor only allows NavNodes with locations
    public NavNode(Vector2Int location)
    {
        this.location = location;
        neighbors = new List<NavNode>();
    }
}
