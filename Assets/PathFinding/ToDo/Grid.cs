using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using PathFinding;

public class Grid : FiniteGraph<GridCell, CellConnection, GridConnections>
{
    public int width;
	public int height;
	public float obstacleProbability = 0.2f;

	public GridCell[,] cells;

	public void Initialize(int width, int height)
	{
		this.width = width;
		this.height = height;

		cells = new GridCell[width, height];
        nodes = new List<GridCell>();
        connections = new List<GridConnections>();

        int id = 0;

		for(int x = 0; x < width; x++)
			for(int y = 0; y < height; y++)
			{
				GridCell cell = new GridCell(x, y, id);

				if (Random.value < obstacleProbability)
					cell.isObstacle = true;

				cells[x, y] = cell;
				nodes.Add(cell);
				id++;
			}

        cells[0, 0].isObstacle = false;
        cells[width - 1, height - 1].isObstacle = false;

        BuildConnections();
	}

	private void BuildConnections()
	{
		foreach (GridCell cell in cells)
		{
            GridConnections nconnections = new GridConnections();
			
            if (!cell.isObstacle)
			{
                TryAddConnection(nconnections, cell.gridPos.x + 1, cell.gridPos.y, cell);
                TryAddConnection(nconnections, cell.gridPos.x - 1, cell.gridPos.y, cell);
                TryAddConnection(nconnections, cell.gridPos.x, cell.gridPos.y + 1, cell);
                TryAddConnection(nconnections, cell.gridPos.x, cell.gridPos.y - 1, cell);
            }

			connections.Add(nconnections);
        }
    }

	private void TryAddConnection(GridConnections connections, int x, int y, GridCell cell)
	{
        if (connections == null) return;
        if (x < 0 || y < 0) return;
		if (x >= width || y >= height) return;

		GridCell neighbor = cells[x, y];

		if (!neighbor.isObstacle)
			connections.Add(new CellConnection(cell, neighbor));
	}
	// Class that represent the finite graph corresponding to a grid of cells
	// There is a known set of nodes (GridCells), 
	// and a known set of connections (CellConnections) between those nodes (GridConnections)
	
	// Example Data 
	/* 
	protected float xMin;
	protected float xMax;
	protected float zMin;
	protected float zMax;
	
	protected float gridHeight;
	
	protected float sizeOfCell;
	
	protected int numCells;
	protected int numRows;
	protected int numColumns;
	*/
	
	// Example Constructor function declaration
	// public Grid(float minX, float maxX, float minZ, float maxZ, float cellSize, float height = 0):base()
	
	// You have basically to fill the base fields "nodes" and "connections", 
	// i.e. create your list of GridCells (with random obstacles) 
	// and then create the corresponding GridConnections for each one of them
	// based on where the obstacles are and the valid movements allowed between GridCells. 
	
	
	// TO IMPLEMENT
		
	
	
};
