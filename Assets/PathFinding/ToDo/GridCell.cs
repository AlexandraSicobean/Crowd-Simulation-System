
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using PathFinding;

public class GridCell : Node 
{
	public Vector2Int gridPos;
	public bool isObstacle = false;
	public GameObject visual;
    public int agentCount = 0;

    public GridCell(int x, int y, int i):base(i) {
		this.gridPos = new Vector2Int(x, y);
	}

	public GridCell(GridCell n):base(n) {
		this.gridPos = n.gridPos;
		this.isObstacle = n.isObstacle;
	}

	// Spawns a new cube in the scene
	public void SetVisual (GameObject prefab, Transform parent)
	{
		visual = GameObject.Instantiate(prefab, parent);
		visual.transform.position = new Vector3(gridPos.x, 0, gridPos.y);
		UpdateColor();
	}

	// Change color of the cube (black = obstacle, white = not obstacle)
    public void UpdateColor()
    {
        if (visual == null) return;

        Renderer r = visual.GetComponent<Renderer>();

        if (isObstacle)
            r.material.color = Color.black;
        else
            r.material.color = Color.gray;
    }

    public Vector3 GetWorldPosition(Vector3 origin)
    {
        return origin + new Vector3(
            gridPos.x + 0.5f,
            0,
            gridPos.y + 0.5f
        );
    }



    // Your class that represents a grid cell node derives from Node

    // You add any data needed to represent a grid cell node

    // EXAMPLE DATA
    /*
	protected float xMin;
	protected float xMax;
	protected float zMin;
	protected float zMax;

	protected bool occupied;

	protected Vector3 center;
	*/

    // You also add any constructors and methods to implement your grid cell node class

    // TO IMPLEMENT
};
