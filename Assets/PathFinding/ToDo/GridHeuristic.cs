using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using PathFinding;

public class GridHeuristic : Heuristic<GridCell>
{
	// Class that represents a Heuristic function to estimate the cost of going from 
	// one GridCell to another

	
	// constructor takes a goal node for estimating
	public GridHeuristic(GridCell goal):base(goal){
		goalNode = goal;
	}
	
	 // generates an estimated cost to reach the stored goal from the given node
	public override float estimateCost(GridCell fromNode){
		return Mathf.Sqrt((fromNode.gridPos.x - goalNode.gridPos.x) * (fromNode.gridPos.x - goalNode.gridPos.x) +
			(fromNode.gridPos.y - goalNode.gridPos.y) * (fromNode.gridPos.y - goalNode.gridPos.y));
	}

	// determines if the goal node has been reached by node
	public override bool goalReached(GridCell node){
		return node.gridPos.x == goalNode.gridPos.x && 
			node.gridPos.y == goalNode.gridPos.y;
	}

};
