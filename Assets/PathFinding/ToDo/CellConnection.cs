using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using PathFinding;

public class CellConnection : Connection<GridCell>
{
	// Class that represent the connection between 2 GridCells
	
	public CellConnection(GridCell from, GridCell to, float cost = 1f):base(from,to){

		this.cost = cost;
	}
};
