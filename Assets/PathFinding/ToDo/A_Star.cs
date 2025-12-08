using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PathFinding{

	public class A_Star<TNode, TConnection, TNodeConnection, TGraph, THeuristic> : PathFinder<TNode, TConnection, TNodeConnection, TGraph, THeuristic>
	where TNode : Node
	where TConnection : Connection<TNode>
	where TNodeConnection : NodeConnections<TNode, TConnection>
	where TGraph : Graph<TNode, TConnection, TNodeConnection>
	where THeuristic : Heuristic<TNode>
	{
		// Class that implements the A* pathfinding algorithm
		// You have to implement the findpath function.
		// You can add whatever you need.

		protected Dictionary<TNode, NodeRecord> nodeRecords;
		protected List<NodeRecord> openList;
		protected List<NodeRecord> closedList;

		protected List<TNode> visitedNodes; // list of visited nodes 

		protected NodeRecord currentBest; // current best node found

		protected enum NodeRecordCategory { OPEN, CLOSED, UNVISITED };

		protected class NodeRecord {
			// You can use (or not) this structure to keep track of the information that we need for each node

			public NodeRecord() { }

			public TNode node;
			public NodeRecord connection;   // connection traversed to reach this node 
			public float costSoFar; // cost accumulated to reach this node
			public float estimatedTotalCost; // estimated total cost to reach the goal from this node
			public NodeRecordCategory category; // category of the node: open, closed or unvisited
			public int depth; // depth in the search graph
		};

		public A_Star(int maxNodes, float maxTime, int maxDepth) : base() {

			visitedNodes = new List<TNode>();

		}

		public virtual List<TNode> getVisitedNodes() {
			return visitedNodes;
		}

		private void InitializeSearch(TNode start, THeuristic heuristic)
		{
			nodeRecords = new Dictionary<TNode, NodeRecord>();
			openList = new List<NodeRecord>();
			closedList = new List<NodeRecord>();
			visitedNodes.Clear();

			NodeRecord startNode = new NodeRecord();
			startNode.node = start;
			startNode.costSoFar = 0f;
			startNode.estimatedTotalCost = heuristic.estimateCost(start);
			startNode.category = NodeRecordCategory.OPEN;

			nodeRecords[start] = startNode;
			openList.Add(startNode);
		}

		public override List<TNode> findpath(TGraph graph, TNode start, TNode end, THeuristic heuristic, ref int found)
		{
			found = -1;
			InitializeSearch(start, heuristic);

			while (openList.Count > 0)
			{
				openList = openList.OrderBy(r => r.estimatedTotalCost).ToList();
				NodeRecord currentNode = openList[0];

				if (heuristic.goalReached(currentNode.node) || currentNode.node.Equals(end))
				{
					found = 1;
					return BuildPath(currentNode);
				}

				openList.Remove(currentNode);
				currentNode.category = NodeRecordCategory.CLOSED;
				closedList.Add(currentNode);

				visitedNodes.Add(currentNode.node);

				TNodeConnection connections = graph.getConnections(currentNode.node);
				foreach (var connection in connections.connections)
				{
					TNode next = connection.toNode;
					float g = currentNode.costSoFar + connection.cost;

					NodeRecord nextNode;
					bool isNodeClosed = false;
					bool isNodeOpen = false;

					if (!nodeRecords.TryGetValue(next, out nextNode))
					{
						nextNode = new NodeRecord();
						nextNode.node = next;
						nextNode.category = NodeRecordCategory.UNVISITED;
						nodeRecords[next] = nextNode;
					}
					else
					{
						isNodeClosed = closedList.Contains(nextNode);
						isNodeOpen = openList.Contains(nextNode);
					}

					if (isNodeOpen && g >= nextNode.costSoFar)
						continue;
					if (isNodeClosed && g >= nextNode.costSoFar)
						continue;

					nextNode.costSoFar = g;
					nextNode.connection = currentNode;
					nextNode.estimatedTotalCost = g + heuristic.estimateCost(nextNode.node);

					if (!isNodeOpen)
					{
						nextNode.category = NodeRecordCategory.OPEN;
						openList.Add(nextNode);
					}
					if (isNodeClosed)
						closedList.Remove(nextNode);
				}

			}

			found = 0;

			return new List<TNode>();
		}

		public List<TNode> Step(TGraph graph, TNode start, TNode end, THeuristic heuristic, HashSet<TNode> visited, ref int found)
		{
			if (openList == null || nodeRecords == null)
				InitializeSearch(start, heuristic);

			if (openList.Count == 0)
			{
				found = 0;
				return null;
			}

            openList = openList.OrderBy(r => r.estimatedTotalCost).ToList();
            NodeRecord currentNode = openList[0];

            if (heuristic.goalReached(currentNode.node) || currentNode.node.Equals(end))
            {
                found = 1;
                return BuildPath(currentNode);
            }

            openList.Remove(currentNode);
            currentNode.category = NodeRecordCategory.CLOSED;
            closedList.Add(currentNode);

            visited.Add(currentNode.node);

            TNodeConnection connections = graph.getConnections(currentNode.node);
            foreach (var connection in connections.connections)
            {
                TNode next = connection.toNode;
                float g = currentNode.costSoFar + connection.cost;

                NodeRecord nextNode;
                bool isNodeClosed = false;
                bool isNodeOpen = false;

                if (!nodeRecords.TryGetValue(next, out nextNode))
                {
                    nextNode = new NodeRecord();
                    nextNode.node = next;
                    nextNode.category = NodeRecordCategory.UNVISITED;
                    nodeRecords[next] = nextNode;
                }
                else
                {
                    isNodeOpen = openList.Contains(nextNode);
                    isNodeClosed = closedList.Contains(nextNode);
                }

                if (isNodeOpen && g >= nextNode.costSoFar)
                    continue;
                if (isNodeClosed && g >= nextNode.costSoFar)
                    continue;

                nextNode.costSoFar = g;
                nextNode.connection = currentNode;
                nextNode.estimatedTotalCost = g + heuristic.estimateCost(nextNode.node);

                if (isNodeClosed)
                    closedList.Remove(nextNode);
                if (!isNodeOpen)
                {
                    nextNode.category = NodeRecordCategory.OPEN;
                    openList.Add(nextNode);
                }
            }

            found = 0;

            return null;
        }

        protected List<TNode> BuildPath(NodeRecord endNode)
		{
			List<TNode> path = new List<TNode>();

			NodeRecord currentNode = endNode;

			while (currentNode != null)
			{
				path.Add(currentNode.node);
				currentNode = currentNode.connection;
			}

			path.Reverse();
			return path;

		}

		public List<TNode> BuildPartialPath(TNode meetNode)
		{
			if(!nodeRecords.ContainsKey(meetNode))
				return new List<TNode> ();
			return BuildPath(nodeRecords[meetNode]);
		}

	};

}