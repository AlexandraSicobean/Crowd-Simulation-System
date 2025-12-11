using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathFinding;

public class GridManager : MonoBehaviour
{
    public GameObject cellPrefab;
    public Grid grid;
    public int width = 20;
    public int height = 20;
    public Vector3 gridOrigin = Vector3.zero;

    private Dictionary<GridCell, GameObject> cellToCube;

    void Start()
    {
        grid = new Grid(); 
        grid.Initialize(width, height);

        Draw();
        TestAStar();
        //TestBidirectionalAStar();
    }

    void Draw()
    {
        cellToCube = new Dictionary<GridCell, GameObject>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridCell cell = grid.cells[x, y];

                Vector3 pos = gridOrigin + new Vector3(x, 0, y);
                GameObject cube = Instantiate(cellPrefab, pos, Quaternion.identity);

                cube.GetComponent<Renderer>().material.color =
                    cell.isObstacle ? Color.black : Color.white;

                cellToCube[cell] = cube;
            }
        }
    }

    
    public void TestAStar()
    {
        Debug.Log("<color=yellow>Running A*</color>");

        GridCell start = grid.cells[0, 0];
        GridCell goal = grid.cells[width - 1, height - 1];

        GridHeuristic heuristic = new GridHeuristic(goal);

        Grid_A_Star astar = new Grid_A_Star(500000, 2f, 500);
        int found = -1;

        List<GridCell> path = astar.findpath(grid, start, goal, heuristic, ref found);

        if (found == 1)
        {
            Debug.Log($"START = {start.gridPos}, GOAL = {goal.gridPos}");
            Debug.Log($"<color=green>A* found path! Length = {path.Count}</color>");
            DrawPath(path);
        }
        else
        {
            Debug.Log("<color=red>No path found by A*</color>");
        }
    }
    /*
    public void TestBidirectionalAStar()
    {
        Debug.Log("<color=cyan>Running Bidirectional A*</color>");

        GridCell start = grid.cells[0, 0];
        GridCell goal = grid.cells[width - 1, height - 1];

        GridHeuristic heuristic = new GridHeuristic(goal);
        Bidirectional_A_Star biAStar = new Bidirectional_A_Star(500000, 2f, 500);

        int found = -1;
        List<GridCell> path = biAStar.findpath(grid, start, goal, heuristic, ref found);

        if (found == 1)
        {
            Debug.Log($"<color=green>Bidirectional A* found path! Length = {path.Count}</color>");
            Debug.Log($"START = {start.gridPos}, GOAL = {goal.gridPos}");
            DrawPath(path);
        }
        else
        {
            Debug.Log("<color=red>Bidirectional A* found NO path</color>");
        }
    }
    */
    public void DrawPath(List<GridCell> path)
    {
        // Reset all colors
        foreach (var entry in cellToCube)
        {
            GridCell cell = entry.Key;
            GameObject cube = entry.Value;

            cube.GetComponent<Renderer>().material.color =
                cell.isObstacle ? Color.black : Color.gray;
        }

        // Color path
        foreach (GridCell cell in path)
        {
            GameObject cube = cellToCube[cell];
            cube.GetComponent<Renderer>().material.color = Color.blue;
        }

        // Highlight start and goal
        cellToCube[path[0]].GetComponent<Renderer>().material.color = Color.green;
        cellToCube[path[path.Count - 1]].GetComponent<Renderer>().material.color = Color.red;
    }
    

    public GridCell GetClosestCell(Vector3 worldPos)
    {
        Vector3 local = worldPos - gridOrigin;

        int x = Mathf.Clamp(Mathf.RoundToInt(local.x), 0, width - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt(local.z), 0, height - 1);

        return grid.cells[x, y];
    }

    public GridCell GetRandomFreeCell()
    {
        while (true)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            GridCell cell = grid.cells[x, y];
            if (!cell.isObstacle)
                return cell;
        }
    }

    public GridCell GetClosestFreeCell(Vector3 worldPos)
    {
        Vector3 local = worldPos - gridOrigin;

        int sx = Mathf.RoundToInt(local.x);
        int sy = Mathf.RoundToInt(local.z);

        int startX = Mathf.Clamp(sx, 0, width - 1);
        int startY = Mathf.Clamp(sy, 0, height - 1);

        // If already free:
        if (!grid.cells[startX, startY].isObstacle)
            return grid.cells[startX, startY];

        // Search outward until any free cell is found
        for (int r = 1; r < Mathf.Max(width, height); r++)
        {
            for (int dx = -r; dx <= r; dx++)
            {
                for (int dy = -r; dy <= r; dy++)
                {
                    int nx = startX + dx;
                    int ny = startY + dy;

                    if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                        continue;

                    if (!grid.cells[nx, ny].isObstacle)
                        return grid.cells[nx, ny];
                }
            }
        }

        // Absolute fallback
        return grid.cells[startX, startY];
    }
}
