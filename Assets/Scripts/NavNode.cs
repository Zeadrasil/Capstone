using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavNode
{
    public List<NavNode> neighbors;
    public NavNode parent;
    public Vector2Int location;
    public float cost;
    public NavNode(Vector2Int location)
    {
        this.location = location;
        neighbors = new List<NavNode>();
    }
}
