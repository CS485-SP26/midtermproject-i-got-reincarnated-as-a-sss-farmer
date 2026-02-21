using UnityEngine;

public class OrbitAroundPoint : MonoBehaviour
{
    // Assign the target object in the Inspector to use its position as the pivot
    public GameObject target; 
    public float rotationSpeed = 50f;

    void Update()
    {
        // Rotate the object around the target's position, using the Vector3.up axis
        // with a speed scaled by Time.deltaTime
        transform.RotateAround(target.transform.position, Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
