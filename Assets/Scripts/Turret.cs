using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Turret : MonoBehaviour, IDamageable
{
    float firerate = 3;
    float damage = 10;
    float health = 20;
    bool killed = false;
    [SerializeField] float range = 100;
    public Vector2Int location = Vector2Int.zero;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(fireLoop());
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public float takeDamage(float damage)
    {
        health -= damage;
        TileManager.Instance.PlayerBuildingHealths.Remove(location);
        if (health <= 0)
        {
            killed = true;
            GameManager.Instance.playerBuildings.Remove(location);
        }
        else
        {
            TileManager.Instance.PlayerBuildingHealths.Add(location, health);
        }
        return health;
    }

    public void LateUpdate()
    {
        if(killed)
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    IEnumerator fireLoop()
    {
        while (true)
        {

            Enemy target = findTargets();
            if (target != null)
            {
                target.takeDamage(damage);
            }
            yield return new WaitForSeconds(10 / firerate);
        }

    }

    private Enemy findTargets()
    {
        Collider2D[] hits =Physics2D.OverlapCircleAll(this.transform.position, range);
        Utils.PriorityQueue<Enemy, float> targets = new Utils.PriorityQueue<Enemy, float>();
        foreach (Collider2D hit in hits)
        {
            if(hit.gameObject.TryGetComponent<Enemy>(out Enemy enemy))
            {
                targets.Enqueue(enemy, Mathf.Abs((hit.gameObject.transform.position - transform.position).magnitude));
            }
        }
        if (targets.Count > 0)
        {
            return targets.Dequeue();
        }
        
        return null;

    }
}
