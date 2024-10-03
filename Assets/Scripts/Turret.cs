using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Turret : MonoBehaviour, IDamageable
{
    float firerate = 3;
    float damage = 10;
    float health = 100;
    [SerializeField] float range = 100;
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

    public void takeDamage(float damage)
    {

    }

    IEnumerator fireLoop()
    {
        while (true)
        {
            Debug.Log("fire attempt");

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
        Debug.Log("no enemies found");
        
        return null;

    }
}
