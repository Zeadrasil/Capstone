using Priority_Queue;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

//TileManager manages all of the tile data for the map
public class TileManager : Singleton<TileManager>
{
    //Simple enum designed to allow you to know what direction things are
    public enum TileDirection
    {
        RightStright,
        RightDown,
        LeftDown,
        LeftStraight,
        LeftUp,
        RightUp
    }
    
    //Gets the direction from a direction vector, rounded to the closest direction
    public static TileDirection FromRelativePosition(Vector2 relativePosition)
    {
        //Creates a vector to use for checks
        Vector2 checker = new Vector2(1, 0);
        
        //Goes through and rotates the checking vector to check for a suitably close vector
        for(int i = 0; i < 6; i++)
        {
            //Returns the direction for the number of rotations if it results in a suitably close direction
            if (Vector2.Distance(checker, relativePosition.normalized) < 0.5f)
            {
                return (TileDirection)i;
            }
            //If it is not suitably close, rotate by 60 degrees
            checker = Quaternion.AngleAxis(60, new Vector3(0, 0, 1)) * checker;
        }
        //Returns the default if none can be found, which should be impossible
        return 0;
    }

    //Gets the quaternion that will rotate you to face the given direction
    public static Quaternion RotateToDirection(TileDirection direction)
    {
        return Quaternion.AngleAxis(60 * ((int)direction), new Vector3(0, 0, 1));
    }

    //Offset for when you do not spawn on a traversable tile
    int xOffset, yOffset;

    //Tilemaps
    public Tilemap TraversableTilemap;
    public Tilemap BlockerTilemap;

    //Tiles
    public TileBase blockerTile;
    public TileBase traversableTile;
    public TileBase blockerResourceTile;
    public TileBase traversableResourceTile;

    //Map scaling (compresses map to make gaps smaller)
    int mapScaling = 300000000;

    //Relative compression of resources compared to the normal map
    float resourceScaling = 2.5f;

    //Adjcenct tile container
    public Dictionary<Vector2Int, NavNode> Adjacencies = new Dictionary<Vector2Int, NavNode>();

    //All tiles that block enemies
    public List<Vector2Int> BlockerTiles = new List<Vector2Int>();

    //Tiles that are going to be expanded to next
    private List<Vector2Int> nextExpansion = new List<Vector2Int> { new Vector2Int(0, 0) };

    //Tiles that used to be blockers but have been replaced with traversable tiles in order to get a path to the base
    public List<Vector2Int> subbedTiles = new List<Vector2Int>();

    //Tiles that are traversable that were just spawned
    public List<Vector2Int> potentialSpawnpoints = new List<Vector2Int>();

    //Cutoffs for tile types
    public float traversableCutoff = 0.45f;
    public float resourceCutoff = 0.725f;

    //Seed data
    public bool customSeeds = false;
    public uint seedA;
    public uint seedB;
    public uint seedC;
    public uint seedD;
    public uint seedE;
    public uint seedF;
    // Start is called before the first frame update
    void Start()
    {
    }

    //Get the manager ready to run after allowing data to be passed in
    public void Initialize()
    {
        //Clear out data
        BlockerTilemap.ClearAllTiles();
        TraversableTilemap.ClearAllTiles();
        subbedTiles.Clear();
        nextExpansion.Clear();
        nextExpansion.Add(new Vector2Int(0, 0));
        BlockerTiles.Clear();
        Adjacencies.Clear();
        potentialSpawnpoints.Clear();

        //Generate 14 tiles out from the center
        for (int i = 0; i < 15; i++)
        {
            Next();
        }
    }

    //Generates a new layer of the map
    public void Next()
    {
        //Storage for tiles that the next expansion will use
        List<Vector2Int> newExpansion = new List<Vector2Int>();

        //Storage for tiles that enemies can spawn on
        List<Vector2Int> potentialSpawns = new List<Vector2Int>();

        //Goes through all tiles in the currect expansion to generate them
        foreach (Vector2Int location in nextExpansion)
        {
            //Generate the tile
            Generate(location);

            //If it is a traversable tile then add it to potential spawns
            if(Check(location.x, location.y) >= 0.45 || subbedTiles.Contains(location))
            {
                potentialSpawns.Add(location);
            }
            //Ensure that you do not have anythign in the next expansion that you are generating in this expansion
            newExpansion.Remove(location);

            //Go through all of the adjacent tiles to the current tile
            foreach(Vector2Int adjacent in getAllAdjacent(location))
            {
                //If it is an ungenerated tile that has not been added to the next expansion add it to the next expansion
                if(TraversableTilemap.GetTile(new Vector3Int(adjacent.x, adjacent.y, 0)) == null && BlockerTilemap.GetTile(new Vector3Int(adjacent.x, adjacent.y, 0)) == null && !newExpansion.Contains(adjacent))
                {
                    newExpansion.Add(adjacent);
                }
            }

        }

        //Backup plan for if there are no generated spawnpoints
        if(potentialSpawns.Count == 0)
        {
            //Gets a navnode that following will create a path to a spawnpoint in the future
            NavNode foundNode = findPathFromNextValidSpawn(identifyNextValidSpawn(nextExpansion));

            //Keep following nodes until you have gone the entire way
            while(foundNode.parent != null)
            {
                //Ensure that there is a stored spawnpoint at an appropriate spot
                if (foundNode.parent.parent == null)
                {
                    potentialSpawnpoints = new List<Vector2Int>() { foundNode.location };
                }
                //Follow node
                foundNode = foundNode.parent;

                //Add tile to subbed tiles
                subbedTiles.Add(foundNode.location);

                //If it is a blocker tile then regenerate it as a traversable tile
                if(BlockerTiles.Contains(foundNode.location))
                {
                    Generate(foundNode.location);

                    //Removes from blocker tiles as it is now a traversable tile
                    BlockerTiles.Remove(foundNode.location);
                }
            }
        }
        else
        {
            //If it generates at lest one spawnpoint, set the final variable.
            potentialSpawnpoints = potentialSpawns;
        }
        //Set the stored expansion to the next expansion
        nextExpansion = newExpansion;
    }

    //Gets the next valid spawnpoint
    private Vector2Int identifyNextValidSpawn(List<Vector2Int> initialExpansion)
    {
        //Stores the expansion that is currently in progress
        List<Vector2Int> currentExpansion = new List<Vector2Int>(initialExpansion);

        //Stores the expansion that will be done next
        List<Vector2Int> nextExpansion = new List<Vector2Int>();

        //Limits it to trying to find a spawnpoint within 200 tiles, if it is outside of this it is going to take forever to find anyways
        for (int i = 0; i < 200; i++)
        {
            //Goes through the current expansion to check every single tile
            foreach (Vector2Int location in currentExpansion)
            {
                //Checks to see if it is a valid spawn and returns it if it is
                if (TraversableTilemap.GetTile(new Vector3Int(location.x, location.y)) == null && Check(location.x, location.y) >= traversableCutoff)
                {
                    return location;
                }
                //If it is not add adjacent tiles to the next expansion
                foreach (Vector2Int adjacent in getAllAdjacent(location))
                {
                    //If the tile is in this expansion or it was already added skip it
                    if (!currentExpansion.Contains(adjacent) && !nextExpansion.Contains(adjacent))
                    {
                        nextExpansion.Add(adjacent);
                    }
                }
            }
            //Set the current expansion to the next expansion
            currentExpansion = nextExpansion;

            //Create a new list for the next expansion
            nextExpansion = new List<Vector2Int>();
        }
        //Did not find any, so return (0, 0)
        return new Vector2Int(0, 0);
    }

    //Finds a path to a generated traversible tile from a given spot
    private NavNode findPathFromNextValidSpawn(Vector2Int firstValid)
    {
        //List of spots that have already been checked
        List<Vector2Int> checkedSpots = new List<Vector2Int>();

        //Node that you are starting from
        NavNode central = new NavNode(firstValid);

        //Nodes that can be expanded from
        Queue<NavNode> nodesToExpandFrom = new Queue<NavNode>();

        //Stores all of the generated nodes
        Dictionary<Vector2Int, NavNode> nodes = new Dictionary<Vector2Int, NavNode>();

        //Adds the starting node to the dictionary
        nodes.Add(firstValid, central);

        //Adds the starting node to the expansion queue
        nodesToExpandFrom.Enqueue(central);

        //Keeps going until it either finds a path or runs out of expansions
        while (nodesToExpandFrom.Count > 0)
        {
            //Gets the first node in the queue
            NavNode current = nodesToExpandFrom.Dequeue();

            //Goes through all of its adjacencies
            foreach (Vector2Int adjacency in getAllAdjacent(current.location))
            {
                //If it was already generated
                if (nodes.TryGetValue(adjacency, out NavNode node))
                {
                    //Add the adjacent node to this node's neighbors if possible
                    if (!current.neighbors.Contains(node))
                    {
                        current.neighbors.Add(node);
                    }
                    //Add this node to the adjacent node's neighbors if possible
                    if (!node.neighbors.Contains(current))
                    {
                        node.neighbors.Add(current);
                    }
                }
                else
                {
                    //If it was not generated already, create a new node here
                    NavNode next = new NavNode(adjacency);

                    //Add as mutual neighbors
                    current.neighbors.Add(next);
                    next.neighbors.Add(current);

                    //Add node to node dictionary
                    nodes.Add(adjacency, next);

                    //If node is on a traversable tile
                    if (Adjacencies.ContainsKey(next.location))
                    {
                        //Initiate A* by clearing all costs and parents
                        foreach (NavNode nodeToClear in nodes.Values)
                        {
                            nodeToClear.parent = null;
                            nodeToClear.cost = float.MaxValue;
                        }
                        //Generate starting variables for A*
                        NavNode start = next;
                        start.cost = 0;
                        Vector2Int goal = firstValid;
                        SimplePriorityQueue<NavNode, float> queuedNodes = new SimplePriorityQueue<NavNode, float>();
                        queuedNodes.EnqueueWithoutDuplicates(start, Vector3.Distance(TraversableTilemap.CellToWorld(new Vector3Int(start.location.x, start.location.y, 0)), transform.position));
                        bool found = false;

                        //Keep going until you either find it or run out of nodes, which should not be possible
                        while (!found && queuedNodes.Count > 0)
                        {
                            //Get the checking node
                            NavNode currentNode = queuedNodes.Dequeue();
                            //If it is the goal mark as found and end the loop
                            if (currentNode.location.x == goal.x && currentNode.location.y == goal.y)
                            {
                                found = true;
                                break;
                            }
                            //If it is not, check all adjacent nodes
                            foreach (NavNode adjacentNode in currentNode.neighbors)
                            {
                                //Get the cost based on distance
                                float cost = currentNode.cost + Vector3.Distance(TraversableTilemap.CellToWorld(new Vector3Int(adjacentNode.location.x, adjacentNode.location.y)), TraversableTilemap.CellToWorld(new Vector3Int(currentNode.location.x, currentNode.location.y)));
                                
                                //Override if cost is lower than current cost
                                if (cost < adjacentNode.cost)
                                {
                                    //Override cost
                                    adjacentNode.cost = cost;

                                    //Override parent
                                    adjacentNode.parent = currentNode;

                                    //Add to expansion queue
                                    queuedNodes.EnqueueWithoutDuplicates(adjacentNode, adjacentNode.cost + Vector3.Distance(TraversableTilemap.CellToWorld(new Vector3Int(adjacentNode.location.x, adjacentNode.location.y)), transform.position));
                                }
                            }
                        }
                        //Returns the node that will create a path if it exists
                        if(nodes.TryGetValue(firstValid, out NavNode finalNode))
                        {
                            return finalNode;
                        }
                        //If it does not return null
                        return null;
                    }
                    //If it is not a traversable tile, add it as a node to expand from
                    else
                    {
                        nodesToExpandFrom.Enqueue(next);
                    }
                }
            }
        }
        //Return null if nothing can be found
        return null;
    }

    //Gets all adjacent tiles, regardless of traversability
    private Vector2Int[] getAllAdjacent(Vector2Int location)
    {
        //Storage array since guaranteed to be 6 due to hexagons
        Vector2Int[] adjacents = new Vector2Int[6];

        //Gets the world position of the tile
        Vector2 world = TraversableTilemap.CellToWorld(new Vector3Int(location.x, location.y, 0));

        //Creates a direction vector to use t detect adjacencies
        Vector2 adjacancyDetector = new Vector2(1, 0);

        //Rotate 5 times for the 6 adjacencies since you start with 1
        for (int i = 0; i < 6; i++)
        {
            //Get the cell position from the world position and the added direction
            adjacents[i] = (Vector2Int)TraversableTilemap.WorldToCell(world + adjacancyDetector);

            //Rotate the direction 60 degrees to get to the next tile
            adjacancyDetector = Quaternion.AngleAxis(60, new Vector3(0, 0, 1)) * adjacancyDetector;
        }
        //Return the found adjacencies
        return adjacents;
    }

    //Overloaded tile generation
    public void Generate(Vector2Int location)
    {
        //Calls the specific version
        Generate(location.x, location.y);
    }

    //Generates a tile at the given coordinates
    public void Generate(int x, int y)
    {
        //Gets the traversability rating of the tile
        float tileValue = Check(x, y);
        //If it is not sufficiently traversable and it has not been subbed
        if (tileValue < traversableCutoff && !subbedTiles.Contains(new Vector2Int(x, y)))
        {
            //Check the resource status
            if (CheckResource(x, y) >= resourceCutoff)
            {
                //If it is resource-rich, place blocker resource tile
                BlockerTilemap.SetTile(new Vector3Int(x, y, 0), blockerResourceTile);
            }
            else
            {
                //If it is not, place standard blocker tile
                BlockerTilemap.SetTile(new Vector3Int(x, y, 0), blockerTile);
            }
            //Either way it is a blocker, so add it to the blockers
            BlockerTiles.Add(new Vector2Int(x, y));
        }
        else
        {
            //Check the resource status
            if (CheckResource(x, y) >= resourceCutoff)
            {
                //If it is resource-rich, place traversable resource tile
                TraversableTilemap.SetTile(new Vector3Int(x, y, 0), traversableResourceTile);
            }
            else
            {
                //If it is not, place standard traversable tile
                TraversableTilemap.SetTile(new Vector3Int(x, y, 0), traversableTile);
            }
            //Either way it is traversable
            //Get world position of generated tile
            Vector2 world = BlockerTilemap.CellToWorld(new Vector3Int(x, y, 0));

            //Direction vector to find adjacencies
            Vector2 adjacancyDetector = new Vector2(1, 0);

            //New navnode representing the generated tile
            NavNode addedNode = new NavNode(new Vector2Int(x, y));

            //Go through each potential adjacency
            for (int i = 0; i < 6; i++)
            {
                //Gets tilemap-based location of adjacent tile
                Vector2Int currentCheck = (Vector2Int)BlockerTilemap.WorldToCell(world + adjacancyDetector);

                //If the adjacent tile is a generated traversable tile, add as mutual neighbors
                if(Adjacencies.TryGetValue(currentCheck, out NavNode adjacentNode))
                {
                    addedNode.neighbors.Add(adjacentNode);
                    adjacentNode.neighbors.Add(addedNode);
                }
                //Rotate adjacency detector to check next one
                adjacancyDetector = Quaternion.AngleAxis(60, new Vector3(0, 0, 1)) * adjacancyDetector;
            }
            //Add new node to adjacency storage
            Adjacencies.TryAdd(new Vector2Int(x, y), addedNode);
        }

    }

    //Get the traversability rating of a tile
    public float Check(int x, int y)
    {
        //Manipulate x and y to be based off of scaling and offset
        float actualx = (x + xOffset) / (float)int.MaxValue * mapScaling;
        float actualy = (y + yOffset) / (float)int.MaxValue * mapScaling;
        
        //Get the perlin noise map height at the given location using the traversability seeds
        return PerlinGenerator.Noise(actualx, actualy, seedA, seedB, seedC);
    }

    //Overloaded Check() variant using a Vector2Int
    public float Check(Vector2Int coords)
    {
        //Calls the specific version
        return Check(coords.x, coords.y);
    }

    //Get the Resource density of a tile
    public float CheckResource(int x, int y)
    {
        //Manipulate x and y to be based off of scaling and offset
        float actualx = (x + xOffset) / (float)int.MaxValue * mapScaling * resourceScaling;
        float actualy = (y + yOffset) / (float)int.MaxValue * mapScaling * resourceScaling;

        //Get the perlin noise map height at the given location using the resource seeds
        return PerlinGenerator.Noise(actualx, actualy, seedD, seedE, seedF);
    }

    //Overloaded CheckResource() variant using a Vector2Int
    public float CheckResource(Vector2Int coords)
    {
        //Calls the specific version
        return CheckResource(coords.x, coords.y);
    }

    public void Deactivate()
    {
        Destroy(BlockerTilemap.transform.parent.gameObject);
    }

    public Dictionary<Vector2Int, NavNode> copyAdjacencies()
    {
        Dictionary<Vector2Int, NavNode> result = new Dictionary<Vector2Int, NavNode>();
        foreach(NavNode oldNode in Adjacencies.Values)
        {
            result.Add(oldNode.location, new NavNode(oldNode.location, oldNode.position));
        }
        foreach (Vector2Int at in Adjacencies.Keys)
        {
            foreach (NavNode node in Adjacencies[at].neighbors)
            {
                result[at].neighbors.Add(result[node.location]);
            }
        }
        return result;
    }
}
