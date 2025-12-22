using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathFinding;

public class Agent : MonoBehaviour
{
    public Vector3 goal;
    public float speed = 0.5f;

    public GridManager gridManager;
    public Grid grid;

    public List<GridCell> path = new List<GridCell>();
    public int pathIndex = 0;
    public float waypointThreshold = 0.2f;

    int stuckCounter = 0;
    float lastMoveTime = 0f;

    private GridCell currentCell = null;
    private GridCell previousCell = null;

    public Vector3 velocity = Vector3.zero;

    void Start()
    {
        if (gridManager != null)
            grid = gridManager.grid;

        RequestNewPath();
    }

    public void RequestNewPath()
    {
        if (grid == null)
        {
            Debug.LogWarning("Agent has no grid reference!");
            return;
        }

        for (int attempt = 0; attempt < 10; attempt++)
        {
            GridCell startNode = gridManager.GetClosestFreeCell(transform.position);
            GridCell endNode = gridManager.GetRandomFreeCell();

            GridHeuristic heuristic = new GridHeuristic(endNode);
            //Bidirectional_A_Star astar = new Bidirectional_A_Star(500000, 2f, 500);
            Grid_A_Star astar = new Grid_A_Star(500000, 2f, 500);


            int found = -1;
            path = astar.findpath(grid, startNode, endNode, heuristic, ref found);

            if (found != 1 || path == null || path.Count < 2)
                continue; // try again
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

    // [DEBUG]
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
