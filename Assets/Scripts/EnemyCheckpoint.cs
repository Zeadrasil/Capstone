using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyCheckpoint : MonoBehaviour
{
    public EnemyCheckpoint next;
    public Vector2 direction;
    public static uint runningCount = 0;
    public uint id;
    // Start is called before the first frame update
    void Start()
    {
        id = runningCount;
        runningCount++;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
