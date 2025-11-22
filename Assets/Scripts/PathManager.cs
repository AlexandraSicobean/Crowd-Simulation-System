using UnityEngine;

public class PathManager : MonoBehaviour
{
    public Vector3 goal;
    public float worldSize = 10f;
    public float reachThreshold = 0.5f;

    void Start()
    {
        AssignNewGoal();
    }

    public void AssignNewGoal()
    {
        goal = new Vector3(
            Random.Range(-worldSize, worldSize),
            transform.position.y,
            Random.Range(-worldSize, worldSize)
        );
    }

    public bool GoalReached()
    {
        Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
        Vector2 goal2D = new Vector2(goal.x, goal.z);

        return Vector2.Distance(pos2D, goal2D) < reachThreshold;
    }
}
