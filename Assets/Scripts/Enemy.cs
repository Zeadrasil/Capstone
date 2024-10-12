using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Priority_Queue;
public class Enemy : MonoBehaviour, IDamageable
{
    private float health = 10;
    private float baseHealth = 10;
    private float movementSpeed = 1;
    private float damage = 2;
    private bool attackMode = false;
    private bool pitbullMode = false;
    private bool ranged = false;
    private bool blocked = false;
    private float firerate = 10;
    public EnemyCheckpoint currentGuide;
    private IDamageable target;
    private RangedEnemyTargeter targeter;
    private bool active = false;
    [SerializeField] private float radius;

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = currentGuide.transform.rotation;
    }

    void Update()
    {
        if(target == null && attackMode)
        {
            StopAllCoroutines();
            attackMode = false;
            blocked = false;
        }
        if (!attackMode || (ranged && !blocked))
        {
            if (!pitbullMode)
            {
                transform.position += movementSpeed * Time.deltaTime * transform.right.normalized;
            }
            else
            {
                transform.position += (Singleton<GameManager>.Instance.PlayerBase.transform.position - transform.position).normalized * movementSpeed * Time.deltaTime;
            }
        }
    }

    IEnumerator fireLoop()
    {
        while (true)
        {
            if (target.takeDamage(damage) <= 0)
            {
                attackMode = false;
                blocked = false;
                StopAllCoroutines();
                if (ranged)
                {
                    targeter.FindTarget();
                }
            }
            yield return new WaitForSeconds(10 / firerate);
        }

    }

    public EnemyCheckpoint GeneratePath()
    {
        Tilemap tilemap = TileManager.Instance.Tilemap;
        Dictionary<Vector2Int, NavNode> tileAdjacencies = TileManager.Instance.Adjacencies;
        foreach(NavNode nodeToClear in tileAdjacencies.Values )
        {
            nodeToClear.parent = null;
            nodeToClear.cost = float.MaxValue;
        }
        float avoidanceModifier = 0.9f * movementSpeed / (damage * firerate * 0.1f);
        tileAdjacencies.TryGetValue(new Vector2Int(), out NavNode start);
        start.cost = 0;
        Vector2Int goal = (Vector2Int)tilemap.WorldToCell(transform.position);
        SimplePriorityQueue<NavNode, float> nodes = new SimplePriorityQueue<NavNode, float>();
        nodes.EnqueueWithoutDuplicates(start, Vector3.Distance(tilemap.CellToWorld(new Vector3Int(start.location.x, start.location.y, 0)), transform.position));
        bool found = false;
        while(!found && nodes.Count > 0)
        {
            NavNode node = nodes.Dequeue();
            if (node.location.x == goal.x && node.location.y == goal.y)
            {
                found = true;
                break;
            }
            foreach (NavNode adjacentNode in node.neighbors)
            {
                float health = 0;
                if(GameManager.Instance.playerBuildings.TryGetValue(adjacentNode.location, out GameObject holder))
                {
                    health = holder.GetComponent<IDamageable>().getHealth();
                }
                float cost = node.cost + Vector3.Distance(tilemap.CellToWorld(new Vector3Int(adjacentNode.location.x, adjacentNode.location.y)), tilemap.CellToWorld(new Vector3Int(node.location.x, node.location.y))) + health * avoidanceModifier;
                if(cost < adjacentNode.cost)
                {
                    adjacentNode.cost = cost;
                    adjacentNode.parent = node;
                    nodes.EnqueueWithoutDuplicates(adjacentNode, adjacentNode.cost + Vector3.Distance(tilemap.CellToWorld(new Vector3Int(adjacentNode.location.x, adjacentNode.location.y)), transform.position));
                }
            }
        }
        if(found)
        {
            TileManager.TileDirection currentDirection;
            TileManager.TileDirection nextDirection;
            tileAdjacencies.TryGetValue(goal, out NavNode currentNode);
            nextDirection = TileManager.FromRelativePosition(tilemap.CellToWorld(new Vector3Int(currentNode.parent.location.x, currentNode.parent.location.y)) - transform.position);
            EnemyCheckpoint enemyCheckpoint = null;
            EnemyCheckpoint prevEnemyCheckpoint = null;
            currentNode = currentNode.parent;
            while (currentNode.parent != null)
            {
                currentDirection = nextDirection;
                nextDirection = TileManager.FromRelativePosition(tilemap.CellToWorld(new Vector3Int(currentNode.parent.location.x, currentNode.parent.location.y)) - tilemap.CellToWorld(new Vector3Int(currentNode.location.x, currentNode.location.y)));
                if (currentDirection != nextDirection)
                {
                    if(Instantiate(GameManager.Instance.EnemyCheckpointPrefab, tilemap.CellToWorld(new Vector3Int(currentNode.parent.location.x, currentNode.parent.location.y)), TileManager.RotateToDirection(currentDirection)).TryGetComponent(out EnemyCheckpoint checkpoint))
                    {
                        GameManager.Instance.checkpoints.Add(checkpoint.gameObject);
                        if(enemyCheckpoint == null)
                        {
                            enemyCheckpoint = checkpoint;
                            prevEnemyCheckpoint = checkpoint;
                        }
                        else
                        {
                            prevEnemyCheckpoint.next = checkpoint;
                            prevEnemyCheckpoint.next.previous = prevEnemyCheckpoint;
                            prevEnemyCheckpoint = prevEnemyCheckpoint.next;
                        }
                    }
                }
                currentNode = currentNode.parent;
            }
            if (enemyCheckpoint == null)
            {
                return null;
            }
            active = true;
            return enemyCheckpoint;
        }
        return null;
    }

    public void activatePath(EnemyCheckpoint checkpoint)
    {
        active = true;
        currentGuide = checkpoint;
    }

    
    private void FixedUpdate()
    {
        if (!blocked)
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, radius, transform.right, radius, LayerMask.GetMask(new string[] { "EnemyBlockers" }));
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.TryGetComponent(out MultiTag tags))
                    {
                        if (tags.Tags.Contains("PlayerBuilding"))
                        {
                            blocked = true;
                            attackMode = true;
                            target = hit.collider.gameObject.GetComponent<IDamageable>();
                            StartCoroutine(fireLoop());
                        }
                    }
                    else if (hit.collider.gameObject.TryGetComponent(out EnemyCheckpoint checkpoint))
                    {
                        if (currentGuide != null && checkpoint.id == currentGuide.id)
                        {
                            if (currentGuide.next != null)
                            {
                                currentGuide = currentGuide.next;
                                transform.rotation = currentGuide.transform.rotation;
                            }
                            else
                            {
                                pitbullMode = true;
                            }

                        }
                    }
                }
            }
        }
    }

    public void RangedTargetUpdate(IDamageable target)
    {
        if (ranged && !attackMode)
        {
            this.target = target;
            attackMode = true;
        }
    }
    public float takeDamage(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            GameManager.Instance.budget += baseHealth;
            GameManager.Instance.currentEnemies.Remove(this);
            GameManager.Instance.betweenWaves = GameManager.Instance.currentEnemies.Count == 0;
            Destroy(gameObject);
        }
        return health;
    }

    public float getHealth()
    {
        return health;
    }
}
