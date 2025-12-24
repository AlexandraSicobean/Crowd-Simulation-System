using UnityEngine;
using System.Collections.Generic;

public class Simulator : MonoBehaviour
{
    public static Simulator _instance = null;
    public bool useSteering = false;

    public static Simulator GetInstance()
    {
        if (_instance == null)
        {
            _instance = FindFirstObjectByType<Simulator>();

            if (_instance == null)
            {
                GameObject simObj = new GameObject("Simulator");
                _instance = simObj.AddComponent<Simulator>();
            }
        }
        return _instance;
    }


    public enum SimulationMode
    {
        CollisionAvoidance,     // Lab 2
        PathFollowing,          // Lab 3
        Steering                // Lab 4
    }

    public SimulationMode mode = SimulationMode.CollisionAvoidance;
    private List<Agent> agents = new List<Agent>();

    public void AddAgent(Agent a)
    {
        agents.Add(a);
    }

    public void RemoveAgent(Agent a)
    {
        agents.Remove(a);
    }

    void Update()
    {
        float dt = Time.deltaTime;

        foreach (Agent a in agents)
        {
            if (a == null) continue;

            switch (mode)
            {
                case SimulationMode.CollisionAvoidance:
                    UpdateCollisionAvoidance(a, dt);
                    break;
                case SimulationMode.PathFollowing:
                    UpdatePathFollowing(a, dt); 
                    break;
                case SimulationMode.Steering:
                    UpdateSteering(a, dt); 
                    break;
            }
        }
    }

    void UpdateCollisionAvoidance(Agent a, float dt)
    {
        Vector3 desiredDir = a.goal - a.transform.position;
        desiredDir.y = 0f;

        if (desiredDir.magnitude  < a.goalThreshold)
        {
            a.AssignNewRandomGoal();
        }

        Vector3 desiredVel = desiredDir.normalized * a.speed;
        a.velocity = desiredVel;

        Rigidbody rb = a.GetComponent<Rigidbody>();
        if (rb != null)
            rb.MovePosition(rb.position + a.velocity * dt);
        else
            a.transform.position += a.velocity * dt;
    }

    void UpdatePathFollowing(Agent a, float dt)
    {
        Vector3 target = a.GetCurrentWaypointWorld();
        Vector3 dir = target - a.transform.position;
        dir.y = 0f;

        if (dir.magnitude < a.waypointThreshold)
            return;

        a.velocity = dir.normalized * a.speed;
        a.transform.position += a.velocity * dt;
    }

    public float arriveWeight = 1.0f;
    public float agentAvoidanceWeight = 1.0f;
    public float obstacleWeight = 3.0f;
    void UpdateSteering(Agent a, float dt)
    {
        Vector3 desiredDir = a.GetDirection();

        Vector3 force = Vector3.zero;
        Vector3 target = a.GetCurrentWaypointWorld();
        force += obstacleWeight * AvoidObstacles(a);
        force += agentAvoidanceWeight * AvoidAgents(a);
        force += arriveWeight * Arrive(a, target);

        if (force.magnitude > a.maxForce)
            force = force.normalized * a.maxForce;
        a.velocity += force * dt;

        a.transform.position += a.velocity * dt;
        a.UpdateCellOccupation();
    }


    public Vector3 Seek(Agent a, Vector3 desiredDir)
    {
        Vector3 desiredVelocity = desiredDir * a.speed;
        return desiredVelocity - a.velocity;
    }

    public Vector3 Arrive(Agent a, Vector3 target)
    {
        Vector3 dir = target - a.transform.position;
        dir.y = 0f;

        float d = dir.magnitude;
        if (d < 0.001f)
            return Vector3.zero;

        float slowingDistance = 1f;

        float rampedSpeed = a.speed * (d / slowingDistance);
        float clippedSpeed = Mathf.Min(rampedSpeed, a.speed);

        Vector3 desiredVelocity = dir * (clippedSpeed / d);

        return desiredVelocity - a.velocity;
    }


    public Vector3 AvoidObstacles(Agent a)
    {
        float lookAhead = Mathf.Lerp(1f, 3f, a.velocity.magnitude / a.speed);
        float corridorWidth = 0.6f;

        GridManager gridManager = a.gridManager;
        GridCell center = gridManager.GetClosestCell(a.transform.position);

        GridCell obstacle = null;
        float dist = float.MaxValue;

        for (int dx = -3; dx <= 3; dx++)
        {
            for (int dy = -3; dy <= 3; dy++)
            {
                int x = center.gridPos.x + dx;
                int y = center.gridPos.y + dy;

                if (x < 0 || y < 0 || x >= gridManager.width || y >= gridManager.height)
                    continue;

                GridCell cell = gridManager.grid.cells[x, y];
                if (!cell.isObstacle) continue;

                Vector3 worldPos = cell.GetWorldPosition(gridManager.gridOrigin);
                Vector3 localPos = a.transform.InverseTransformPoint(worldPos);

                if (localPos.z < 0) continue;
                if (localPos.z > lookAhead) continue;
                if (Mathf.Abs(localPos.x) > corridorWidth) continue;

                if (localPos.z < dist)
                {
                    dist = localPos.z;
                    obstacle = cell;
                }
            }
        }

        if (obstacle == null) return Vector3.zero;

        Vector3 offset = obstacle.GetWorldPosition(gridManager.gridOrigin) - a.transform.position;

        return - new Vector3(offset.x, 0, 0).normalized * a.speed;
    }

    public Vector3 AvoidAgents(Agent a)
    {
        float predictionHorizon = 1.0f;
        Agent closestAgent = null;
        float closestTime = float.MaxValue;

        foreach (Agent b in agents)
        {
            if (a == b) continue;

            Vector3 relPos = b.transform.position - a.transform.position;
            Vector3 relVel = b.velocity - a.velocity;

            if (relVel.sqrMagnitude < 0.0001f) continue;
            
            float closestT = -Vector3.Dot(relVel, relPos) / relVel.sqrMagnitude;
            if (closestT < 0 || closestT > predictionHorizon) continue;

            Vector3 nextPosA = a.transform.position + a.velocity * closestT;
            Vector3 nextPosB = b.transform.position + b.velocity * closestT;
            float dist = (nextPosA - nextPosB).magnitude;

            if (dist < 1.0f && closestT < closestTime)
            {
                closestTime = closestT;
                closestAgent = b;
            }

        }

        if (closestAgent == null) return Vector3.zero;

        Vector3 offset = closestAgent.transform.position - a.transform.position;
        return Vector3.Cross(new Vector3(offset.x, 0, offset.z),Vector3.up).normalized * a.speed;
    }

}
