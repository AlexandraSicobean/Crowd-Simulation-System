using UnityEngine;

public class LocomotionController : MonoBehaviour
{
    private Animator animator;
    private Tracker tracker;

    void Start()
    {
        animator = GetComponent<Animator>();
        tracker = GetComponent<Tracker>();

        animator.applyRootMotion = false;
    }

    void Update()
    {
        animator.SetFloat("VelX", tracker.localVelocity.x);
        animator.SetFloat("VelZ", tracker.localVelocity.z);
    }
}
