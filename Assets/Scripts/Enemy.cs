using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Priority_Queue;
using Unity.VisualScripting;
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
    [SerializeField] private float radius;
    private Vector3 offset = Vector3.zero;
    private float offsetDistance = 0;

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = currentGuide.transform.rotation;
        baseHealth *= GameManager.Instance.enemyDifficulty;
        health = baseHealth;
        damage *= GameManager.Instance.enemyDifficulty;
        movementSpeed *= GameManager.Instance.enemyDifficulty;
        firerate *= GameManager.Instance.enemyDifficulty;
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
                if (offsetDistance > 0)
                {
                    transform.position += movementSpeed * Time.deltaTime * offset;
                    offsetDistance -= movementSpeed * Time.deltaTime;
                }
                else
                {
                    transform.position += movementSpeed * Time.deltaTime * transform.right.normalized;
                }
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
        Tilemap tilemap = TileManager.Instance.TraversableTilemap;
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
            return enemyCheckpoint;
        }
        return null;
    }

    public void activatePath(EnemyCheckpoint checkpoint)
    {
        currentGuide = checkpoint;
    }

    
    private void FixedUpdate()
    {
        if (!blocked)
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, radius, transform.right, movementSpeed * 0.06f, LayerMask.GetMask(new string[] { "EnemyBlockers" }));
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
                        else if(tags.Tags.Contains("Ground"))
                        {
                            //offsetDistance = 1;
                            //if(Vector2.Distance(transform.position + Quaternion.AngleAxis(30, new Vector3(0, 0, 1)) * transform.right.normalized, currentGuide.transform.position) > Vector2.Distance(transform.position + Quaternion.AngleAxis(-30, new Vector3(0, 0, 1)) * transform.right.normalized, currentGuide.transform.position))
                            //{
                            //    offset = Quaternion.AngleAxis(-30, new Vector3(0, 0, 1)) * transform.right.normalized;
                            //}
                            //else
                            //{
                            //    offset = Quaternion.AngleAxis(30, new Vector3(0, 0, 1)) * transform.right.normalized;
                            //}
                            //RaycastHit2D[] sideHits = Physics2D.CircleCastAll(transform.position, radius, Quaternion.AngleAxis(-30, new Vector3(0, 0, 1)) * transform.right, radius, LayerMask.GetMask(new string[] { "EnemyBlockers" }));
                            //bool wallBlocking = false;
                            //foreach (RaycastHit2D sideHit in sideHits)
                            //{
                            //    wallBlocking = hit.collider != null && hit.collider.TryGetComponent(out MultiTag sideTags) && sideTags.Tags.Contains("Ground");
                            //    if(wallBlocking)
                            //    {
                            //        break;
                            //    }
                            //}
                            //if (wallBlocking)
                            //{
                            //    if (!(Vector2.Distance(transform.position + Quaternion.AngleAxis(30, new Vector3(0, 0, 1)) * transform.right.normalized, currentGuide.transform.position) > Vector2.Distance(transform.position + Quaternion.AngleAxis(-30, new Vector3(0, 0, 1)) * transform.right.normalized, currentGuide.transform.position)))
                            //    {
                            //        offset = Quaternion.AngleAxis(-30, new Vector3(0, 0, 1)) * transform.right.normalized;
                            //    }
                            //    else
                            //    {
                            //        offset = Quaternion.AngleAxis(30, new Vector3(0, 0, 1)) * transform.right.normalized;
                            //    }
                            //    sideHits = Physics2D.CircleCastAll(transform.position, radius, Quaternion.AngleAxis(-30, new Vector3(0, 0, 1)) * transform.right, radius, LayerMask.GetMask(new string[] { "EnemyBlockers" }));
                            //    wallBlocking = false;
                            //    foreach (RaycastHit2D sideHit in sideHits)
                            //    {
                            //        wallBlocking = hit.collider != null && hit.collider.TryGetComponent(out MultiTag sideTags) && sideTags.Tags.Contains("Ground");
                            //        if (wallBlocking)
                            //        {
                            //            break;
                            //        }
                            //    }
                            //    if (wallBlocking)
                            //    {
                            //        offsetDistance = 0;
                            //        currentGuide = GeneratePath();
                            //    }
                            //}
                            //if(Vector3.Distance(hit.collider.transform.position, transform.position) < radius * 1.5f)
                            //{
                                currentGuide = GeneratePath();
                                transform.rotation = currentGuide.transform.rotation;
                            //}
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
            GameManager.Instance.budget += baseHealth * GameManager.Instance.playerIncome;
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
