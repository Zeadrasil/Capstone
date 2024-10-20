using System.Collections.Generic;
using UnityEngine;

//EnemyCheckpoint class is used for Enemy "pathfinding"
[RequireComponent(typeof(BoxCollider2D))]
public class EnemyCheckpoint : MonoBehaviour
{
    //EnemyCheckpoint data
    public EnemyCheckpoint next;
    public static uint runningCount = 0;
    public uint id;
    public EnemyCheckpoint previous;
    public static Dictionary<(Vector3, Quaternion), EnemyCheckpoint> positions = new Dictionary<(Vector3, Quaternion), EnemyCheckpoint>();
    
    void Start()
    {
        //Cull checkpoints that are duplicate in terms of having the same location and rotation
        if (positions.TryGetValue((transform.position, transform.rotation), out EnemyCheckpoint newNext))
        {
            //If it has a previous checkpoint, set the previous checkpoints next checkpoint to the first stored checkpoint
            if (previous != null)
            {
                previous.next = newNext;
                Destroy(gameObject);
            }
            //Do not cull if it is a guide that is already guiding an enemy
            else
            {
                id = runningCount;
                runningCount++;
            }
        }
        //If it is a new checkpoint, add it to the list of stored checkpoints and give it a unique ID
        else
        {
            positions.Add((transform.position, transform.rotation), this);
            id = runningCount;
            runningCount++;
        }
    }
}
