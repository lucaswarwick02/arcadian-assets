using System.Collections.Generic;
using UnityEngine;

namespace Arcadian.Pathfinding
{
    public class GridPathfinder : MonoBehaviour
    {
        public Vector2Int gridSize = new(10, 10);
        public float nodeSize = 1f;
        
        private Node[,] _grid;
        private Vector3 _gridOrigin;

        private void Awake()
        {
            CreateGrid();
        }

        private void CreateGrid()
        {
            _grid = new Node[gridSize.x, gridSize.y];
            CalculateGridOrigin();

            for (var x = 0; x < gridSize.x; x++)
            {
                for (var y = 0; y < gridSize.y; y++)
                {
                    var worldPosition = _gridOrigin + new Vector3(x * nodeSize, y * nodeSize, 0);
                    var walkable = !Physics2D.OverlapCircle(worldPosition, nodeSize / 2);
                    _grid[x, y] = new Node(walkable, worldPosition, x, y);
                }
            }
        }

        private void CalculateGridOrigin()
        {
            var gridWorldSize = new Vector3(gridSize.x * nodeSize, gridSize.y * nodeSize, 0);
            _gridOrigin = transform.position - gridWorldSize / 2;
        }

        public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
        {
            var startNode = NodeFromWorldPoint(startPos);
            var targetNode = NodeFromWorldPoint(targetPos);

            var openSet = new List<Node>();
            var closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                var currentNode = openSet[0];
                for (var i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost)
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    return RetracePath(startNode, targetNode);
                }

                foreach (var neighbor in Neighbors(currentNode))
                {
                    if (!neighbor.Walkable || closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    var newMovementCostToNeighbor = currentNode.GCost + Distance(currentNode, neighbor);
                    if (newMovementCostToNeighbor >= neighbor.GCost && openSet.Contains(neighbor)) continue;
                    
                    neighbor.GCost = newMovementCostToNeighbor;
                    neighbor.HCost = Distance(neighbor, targetNode);
                    neighbor.Parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }

            return null;
        }
        
        public Node GetClosestValidNode(Vector3 worldPosition)
        {
            Node closestNode = NodeFromWorldPoint(worldPosition);
    
            // If the closest node is already walkable, return it
            if (closestNode.Walkable)
                return closestNode;

            // If not, search for the nearest walkable node
            var openSet = new List<Node> { closestNode };
            var closedSet = new HashSet<Node>();

            while (openSet.Count > 0)
            {
                var currentNode = openSet[0];
                openSet.RemoveAt(0);
                closedSet.Add(currentNode);

                if (currentNode.Walkable)
                    return currentNode;

                foreach (var neighbor in Neighbors(currentNode))
                {
                    if (closedSet.Contains(neighbor))
                        continue;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }

                // Sort the open set by distance from the original position
                openSet.Sort((a, b) => 
                    Vector3.Distance(a.WorldPosition, worldPosition).CompareTo(
                        Vector3.Distance(b.WorldPosition, worldPosition)));
            }

            // If no walkable node is found, return null or handle the case as needed
            return null;
        }

        private static List<Node> RetracePath(Node startNode, Node endNode)
        {
            var path = new List<Node>();
            var currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            path.Reverse();
            return path;
        }

        private List<Node> Neighbors(Node node)
        {
            var neighbors = new List<Node>();
            for (var x = -1; x <= 1; x++)
            {
                for (var y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    if (Mathf.Abs(x) + Mathf.Abs(y) == 2)
                        continue; // Skip diagonals

                    var checkX = node.GridX + x;
                    var checkY = node.GridY + y;

                    if (checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y)
                    {
                        neighbors.Add(_grid[checkX, checkY]);
                    }
                }
            }
            return neighbors;
        }

        public Node NodeFromWorldPoint(Vector3 worldPosition)
        {
            var localPosition = worldPosition - _gridOrigin;
            var x = Mathf.Clamp(Mathf.RoundToInt(localPosition.x / nodeSize), 0, gridSize.x - 1);
            var y = Mathf.Clamp(Mathf.RoundToInt(localPosition.y / nodeSize), 0, gridSize.y - 1);
            return _grid[x, y];
        }

        private static int Distance(Node nodeA, Node nodeB)
        {
            var dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
            var dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);
            return dstX + dstY;
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying && _grid != null)
            {
                foreach (var node in _grid)
                {
                    Gizmos.color = node.Walkable ? Color.white : Color.red;
                    Gizmos.DrawCube(node.WorldPosition, Vector3.one * (nodeSize - 0.1f));
                }
            }
            else
            {
                CalculateGridOrigin();
                Gizmos.color = Color.yellow;
                var gridWorldSize = new Vector3(gridSize.x * nodeSize, gridSize.y * nodeSize, 0);
                Gizmos.DrawWireCube(transform.position, gridWorldSize);
            }
        }

        private void OnValidate()
        {
            if (gridSize.x < 1) gridSize.x = 1;
            if (gridSize.y < 1) gridSize.y = 1;
            if (nodeSize <= 0) nodeSize = 0.1f;
        }
    }

    public class Node
    {
        public readonly bool Walkable;
        public Vector3 WorldPosition;
        public readonly int GridX;
        public readonly int GridY;
        public int GCost;
        public int HCost;
        public Node Parent;

        public int FCost => GCost + HCost;

        public Node(bool walkable, Vector3 worldPos, int gridX, int gridY)
        {
            Walkable = walkable;
            WorldPosition = worldPos;
            GridX = gridX;
            GridY = gridY;
        }
    }
}