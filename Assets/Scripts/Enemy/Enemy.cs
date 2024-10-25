using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Priority_Queue;

//Base Enemy class that all types of enemies will use
public class Enemy : MonoBehaviour, IDamageable, IDamager
{
    //Enemy data
    private float health = 10;
    [SerializeField] private float baseHealth = 10;
    [SerializeField] private float movementSpeed = 1;
    [SerializeField] private float damage = 2;
    private bool attackMode = false;
    private bool pitbullMode = false;
    private bool ranged = false;
    private bool blocked = false;
    [SerializeField] private float firerate = 10;
    public bool swarmer = false;
    public EnemyCheckpoint currentGuide;
    private IDamageable target;
    private RangedEnemyTargeter targeter;
    [SerializeField] private float radius;
    private Vector3 offset = Vector3.zero;
    private float offsetDistance = 0;
    private List<IDamager> currentDamagers = new List<IDamager>();
    [SerializeField] GameObject healthBar;

    // Start is called before the first frame update
    void Start()
    {
        //Set starting rotation
        transform.rotation = currentGuide.transform.rotation;

        //Apply diffculty scaling to enemy
        baseHealth *= GameManager.Instance.enemyDifficulty;
        health = baseHealth;
        damage *= GameManager.Instance.enemyDifficulty;
        movementSpeed *= GameManager.Instance.enemyDifficulty;
        firerate *= GameManager.Instance.enemyDifficulty;
    }

    void Update()
    {
        //Only move if you are not attacking, unless you are attacking with a ranged attack
        if (!attackMode || (ranged && !blocked))
        {
            //Pitbull mode means go straight to player base
            if (!pitbullMode)
            {
                //Move according to direction
                transform.parent.position += movementSpeed * Time.deltaTime * transform.right.normalized;
            }
            else
            {
                //Move straight to player base
                transform.parent.position += (Singleton<GameManager>.Instance.PlayerBase.transform.position - transform.position).normalized * movementSpeed * Time.deltaTime;
            }
        }
    }

    //Loop for attacking
    IEnumerator fireLoop()
    {
        while (true)
        {
            target.TakeDamage(damage);
            yield return new WaitForSeconds(10 / firerate);
        }

    }

    //Generates a path for the enemy to follow
    public EnemyCheckpoint GeneratePath()
    {
        //Ready navigation map
        Tilemap tilemap = TileManager.Instance.TraversableTilemap;
        Dictionary<Vector2Int, NavNode> tileAdjacencies = TileManager.Instance.Adjacencies;
        foreach(NavNode nodeToClear in tileAdjacencies.Values )
        {
            nodeToClear.parent = null;
            nodeToClear.cost = float.MaxValue;
        }

        //Avoidance modifier makes it so that enemies prioritize going around walls and the like
        float avoidanceModifier = 0.9f * movementSpeed / (damage * firerate * 0.1f);

        //Get starting locaation
        tileAdjacencies.TryGetValue(new Vector2Int(), out NavNode start);
        start.cost = 0;
        Vector2Int goal = (Vector2Int)tilemap.WorldToCell(transform.position);

        //Use priorityqueue to ensure that the first path found is the most efficient or tied for most efficient
        SimplePriorityQueue<NavNode, float> nodes = new SimplePriorityQueue<NavNode, float>();
        nodes.EnqueueWithoutDuplicates(start, Vector3.Distance(tilemap.CellToWorld(new Vector3Int(start.location.x, start.location.y, 0)), transform.position));
        bool found = false;

        //Loop until either a path is found or it confirms that there is no path
        while(!found && nodes.Count > 0)
        {
            //Gets the cheapest node and checks if it is the goal
            NavNode node = nodes.Dequeue();
            if (node.location.x == goal.x && node.location.y == goal.y)
            {
                found = true;
                break;
            }
            //Goes through each of the connected nodes
            foreach (NavNode adjacentNode in node.neighbors)
            {
                float health = 0;
                if(GameManager.Instance.playerBuildings.TryGetValue(adjacentNode.location, out GameObject holder))
                {
                    health = holder.GetComponentInChildren<IDamageable>().GetHealth();
                }
                //Gets cost of going to the node based off of both the distance and health of any buildings
                float cost = node.cost + Vector3.Distance(tilemap.CellToWorld(new Vector3Int(adjacentNode.location.x, adjacentNode.location.y)), tilemap.CellToWorld(new Vector3Int(node.location.x, node.location.y))) + health * avoidanceModifier;
                
                //If the adjacent node already has a cost, skip it unless this is a cheaper path
                if(cost < adjacentNode.cost)
                {
                    //Set cost and parent to new optimal path, and then add it to the queue
                    adjacentNode.cost = cost;
                    adjacentNode.parent = node;
                    nodes.EnqueueWithoutDuplicates(adjacentNode, adjacentNode.cost + Vector3.Distance(tilemap.CellToWorld(new Vector3Int(adjacentNode.location.x, adjacentNode.location.y)), transform.position));
                }
            }
        }
        //If there is a path found, generate a system of checkpoints for the enemy to follow
        if(found)
        {
            TileManager.TileDirection currentDirection;
            TileManager.TileDirection nextDirection;

            //Gets the pathing node that is at the player's location
            tileAdjacencies.TryGetValue(goal, out NavNode currentNode);

            //Gets the direction from the previous node to the final node
            nextDirection = TileManager.FromRelativePosition(tilemap.CellToWorld(new Vector3Int(currentNode.parent.location.x, currentNode.parent.location.y)) - transform.position);
            
            //Storage variables to hold checkpoints
            EnemyCheckpoint enemyCheckpoint = null;
            EnemyCheckpoint prevEnemyCheckpoint = null;
            
            //Get previous node to start backtracking to enemy location
            currentNode = currentNode.parent;
            
            //Keep going until you run out of backtracking nodes, meaning that you have reached the location of the enemy
            while (currentNode.parent != null)
            {
                //Progress direction trackers
                currentDirection = nextDirection;
                nextDirection = TileManager.FromRelativePosition(tilemap.CellToWorld(new Vector3Int(currentNode.parent.location.x, currentNode.parent.location.y)) - tilemap.CellToWorld(new Vector3Int(currentNode.location.x, currentNode.location.y)));
                
                //Skip placing a new checkpoint if direction does not change
                if (currentDirection != nextDirection)
                {
                    //Create new checkpoint
                    if(Instantiate(GameManager.Instance.EnemyCheckpointPrefab, tilemap.CellToWorld(new Vector3Int(currentNode.parent.location.x, currentNode.parent.location.y)), TileManager.RotateToDirection(currentDirection)).TryGetComponent(out EnemyCheckpoint checkpoint))
                    {
                        //Add the new checkpoint to tracker for culling later
                        GameManager.Instance.checkpoints.Add(checkpoint.gameObject);

                        //Check for first created checkpoint
                        if(enemyCheckpoint == null)
                        {
                            //Store checkpoint in order to link them together
                            enemyCheckpoint = checkpoint;
                            prevEnemyCheckpoint = checkpoint;
                        }
                        else
                        {
                            //Link checkpoints together and store them
                            prevEnemyCheckpoint.next = checkpoint;
                            prevEnemyCheckpoint.next.previous = prevEnemyCheckpoint;
                            prevEnemyCheckpoint = prevEnemyCheckpoint.next;
                        }
                    }
                }
                //Progress towards enemy
                currentNode = currentNode.parent;
            }
            //If it could not generate a path of checkpoints (typically if it is a straight shot to the enemy base) return null
            if (enemyCheckpoint == null)
            {
                return null;
            }
            //Return the first checkpoint that will start guiding enemies
            return enemyCheckpoint;
        }
        //If it could not find a path in the modified A*, return null
        return null;
    }

    //Set guide to a generated path
    public void activatePath(EnemyCheckpoint checkpoint)
    {
        currentGuide = checkpoint;
    }

    //Physics so that rigidbodies can be avoided in order to improve performance
    private void FixedUpdate()
    {
        //If movement is not confirmed to be blocked, do checks
        if (!blocked)
        {
            //Check in front of enemy and get all collisions shortly ahead
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, radius, transform.right, movementSpeed * 0.06f, LayerMask.GetMask(new string[] { "EnemyBlockers" }));
            
            //Go through each collision and figure out what to do with them
            foreach (RaycastHit2D hit in hits)
            {
                //Checks for Multitag component (allows multiple tags per object)
                if (hit.collider.gameObject.TryGetComponent(out MultiTag tags))
                {
                    //If it is a player building, start attacking it
                    if (tags.Tags.Contains("PlayerBuilding"))
                    {
                        blocked = true;
                        attackMode = true;
                        target = hit.collider.gameObject.GetComponent<IDamageable>();
                        target.AddDamager(this);
                        StartCoroutine(fireLoop());
                    }
                    //If it is a blocking terrain tile, generate a new path since this means that you are off of the path
                    else if(tags.Tags.Contains("Ground"))
                    {
                        currentGuide = GeneratePath();
                        if (currentGuide != null)
                        {
                            transform.rotation = currentGuide.transform.rotation;
                        }
                        transform.parent.position = TileManager.Instance.BlockerTilemap.CellToWorld(TileManager.Instance.BlockerTilemap.WorldToCell(transform.position));
                    }
                }
                //If it is an enemy checkpoint
                else if (hit.collider.gameObject.TryGetComponent(out EnemyCheckpoint checkpoint))
                {
                    //Check to see if you have a guiding checkpoint, and if so check to see if the checkpoint you just hit is the checkpoint that is currently guiding you
                    if (currentGuide != null && checkpoint.id == currentGuide.id)
                    {
                        //If your current guide has a next guide
                        if (currentGuide.next != null)
                        {
                            //Change guides to the next guide
                            currentGuide = currentGuide.next;
                            transform.rotation = currentGuide.transform.rotation;
                        }
                        else
                        {
                            //If there is no next guide go into pitbull mode
                            pitbullMode = true;
                        }
                    }
                }
            }
        }
    }

    //Register ranged attack target update, currently useless
    public void RangedTargetUpdate(IDamageable target)
    {
        if (ranged && !attackMode)
        {
            this.target = target;
            attackMode = true;
        }
    }
    //Take damage
    public float TakeDamage(float damage)
    {
        health -= damage;
        healthBar.transform.localScale = new Vector3(health / baseHealth, 0.1f, 1);
        healthBar.transform.localPosition = new Vector3((-1 + health / baseHealth) * 0.5f, -0.55f, 0);
        //Kill self if out of health
        if(health <= 0)
        {
            //Give budget based on max health
            GameManager.Instance.budget += baseHealth * GameManager.Instance.playerIncome;

            //Used to update wave progress
            GameManager.Instance.KillEnemy(this);

            //Remove self from list of attackers on your target if you have a target
            if (target != null)
            {
                target.RemoveDamager(this);
            }
            //Tell all people attacking you to stop since you are dead
            foreach(IDamager damager in currentDamagers)
            {
                damager.cancelAttack();
            }
            //Finish destroying self
            Destroy(transform.parent.gameObject);
        }
        //Send health back for use by caller
        return health;
    }

    //Send health to callers for them to use
    public float GetHealth()
    {
        return health;
    }

    //Stop attacking, should only happen when either you or your target dies
    public void cancelAttack()
    {
        StopAllCoroutines();
        attackMode = false;
        blocked = false;
        target = null;
    }

    //Add to the list of people attacking you
    public void AddDamager(IDamager damager)
    {
        currentDamagers.Add(damager);
    }

    //Remove from the list of people attacking you
    public void RemoveDamager(IDamager damager)
    {
        currentDamagers.Remove(damager);
    }

    //Heal self, currently useless for enemies due to no callers
    public void Heal(float healing)
    {
        health = Mathf.Min(healing + health, baseHealth);
        healthBar.transform.localScale = new Vector3(health / baseHealth, 0.1f, 1);
        healthBar.transform.localPosition = new Vector3((-1 + health / baseHealth) * 0.5f, -0.55f, 0);
    }
}
