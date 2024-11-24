using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] Vector3 axis;
    [SerializeField] float rate;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation *= Quaternion.AngleAxis(rate * Time.deltaTime, axis);
    }
}
