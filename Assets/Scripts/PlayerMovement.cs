using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3f;

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(x, 0, z);

        if (dir.sqrMagnitude > 0.001f)
        {
            transform.Translate(dir.normalized * moveSpeed * Time.deltaTime, Space.World);
        }
    }
}
