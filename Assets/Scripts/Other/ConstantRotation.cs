using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    private Quaternion initialRotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = initialRotation;
    }
}
