using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PathFinding
{
    public class Bidirectional_A_Star : PathFinder<GridCell, CellConnection, GridConnections, Grid, GridHeuristic>
    {
        private Grid_A_Star forwardSearch;
        private Grid_A_Star backwardSearch;
        private HashSet<GridCell> forwardVisited;
        private HashSet<GridCell> backwardVisited;

        public Bidirectional_A_Star(int maxNodes, float maxTime, int maxDepth)
        {
            backwardSearch = new Grid_A_Star(maxNodes, maxTime, maxDepth);
            forwardSearch = new Grid_A_Star(maxNodes, maxTime, maxDepth);
        }

        public override List<GridCell> findpath(Grid graph, GridCell start, GridCell end, GridHeuristic heuristic, ref int found)
        {
            found = -1;

            forwardVisited = new HashSet<GridCell>();
            backwardVisited = new HashSet<GridCell>();  

            GridHeuristic backwardHeuristic = new GridHeuristic(start);

            int maxIterations = 10000;
            int it = 0;

            while (it < maxIterations)
            {
                it++;

                int forwardFound = -1;
                forwardSearch.Step(graph, start, end, heuristic, forwardVisited, ref forwardFound);
                GridCell meet = CheckIntersection(forwardVisited, backwardVisited);
                if (meet != null)
                {
                    found = 1;
                    return MergePaths(forwardSearch.BuildPartialPath(meet),
                        backwardSearch.BuildPartialPath(meet));
                }
                if (forwardFound == 1)
                {
                    found = 1;
                    return forwardSearch.BuildPartialPath(end);
                }

                int backwardFound = -1;
                backwardSearch.Step(graph, end, start, backwardHeuristic, backwardVisited, ref backwardFound);
                meet = CheckIntersection(forwardVisited, backwardVisited);
                if (meet != null)
                {
                    found = 1;
                    return MergePaths(forwardSearch.BuildPartialPath(meet),
                        backwardSearch.BuildPartialPath(meet));
                }
                if (backwardFound == 1)
                {
                    found = 1;
                    return backwardSearch.BuildPartialPath(start);
                }
            }

            found = -1;
            return new List<GridCell>();
        }



        private GridCell CheckIntersection(HashSet<GridCell> cellSet1, HashSet<GridCell> cellSet2)
        {
            foreach (var cell in cellSet1)
                if (cellSet2.Contains(cell))
                    return cell;
            return null;
        }

        private List<GridCell> MergePaths(List<GridCell> forwardPath, List<GridCell> backwardPath)
        {
            backwardPath.Reverse();
            backwardPath.RemoveAt(0);

            List<GridCell> merged = new List<GridCell>(forwardPath);
            merged.AddRange(backwardPath);

            return merged;
        }
    }
}
