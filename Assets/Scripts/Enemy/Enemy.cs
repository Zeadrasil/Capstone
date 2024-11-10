using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Priority_Queue;

//Base Enemy class that all types of enemies will use
public class Enemy : MonoBehaviour, IDamageable, IDamager
{
    //Enemy data
    protected float health = 50;
    [SerializeField] protected float baseHealth = 10;
    public float movementSpeed = 1;
    public float damage = 2;
    protected bool attackMode = false;
    protected bool pitbullMode = false;
    [SerializeField] bool ranged = false;
    protected bool blocked = false;
    public float firerate = 10;
    public bool swarmer = false;
    public EnemyCheckpoint currentGuide;
    protected PlayerBuilding target;
    [SerializeField] float range = 5;
    protected float rangedSearchCooldown = 0;
    protected float baseRangedSearchCooldown = 9;
    [SerializeField] protected float radius;
    protected Vector3 offset = Vector3.zero;
    protected float offsetDistance = 0;
    protected List<IDamager> currentDamagers = new List<IDamager>();
    [SerializeField] GameObject healthBar;
    Coroutine fireCoroutine;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        //If you have a guide, follow it
        if (currentGuide != null)
        {
            //Set starting rotation
            transform.rotation = currentGuide.transform.rotation;
        }
        //Otherwise go straight for the base
        else
        {
            pitbullMode = true;
        }

        //Apply diffculty scaling to enemy
        baseHealth *= GameManager.Instance.enemyStrength;
        health = baseHealth;
        damage *= GameManager.Instance.enemyStrength;
        movementSpeed *= GameManager.Instance.enemyStrength;
        firerate *= GameManager.Instance.enemyStrength;
    }


    //Loop for attacking
    IEnumerator fireLoop()
    {
        while (true)
        {
            if (!ranged || Vector3.Distance(transform.position, target.transform.position) <= range)
            {
                target.TakeDamage(damage);
            }
            else
            {
                cancelAttack();
                if(target != null)
                {
                    target.TakeDamage(damage);
                }
            }
            yield return new WaitForSeconds(10 / firerate);
        }

    }

    //Finds a path for the enemy
    public static (bool, Dictionary<Vector2Int, NavNode>) FindPath(Vector2Int goal, float movementSpeed, float damage, float firerate, Vector3 position)
    {
        Dictionary<Vector2Int, NavNode> tileAdjacencies = TileManager.Instance.copyAdjacencies();
        foreach (NavNode nodeToClear in tileAdjacencies.Values)
        {
            nodeToClear.parent = null;
            nodeToClear.cost = float.MaxValue;
        }

        //Avoidance modifier makes it so that enemies prioritize going around walls and the like
        float avoidanceModifier = 0.9f * movementSpeed / (damage * firerate * 0.1f);

        //Get starting locaation
        tileAdjacencies.TryGetValue(new Vector2Int(), out NavNode start);
        start.cost = 0;

        //Use priorityqueue to ensure that the first path found is the most efficient or tied for most efficient
        SimplePriorityQueue<NavNode, float> nodes = new SimplePriorityQueue<NavNode, float>();
        nodes.EnqueueWithoutDuplicates(start, Vector3.Distance(start.position, position));
        bool found = false;

        //Loop until either a path is found or it confirms that there is no path
        while (!found && nodes.Count > 0)
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
                //Try to avoid walls if possible
                float healthCost = 0;

                //Destroy turrets if it is worthwhile
                float damagePreventionModifier = 0;

                //Cost based on distance travelled
                float distanceCost = Vector3.Distance(adjacentNode.position, node.position);

                //Destroy repair stations if it is worthwhile
                float repairPreventionModifier = 0;

                //Destroy resource extractors if it is worthwhile
                float resourceDenialModifier = 0;
                if (GameManager.Instance.playerBuildings.TryGetValue(adjacentNode.location, out GameObject holder))
                {
                    //Update health
                    healthCost = GameManager.Instance.playerHealths.GetValueOrDefault(adjacentNode.location);

                    //Check if turret
                    if (GameManager.Instance.playerDamageData.TryGetValue(adjacentNode.location, out float baseDamageModifier))
                    {
                        //If turret apply desire to destroy to prevent damage
                        damagePreventionModifier = baseDamageModifier / (healthCost);
                    }

                    //Check if repair station
                    if (GameManager.Instance.playerRepairData.TryGetValue(adjacentNode.location, out float baseRepairModifier))
                    {
                        //If repair station apply desire to destroy to prevent it from repairing structures you want to destroy
                        repairPreventionModifier = baseRepairModifier * 10 / healthCost;
                    }

                    //Check if resource extractor
                    if (GameManager.Instance.playerExtractionData.TryGetValue(adjacentNode.location, out float baseExtractionModifier))
                    {
                        //If resource extractor apply desire to destroy to prevent the player from getting resources
                        resourceDenialModifier = baseExtractionModifier / healthCost;
                    }
                }

                //Combine together weights to form overall weight
                float cost = Mathf.Max(node.cost + healthCost + distanceCost - damagePreventionModifier - repairPreventionModifier - resourceDenialModifier, 0);

                //If the adjacent node already has a cost, skip it unless this is a cheaper path
                if (cost < adjacentNode.cost)
                {
                    //Ensures that it does not accidently cut off its own path
                    bool valid = true;
                    NavNode parentNode = node.parent;
                    while (parentNode != null)
                    {
                        if (parentNode.Equals(adjacentNode))
                        {
                            valid = false;
                            break;
                        }
                        parentNode = parentNode.parent;
                    }
                    //Set cost and parent to new optimal path, and then add it to the queue if it would not cut itself off
                    if (valid)
                    {
                        adjacentNode.cost = cost;
                        adjacentNode.parent = node;
                        nodes.EnqueueWithoutDuplicates(adjacentNode, adjacentNode.cost + Vector3.Distance(adjacentNode.position, position));
                    }
                }
            }
        }
        return (found, tileAdjacencies);
    }

    //Generates a path for the enemy to follow
    public EnemyCheckpoint GeneratePath(Dictionary<Vector2Int, NavNode> tileAdjacencies, Vector2Int goal)
    {
        //Storage variables
        TileManager.TileDirection currentDirection;
        TileManager.TileDirection nextDirection;

        //Gets the pathing node that is at the player's location
        tileAdjacencies.TryGetValue(goal, out NavNode currentNode);

        //Gets the direction from the previous node to the final node
        nextDirection = TileManager.FromRelativePosition(TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(currentNode.parent.location.x, currentNode.parent.location.y)) - transform.position);
            
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
            nextDirection = TileManager.FromRelativePosition(TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(currentNode.parent.location.x, currentNode.parent.location.y)) - TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(currentNode.location.x, currentNode.location.y)));
                
            //Skip placing a new checkpoint if direction does not change
            if (currentDirection != nextDirection)
            {
                //Create new checkpoint
                if(Instantiate(GameManager.Instance.EnemyCheckpointPrefab, TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(currentNode.parent.location.x, currentNode.parent.location.y)), TileManager.RotateToDirection(currentDirection)).TryGetComponent(out EnemyCheckpoint checkpoint))
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

    //Set guide to a generated path
    public void ActivatePath(EnemyCheckpoint checkpoint)
    {
        currentGuide = checkpoint;
        transform.rotation = checkpoint.transform.rotation;
    }

    //Physics so that rigidbodies can be avoided in order to improve performance
    protected virtual void FixedUpdate()
    {
        //Only move if you are not attacking, unless you are attacking with a ranged attack
        if (!attackMode || (ranged && !blocked))
        {
            //Pitbull mode means go straight to player base
            if (!pitbullMode)
            {
                //Move according to direction
                transform.parent.position += movementSpeed * 0.02f * transform.right.normalized;
            }
            else
            {
                //Move straight to player base
                transform.parent.position += (Singleton<GameManager>.Instance.PlayerBase.transform.position - transform.position).normalized * movementSpeed * 0.02f;
            }
        }
        //If you are ranged and you are not targeting somethign
        if(ranged && target == null)
        {
            //Cooldown to improve performance at the cost of reaction time
            if (rangedSearchCooldown == 0)
            {
                //Identify target
                target = FindRangedTarget();

                //If target found attack it
                if(target != null)
                {
                    target.AddDamager(this);
                    fireCoroutine = StartCoroutine(fireLoop());
                    attackMode = true;
                }
                //Reset the cooldown regardless of if it found anything
                rangedSearchCooldown = baseRangedSearchCooldown;
            }
            else
            {
                //Process cooldown
                rangedSearchCooldown--;
            }
        }
        //If movement is not confirmed to be blocked, do checks
        if (!blocked)
        {
            //Check in front of enemy and get all collisions shortly ahead
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, radius, transform.right, movementSpeed * 0.02f, LayerMask.GetMask(new string[] { "EnemyBlockers" }));
            
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
                        //If you are already attacking a different building due to ranged, stop attacking it
                        if (ranged && target != hit.collider.gameObject.GetComponent<PlayerBuilding>())
                        {
                            target.RemoveDamager(this);
                        }
                        //If you are not ranged or you are attacking a different building already, start attacking the building
                        if(!ranged || target != hit.collider.gameObject.GetComponent<PlayerBuilding>())
                        {
                            target = hit.collider.gameObject.GetComponent<PlayerBuilding>();
                            target.AddDamager(this);
                            if (fireCoroutine == null)
                            {
                                fireCoroutine = StartCoroutine(fireLoop());
                            }

                        }
                    }
                    //If it is a blocking terrain tile, generate a new path since this means that you are off of the path
                    else if(tags.Tags.Contains("Ground"))
                    {
                        Vector2Int goal = (Vector2Int)TileManager.Instance.TraversableTilemap.WorldToCell(transform.position);
                        (bool, Dictionary<Vector2Int, NavNode>) adjacencies = FindPath(goal, movementSpeed, damage, firerate, transform.position);
                        if (adjacencies.Item1)
                        {
                            currentGuide = GeneratePath(adjacencies.Item2, goal);
                        }
                        else
                        {
                            currentGuide = null;
                        }
                        //Follow the guide if it exists
                        if (currentGuide != null)
                        {
                            transform.rotation = currentGuide.transform.rotation;
                        }
                        //If it does not, go straight for the base
                        else
                        {
                            pitbullMode = true;
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
        //If you killed your enemy and are a ranged attacker
        if (health > 0 && ranged)
        {
            //Find new target
            target = FindRangedTarget();
        }
        else
        {
            //Mark target as nonexistent
            target = null;
        }
        //If a building died, guaranteed that you are no longer blocked since blocking buildings take damage priority
        blocked = false;

        //If no target
        if (target == null)
        {
            //Cancel everything
            StopCoroutine(fireCoroutine);
            fireCoroutine = null;
            attackMode = false;
        }
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

    //Heal self capped to max health, currently useless for enemies due to no callers
    public void Heal(float healing)
    {
        health = Mathf.Min(healing + health, baseHealth);
        healthBar.transform.localScale = new Vector3(health / baseHealth, 0.1f, 1);
        healthBar.transform.localPosition = new Vector3((-1 + health / baseHealth) * 0.5f, -0.55f, 0);
    }

    public PlayerBuilding FindRangedTarget()
    {
        //Get all colliders within range
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);

        //Get a priority queue to ensure that you fire at the closest enemy
        Utils.PriorityQueue<PlayerBuilding, float> targets = new Utils.PriorityQueue<PlayerBuilding, float>();

        //Go through every potential enemy
        foreach (Collider2D hit in hits)
        {
            //If it is an enemy, add it to the queue with the distance between the enemy and yourself as the priority
            if (hit.gameObject.TryGetComponent(out PlayerBuilding building))
            {
                targets.Enqueue(building, Mathf.Abs((hit.gameObject.transform.position - transform.position).magnitude));
            }
        }
        //If there is at least one potential target, return the closest target
        if (targets.Count > 0)
        {
            return targets.Dequeue();
        }
        //Otherwise return null
        return null;
    }

    //Apply the effects of an aura
    public void ApplyAura(int type, int level)
    {
        //What type of aura is it
        switch(type)
        {
            //Speed aura
            case 0:
                {
                    movementSpeed *= Mathf.Pow(1.1f, level);
                    break;
                }
                //Healing aura
            case 1:
                {
                    Heal(level * baseHealth * 0.025f);
                    break;
                }
                //Damage aura
            case 2:
                {
                    damage *= Mathf.Pow(1.1f, level);
                    break;
                }
                //Firerate aura
            case 3:
                {
                    firerate *= Mathf.Pow(1.1f, level);
                    break;
                }
                //Range aura
            case 4:
                {
                    range *= Mathf.Pow(1.1f, level);
                    break;
                }

        }
    }

    //Remove the effects of an aura
    public void RemoveAura(int type, int level)
    {
        //What type of aura is it
        switch(type)
        {
            //Speed aura
            case 0:
                {
                    movementSpeed /= Mathf.Pow(1.1f, level);
                    break;
                }
                //Healing aura, not a constant effect so do nothing
            case 1:
                {
                    break;
                }
                //Damage aura
            case 2:
                {
                    damage /= Mathf.Pow(1.1f, level);
                    break;
                }
                //Firerate aura
            case 3:
                {
                    firerate /= Mathf.Pow(1.1f, level);
                    break;
                }
                //Range aura
            case 4:
                {
                    range /= Mathf.Pow(1.1f, level);
                    break;
                }
        }
    }

    protected virtual int GetEnemyType()
    {
        return 0;
    }
}
