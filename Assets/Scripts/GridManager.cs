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

    private Dictionary<GridCell, GameObject> cellToCube;


    void Start()
    {
        grid = new Grid(); 
        grid.Initialize(width, height);

        Draw();
        //TestAStar();
        TestBidirectionalAStar();
    }

    void Draw()
    {
        cellToCube = new Dictionary<GridCell, GameObject>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridCell cell = grid.cells[x, y];

                Vector3 pos = new Vector3(x, 0, y);
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

    public void DrawPath(List<GridCell> path)
    {
        // Reset all colors
        foreach (var entry in cellToCube)
        {
            GridCell cell = entry.Key;
            GameObject cube = entry.Value;

            cube.GetComponent<Renderer>().material.color =
                cell.isObstacle ? Color.black : Color.white;
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


}
