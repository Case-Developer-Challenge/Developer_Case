using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFind
{
    public class PathFindingNode
    {
        public Vector2Int currentPos;
        public PathFindingNode parent;
        public int g;
        public int h;
        public int F => g + h;
        public PathFindingNode(Vector2Int pos, PathFindingNode parent, int g, int h)
        {
            currentPos = pos;
            this.parent = parent;
            this.g = g;
            this.h = h;
        }
    }
    public static List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        //todo implement for piece bigger than 1,1
        var openList = new List<PathFindingNode>();
        var closedList = new HashSet<PathFindingNode>();

        var startNode = new PathFindingNode(start, null, 0, Heuristic(start, goal));
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            var currentNode = openList.OrderBy(n => n.F).First();

            if (currentNode.currentPos == goal)
                return ReconstructPath(currentNode);

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            List<Vector2Int> neighbors = GetNeighbors(currentNode.currentPos);

            foreach (var neighbor in neighbors)
            {
                if (!IsWalkable(neighbor) || closedList.Any(n => n.currentPos == neighbor))
                    continue;

                var tentativeGScore = currentNode.g + 1; 

                var neighborNode = openList.FirstOrDefault(n => n.currentPos == neighbor);
                if (neighborNode == null)
                {
                    neighborNode = new PathFindingNode(neighbor, currentNode, tentativeGScore, Heuristic(neighbor, goal));
                    openList.Add(neighborNode);
                }
                else if (tentativeGScore < neighborNode.g)
                {
                    neighborNode.parent = currentNode;
                    neighborNode.g = tentativeGScore;
                }
            }
        }

        return null; // No path found
    }
    public static int CalculatePathLength(Vector2Int start, Vector2Int target)
    {
        var path = FindPath(start, target); 
        return path.Count;
    }

    private static int Heuristic(Vector2Int from, Vector2Int to)
    {
        return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
    }
    private static List<Vector2Int> GetNeighbors(Vector2Int currentPos)
    {
        var neighbors = new List<Vector2Int>
        {
            new(currentPos.x + 1, currentPos.y), // Right
            new(currentPos.x - 1, currentPos.y), // Left
            new(currentPos.x, currentPos.y + 1), // Up
            new(currentPos.x, currentPos.y - 1)  // Down
        };

        return neighbors;
    }
    private static bool IsWalkable(Vector2Int pos)
    {
        return BoardManager.Instance.IsPieceEmpty(pos);
    }
    private static List<Vector2Int> ReconstructPath(PathFindingNode currentNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        while (currentNode != null)
        {
            path.Add(currentNode.currentPos);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

}