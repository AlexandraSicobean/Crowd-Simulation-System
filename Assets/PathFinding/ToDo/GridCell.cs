
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

	public void SetVisual (GameObject prefab, Transform parent)
	{
		visual = GameObject.Instantiate(prefab, parent);
		visual.transform.position = new Vector3(gridPos.x, 0, gridPos.y);
		UpdateColor();
	}

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
};
