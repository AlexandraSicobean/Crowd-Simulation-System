using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathFinding;

public class Agent : MonoBehaviour
{
    public Vector3 goal;
    public float speed = 2f;
    public float goalThreshold = 0.5f;

    public Vector3 velocity = Vector3.zero;

    public GridManager gridManager;
    public Grid grid;

    public List<GridCell> path = new List<GridCell>();
    public int pathIndex = 0;
    public float waypointThreshold = 0.2f;

    public float maxForce = 12f;
    public float slowingDistance = 1.0f;

    private GridCell currentCell = null;

    void Start()
    {
        AssignNewRandomGoal();
        // Only used in Exercise 3&4
        if (gridManager != null)
        {
            grid = gridManager.grid;
            RequestNewPath();
        }
    }

    public void AssignNewRandomGoal()
    {
        float range = 10f;

        goal = new Vector3(
            Random.Range(-range, range),
            transform.position.y,
            Random.Range(-range, range)
        );
    }

    public void RequestNewPath()
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            GridCell startNode = gridManager.GetClosestFreeCell(transform.position);
            GridCell endNode = gridManager.GetRandomFreeCell();
            GridHeuristic heuristic = new GridHeuristic(endNode);
            int found = -1;

            if (gridManager.algorithm == GridManager.PathAlgorithm.AStar)
            {
                Grid_A_Star astar = new Grid_A_Star(500000, 2f, 500);
                path = astar.findpath(grid, startNode, endNode, heuristic, ref found);
            }
            else
            {
                Bidirectional_A_Star biAStar = new Bidirectional_A_Star(500000, 2f, 500);
                path = biAStar.findpath(grid, startNode, endNode, heuristic, ref found);
            }

            if (found != 1 || path == null || path.Count < 2)
                continue;
        }

        pathIndex = 0;
    }

    public Vector3 GetDirection()
    {
        if (path == null || path.Count == 0 || pathIndex >= path.Count)
            return Vector3.zero;

        Vector3 target = path[pathIndex].GetWorldPosition(gridManager.gridOrigin);
        Vector3 toTarget = target - transform.position;
        toTarget.y = 0f;

        if (toTarget.magnitude < waypointThreshold)
        {
            pathIndex++;

            if (pathIndex >= path.Count)
            {
                RequestNewPath();
                return Vector3.zero;
            }

            target = path[pathIndex].GetWorldPosition(gridManager.gridOrigin);
            toTarget = target - transform.position;
            toTarget.y = 0f;
        }

        DrawPathWorld(path, gridManager.gridOrigin);

        return toTarget.normalized;
    }


    public void UpdateCellOccupation()
    {
        GridCell newCell = gridManager.GetClosestCell(transform.position);

        if (newCell != currentCell)
        {
            if (currentCell != null)
                currentCell.agentCount--;

            newCell.agentCount++;
            currentCell = newCell;
        }
    }

    public Vector3 GetCurrentWaypointWorld()
    {
        if (path == null || path.Count == 0 || pathIndex >= path.Count)
            return transform.position;

        Vector3 target = path[pathIndex].GetWorldPosition(gridManager.gridOrigin);

        float switchRadius = 0.3f;

        if ((target - transform.position).magnitude < switchRadius)
        {
            pathIndex++;

            if (pathIndex >= path.Count)
            {
                RequestNewPath();
                return transform.position;
            }

            target = path[pathIndex].GetWorldPosition(gridManager.gridOrigin);
        }

        DrawPathWorld(path, gridManager.gridOrigin);
        return target;
    }

    void DrawPathWorld(List<GridCell> path, Vector3 origin) 
    { 
        if (path == null || path.Count < 2) return; 
        
        for (int i = 0; i < path.Count - 1; i++) 
        {
            Vector3 a = path[i].GetWorldPosition(origin); 
            Vector3 b = path[i + 1].GetWorldPosition(origin); 
            Debug.DrawLine(a, b, Color.cyan, 0f); 
        } 
    }
}
