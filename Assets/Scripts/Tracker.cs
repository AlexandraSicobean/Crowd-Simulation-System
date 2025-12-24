using UnityEngine;

public class Tracker : MonoBehaviour
{
    public Vector3 worldVelocity;
    public Vector3 localVelocity;
    public float speed;

    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        // Compute displacement since last frame
        Vector3 displacement = transform.position - lastPosition;

        // Compute velocity in world and local space
        worldVelocity = displacement / Time.deltaTime;
        localVelocity = transform.InverseTransformDirection(worldVelocity);

        speed = worldVelocity.magnitude;

        lastPosition = transform.position;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // World velocity - green
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + worldVelocity);

        // Direction - blue
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.5f);
    }

}
