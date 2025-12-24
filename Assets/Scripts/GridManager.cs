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

    public enum PathAlgorithm
    {
        AStar,
        BidirectionalAStar
    }

    public PathAlgorithm algorithm = PathAlgorithm.AStar;

    void Start()
    {
        grid = new Grid(); 
        grid.Initialize(width, height);

        Draw();
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
                    cell.isObstacle ? Color.black : Color.gray;

                cellToCube[cell] = cube;
            }
        }
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

        if (!grid.cells[startX, startY].isObstacle)
            return grid.cells[startX, startY];

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

        return grid.cells[startX, startY];
    }
}
