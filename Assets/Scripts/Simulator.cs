using UnityEngine;
using System.Collections.Generic;

public class Simulator : MonoBehaviour
{
    public static Simulator _instance = null;

    public static Simulator GetInstance()
    {
        if (_instance == null)
        {
            GameObject simObj = new GameObject("Simulator");
            _instance = simObj.AddComponent<Simulator>();
        }
        return _instance;
    }

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

            Rigidbody rb = a.GetComponent<Rigidbody>();

            Vector3 desiredDir = a.GetDirection();

            if (desiredDir == Vector3.zero) continue;

            Vector3 force = Vector3.zero;
            force += Seek(a, desiredDir) + 2.0f * AvoidObstacles(a) + 3.0f * AvoidAgents(a);

            float maxForce = 10f;
            if (force.magnitude > maxForce)
                force = force.normalized * maxForce;
            a.velocity += force * dt;

            if (a.velocity.magnitude > a.speed)
                a.velocity = a.velocity.normalized * a.speed;

            rb.MovePosition(rb.position + a.velocity * dt);
            a.UpdateCellOccupation();
        }
    }

    public Vector3 Seek(Agent a, Vector3 desiredDir)
    {
        Vector3 desiredVelocity = desiredDir * a.speed;
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
