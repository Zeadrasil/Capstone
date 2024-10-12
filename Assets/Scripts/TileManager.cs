using Priority_Queue;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : Singleton<TileManager>
{
    public enum TileDirection
    {
        RightStright,
        RightDown,
        LeftDown,
        LeftStraight,
        LeftUp,
        RightUp
    }

    public static TileDirection FromRelativePosition(Vector2 relativePosition)
    {
        Vector2 checker = new Vector2(1, 0);
        for(int i = 0; i < 6; i++)
        {
            if (Vector2.Distance(checker, relativePosition.normalized) < 0.5f)
            {
                return (TileDirection)i;
            }
            checker = Quaternion.AngleAxis(60, new Vector3(0, 0, 1)) * checker;
        }
        return 0;
    }

    public static Quaternion RotateToDirection(TileDirection direction)
    {
        return Quaternion.AngleAxis(60 * ((int)direction), new Vector3(0, 0, 1));
    }

    int xOffset, yOffset;
    public Tilemap Tilemap;
    [SerializeField] TileBase blockerTile;
    [SerializeField] TileBase traversableTile;
    [SerializeField] TileBase blockerResourceTile;
    [SerializeField] TileBase traversableResourceTile;
    int mapScaling = 300000000;
    float resourceScaling = 1.5f;
    public Dictionary<Vector2Int, NavNode> Adjacencies = new Dictionary<Vector2Int, NavNode>();
    public List<Vector2Int> BlockerTiles = new List<Vector2Int>();
    private List<Vector2Int> nextExpansion = new List<Vector2Int> { new Vector2Int(0, 0) };
    private List<Vector2Int> subbedTiles = new List<Vector2Int>();
    public List<Vector2Int> potentialSpawnpoints = new List<Vector2Int>();
    public float traversableCutoff = 0.45f;
    public float resourceCutoff = 0.9f;
    private uint seedA;
    private uint seedB;
    private uint seedC;
    private uint seedD;
    private uint seedE;
    private uint seedF;
    // Start is called before the first frame update
    void Start()
    {
        seedA = ((uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue)) + int.MaxValue;
        seedB = ((uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue)) + int.MaxValue;
        seedC = ((uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue)) + int.MaxValue;
        seedD = ((uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue)) + int.MaxValue;
        seedE = ((uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue)) + int.MaxValue;
        seedF = ((uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue)) + int.MaxValue;
        for (int i = 0; i < 15; i++)
        {
            Next();
        }
        Debug.Log("Seed A: " + seedA);
        Debug.Log("Seed B: " + seedB);
        Debug.Log("Seed C: " + seedC);
        Debug.Log("Seed D: " + seedD);
        Debug.Log("Seed E: " + seedE);
        Debug.Log("Seed F: " + seedF);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Next()
    {
        List<Vector2Int> newExpansion = new List<Vector2Int>();
        List<Vector2Int> potentialSpawns = new List<Vector2Int>();
        foreach (Vector2Int location in nextExpansion)
        {
            Generate(location);
            if(Check(location.x, location.y) >= 0.45 || subbedTiles.Contains(location))
            {
                potentialSpawns.Add(location);
            }
            newExpansion.Remove(location);
            foreach(Vector2Int adjacent in getAllAdjacent(location))
            {
                if(Tilemap.GetTile(new Vector3Int(adjacent.x, adjacent.y, 0)) == null && !newExpansion.Contains(adjacent))
                {
                    newExpansion.Add(adjacent);
                }
            }

        }
        if(potentialSpawns.Count == 0)
        {
            NavNode foundNode = findPathFromNextValidSpawn(identifyNextValidSpawn(nextExpansion));
            while(foundNode.parent != null)
            {
                if (foundNode.parent.parent == null)
                {
                    potentialSpawnpoints = new List<Vector2Int>() { foundNode.location };
                }
                foundNode = foundNode.parent;
                subbedTiles.Add(foundNode.location);
                if(BlockerTiles.Contains(foundNode.location))
                {
                    Generate(foundNode.location);
                    BlockerTiles.Remove(foundNode.location);
                }
            }
        }
        else
        {
            potentialSpawnpoints = potentialSpawns;
        }
        nextExpansion = newExpansion;
    }

    private Vector2Int identifyNextValidSpawn(List<Vector2Int> initialExpansion)
    {
        List<Vector2Int> currentExpansion = new List<Vector2Int>(initialExpansion);
        List<Vector2Int> nextExpansion = new List<Vector2Int>();
        for (int i = 0; i < 200; i++)
        {
            foreach (Vector2Int location in currentExpansion)
            {
                if (Tilemap.GetTile(new Vector3Int(location.x, location.y)) == null && Check(location.x, location.y) >= traversableCutoff)
                {
                    return location;
                }
                foreach (Vector2Int adjacent in getAllAdjacent(location))
                {
                    if (!currentExpansion.Contains(adjacent) && !nextExpansion.Contains(adjacent))
                    {
                        nextExpansion.Add(adjacent);
                    }
                }
            }
            currentExpansion = nextExpansion;
            nextExpansion = new List<Vector2Int>();
        }
        return new Vector2Int(0, 0);
    }

    private NavNode findPathFromNextValidSpawn(Vector2Int firstValid)
    {
        List<Vector2Int> checkedSpots = new List<Vector2Int>();
        NavNode central = new NavNode(firstValid);
        Queue<NavNode> nodesToExpandFrom = new Queue<NavNode>();
        Dictionary<Vector2Int, NavNode> nodes = new Dictionary<Vector2Int, NavNode>();
        nodes.Add(firstValid, central);
        nodesToExpandFrom.Enqueue(central);
        while (nodesToExpandFrom.Count > 0)
        {
            NavNode current = nodesToExpandFrom.Dequeue();
            foreach (Vector2Int adjacency in getAllAdjacent(current.location))
            {
                if (nodes.TryGetValue(adjacency, out NavNode node))
                {
                    if (!current.neighbors.Contains(node))
                    {
                        current.neighbors.Add(node);
                    }
                    if (!node.neighbors.Contains(current))
                    {
                        node.neighbors.Add(current);
                    }
                }
                else
                {
                    NavNode next = new NavNode(adjacency);
                    current.neighbors.Add(next);
                    next.neighbors.Add(current);
                    nodes.Add(adjacency, next);
                    if (Adjacencies.ContainsKey(next.location))
                    {
                        foreach (NavNode nodeToClear in nodes.Values)
                        {
                            nodeToClear.parent = null;
                            nodeToClear.cost = float.MaxValue;
                        }
                        NavNode start = next;
                        start.cost = 0;
                        Vector2Int goal = firstValid;
                        SimplePriorityQueue<NavNode, float> queuedNodes = new SimplePriorityQueue<NavNode, float>();
                        queuedNodes.EnqueueWithoutDuplicates(start, Vector3.Distance(Tilemap.CellToWorld(new Vector3Int(start.location.x, start.location.y, 0)), transform.position));
                        bool found = false;
                        while (!found && queuedNodes.Count > 0)
                        {
                            NavNode currentNode = queuedNodes.Dequeue();
                            if (currentNode.location.x == goal.x && currentNode.location.y == goal.y)
                            {
                                found = true;
                                break;
                            }
                            foreach (NavNode adjacentNode in currentNode.neighbors)
                            {
                                float cost = currentNode.cost + Vector3.Distance(Tilemap.CellToWorld(new Vector3Int(adjacentNode.location.x, adjacentNode.location.y)), Tilemap.CellToWorld(new Vector3Int(currentNode.location.x, currentNode.location.y)));
                                if (cost < adjacentNode.cost)
                                {
                                    adjacentNode.cost = cost;
                                    adjacentNode.parent = currentNode;
                                    queuedNodes.EnqueueWithoutDuplicates(adjacentNode, adjacentNode.cost + Vector3.Distance(Tilemap.CellToWorld(new Vector3Int(adjacentNode.location.x, adjacentNode.location.y)), transform.position));
                                }
                            }
                        }
                        if(nodes.TryGetValue(firstValid, out NavNode finalNode))
                        {
                            return finalNode;
                        }
                        return null;
                    }
                    else
                    {
                        nodesToExpandFrom.Enqueue(next);
                    }
                }
            }
        }
        return null;
    }


    private Vector2Int[] getAllAdjacent(Vector2Int location)
    {
        Vector2Int[] adjacents = new Vector2Int[6];
        Vector2 world = Tilemap.CellToWorld(new Vector3Int(location.x, location.y, 0));
        Vector2 adjacancyDetector = new Vector2(1, 0);
        for (int i = 0; i < 6; i++)
        {
            adjacents[i] = (Vector2Int)Tilemap.WorldToCell(world + adjacancyDetector);
            adjacancyDetector = Quaternion.AngleAxis(60, new Vector3(0, 0, 1)) * adjacancyDetector;
        }
        return adjacents;
    }
    public void Generate(Vector2Int location)
    {
        Generate(location.x, location.y);
    }
    public void Generate(int x, int y)
    {
        float tileValue = Check(x, y);
        if (tileValue < traversableCutoff && !subbedTiles.Contains(new Vector2Int(x, y)))
        {
            if (CheckResource(x, y) >= resourceCutoff)
            {
                Tilemap.SetTile(new Vector3Int(x, y, 0), blockerResourceTile);
            }
            else
            {
                Tilemap.SetTile(new Vector3Int(x, y, 0), blockerTile);
            }
            BlockerTiles.Add(new Vector2Int(x, y));
        }
        else
        {
            if (CheckResource(x, y) >= resourceCutoff)
            {
                Tilemap.SetTile(new Vector3Int(x, y, 0), traversableResourceTile);
            }
            else
            {
                Tilemap.SetTile(new Vector3Int(x, y, 0), traversableTile);
            }
            Vector2 world = Tilemap.CellToWorld(new Vector3Int(x, y, 0));
            Vector2 adjacancyDetector = new Vector2(1, 0);
            NavNode addedNode = new NavNode(new Vector2Int(x, y));
            for (int i = 0; i < 6; i++)
            {
                Vector2Int currentCheck = (Vector2Int)Tilemap.WorldToCell(world + adjacancyDetector);
                if(Adjacencies.TryGetValue(currentCheck, out NavNode adjacentNode))
                {
                    addedNode.neighbors.Add(adjacentNode);
                    adjacentNode.neighbors.Add(addedNode);
                }
                adjacancyDetector = Quaternion.AngleAxis(60, new Vector3(0, 0, 1)) * adjacancyDetector;
            }
            Adjacencies.TryAdd(new Vector2Int(x, y), addedNode);
        }

    }
    public float Check(int x, int y)
    {
        float actualx = (x + xOffset) / (float)int.MaxValue * mapScaling;
        float actualy = (y + yOffset) / (float)int.MaxValue * mapScaling;
        //return Mathf.PerlinNoise(actualx, actualy);
        return PerlinGenerator.Noise(actualx, actualy, seedA, seedB, seedC);
    }

    public float Check(Vector2Int coords)
    {
        return Check(coords.x, coords.y);
    }

    public float CheckResource(int x, int y)
    {
        float actualx = (x + xOffset) / (float)int.MaxValue * mapScaling * resourceScaling;
        float actualy = (y + yOffset) / (float)int.MaxValue * mapScaling * resourceScaling;

        return PerlinGenerator.Noise(actualx, actualy, seedD, seedE, seedF);
    }

    public float CheckResource(Vector2Int coords)
    {
        return CheckResource(coords.x, coords.y);
    }
}
