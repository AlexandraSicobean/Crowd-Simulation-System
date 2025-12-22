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
        Vector3 displacement = transform.position - lastPosition;

        worldVelocity = displacement / Time.deltaTime;
        localVelocity = transform.InverseTransformDirection(worldVelocity);
        speed = worldVelocity.magnitude;

        lastPosition = transform.position;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + worldVelocity);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.5f);
    }

}
