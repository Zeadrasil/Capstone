using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyCheckpoint : MonoBehaviour
{
    public EnemyCheckpoint next;
    public static uint runningCount = 0;
    public uint id;
    public EnemyCheckpoint previous;
    public static Dictionary<(Vector3, Quaternion), EnemyCheckpoint> positions = new Dictionary<(Vector3, Quaternion), EnemyCheckpoint>();
    // Start is called before the first frame update
    void Start()
    {
        if (positions.TryGetValue((transform.position, transform.rotation), out EnemyCheckpoint newNext))
        {
            if (previous != null)
            {
                previous.next = newNext;
                Destroy(gameObject);
            }
            else
            {
                id = runningCount;
                runningCount++;
            }
        }
        else
        {
            positions.Add((transform.position, transform.rotation), this);
            id = runningCount;
            runningCount++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
