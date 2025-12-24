using UnityEngine;

public class OrientationManager : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public bool fixedOrientation = false;

    private Tracker tracker;

    void Start()
    {
        tracker = GetComponent<Tracker>();
    }

    void Update()
    {
        // Fix orientation
        if (Input.GetKeyDown(KeyCode.F))
            fixedOrientation = !fixedOrientation;

        if (fixedOrientation)
            return; 

        Vector3 velocity = tracker.worldVelocity;

        if (velocity.sqrMagnitude > 0.001f)
        {
            velocity.y = 0;
            Quaternion targetRot = Quaternion.LookRotation(velocity);

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
