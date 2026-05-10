using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace LucasWarwick02.UnityAssets
{
    /// <summary>
    /// A Unity component that generates a 2D grid of nodes for pathfinding, providing A*-based shortest path calculations, randomized path variation, and utility methods to query the closest walkable node, while also visualising the grid in the editor for debugging.
    /// </summary>
    [AddComponentMenu("Lucas's Unity Assets/Pathfinding/Grid Pathfinder")]
    public class GridPathfinder : MonoBehaviour
    {
        /// <summary>
        /// Size of the grid to create and use.
        /// </summary>
        [Tooltip("Size of the grid to create and use."), BoxGroup("Grid")]
        public Vector2Int gridSize = new(10, 10);

        /// <summary>
        /// Size of each node (Unity units).
        /// </summary>
        [Tooltip("Size of each node (Unity units)."), BoxGroup("Grid")]
        public float nodeSize = 1f;

        /// <summary>
        /// Layer mask to filter which colliders to check when determining node walkability. Defaults to everything.
        /// </summary>
        [Tooltip("Layer mask to filter which colliders to check when determining node walkability. Defaults to everything."), BoxGroup("Settings")]
        public LayerMask walkabilityLayerMask = -1;

        private Node[,] _grid;

        private Vector3 _gridOrigin;

        private HashSet<Node> _openSetHash = new();
        private List<Node> _openSetList = new();
        private HashSet<Node> _closedSet = new();
        private List<Node> _neighbourCache = new(4); // Max 4 neighbors (no diagonals)

        private void Awake()
        {
            CreateGrid();
        }

        public void CreateGrid()
        {
            _grid = new Node[gridSize.x, gridSize.y];
            CalculateGridOrigin();

            for (var x = 0; x < gridSize.x; x++)
            {
                for (var y = 0; y < gridSize.y; y++)
                {
                    var worldPosition = _gridOrigin + new Vector3(x * nodeSize, y * nodeSize, 0);
                    var walkable = !Physics2D.OverlapCircle(worldPosition, nodeSize / 2, walkabilityLayerMask);
                    _grid[x, y] = new Node(walkable, worldPosition, x, y);
                }
            }
        }

        private void CalculateGridOrigin()
        {
            var gridWorldSize = new Vector3(gridSize.x * nodeSize, gridSize.y * nodeSize, 0);
            _gridOrigin = transform.position - gridWorldSize / 2;
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

        private static int Distance(Node nodeA, Node nodeB)
        {
            var dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
            var dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);
            return dstX + dstY;
        }

        private int RandomizedDistance(Node nodeA, Node nodeB, float randomnessFactor)
        {
            var baseDistance = Distance(nodeA, nodeB);
            var randomFactor = Random.Range(-randomnessFactor, randomnessFactor);
            return Mathf.RoundToInt(baseDistance * (1 + randomFactor));
        }

        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying && _grid != null)
            {
                foreach (var node in _grid)
                {
                    Gizmos.color = node.Walkable ? Color.white.SetAlpha(0.5f) : Color.red.SetAlpha(0.5f);
                    Gizmos.DrawCube(node.WorldPosition, Vector3.one * (nodeSize - 0.1f));
                }
            }
            else
            {
                CalculateGridOrigin();
                Gizmos.color = Color.yellow.SetAlpha(0.5f);
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

        public Node NodeFromWorldPoint(Vector3 worldPosition)
        {
            var localPosition = worldPosition - _gridOrigin;
            var x = Mathf.Clamp(Mathf.RoundToInt(localPosition.x / nodeSize), 0, gridSize.x - 1);
            var y = Mathf.Clamp(Mathf.RoundToInt(localPosition.y / nodeSize), 0, gridSize.y - 1);
            return _grid[x, y];
        }

        /// <summary>
        /// Find the closest, walkable node to this in-game position.
        /// </summary>
        /// <param name="worldPosition">In-game position to query against.</param>
        /// <returns>Closest node to the point, or null.</returns>
        public Node GetClosestValidNode(Vector3 worldPosition)
        {
            Node closestNode = NodeFromWorldPoint(worldPosition);

            if (closestNode.Walkable)
                return closestNode;

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

                openSet.Sort((a, b) =>
                    Vector3.Distance(a.WorldPosition, worldPosition).CompareTo(
                        Vector3.Distance(b.WorldPosition, worldPosition)));
            }

            return null;
        }

        // Modified to reuse a list instead of allocating new one
        private void GetNeighbors(Node node, List<Node> output)
        {
            output.Clear();
            
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
                        output.Add(_grid[checkX, checkY]);
                    }
                }
            }
        }

        private void ResetNodes()
        {
            foreach (var node in _grid)
            {
                node.GCost = int.MaxValue;
                node.HCost = 0;
                node.Parent = null;
            }
        }
        
        /// <summary>
        /// Find the shortest path from A to B.
        /// </summary>
        /// <param name="startPos">In-game starting position.</param>
        /// <param name="targetPos">In-game target (end) position.</param>
        /// <param name="randomnessFactor">How much randomness to add to the generated path (0-1).</param>
        /// <returns></returns>
        public List<Node> FindPath(Vector3 startPos, Vector3 targetPos, float randomnessFactor = 0f)
        {
            var startNode = NodeFromWorldPoint(startPos);
            var targetNode = NodeFromWorldPoint(targetPos);

            // Clear and reuse collections
            _openSetList.Clear();
            _openSetHash.Clear();
            _closedSet.Clear();
            
            // Reset all nodes
            ResetNodes();
            
            _openSetList.Add(startNode);
            _openSetHash.Add(startNode);
            startNode.GCost = 0;
            startNode.HCost = RandomizedDistance(startNode, targetNode, randomnessFactor);

            while (_openSetList.Count > 0)
            {
                // Find lowest F cost node
                var currentNode = _openSetList[0];
                var currentIndex = 0;
                for (var i = 1; i < _openSetList.Count; i++)
                {
                    if (_openSetList[i].FCost < currentNode.FCost || 
                        _openSetList[i].FCost == currentNode.FCost && _openSetList[i].HCost < currentNode.HCost)
                    {
                        currentNode = _openSetList[i];
                        currentIndex = i;
                    }
                }

                // Remove using swap-and-pop (faster than Remove)
                _openSetList[currentIndex] = _openSetList[_openSetList.Count - 1];
                _openSetList.RemoveAt(_openSetList.Count - 1);
                _openSetHash.Remove(currentNode);
                _closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    return RetracePath(startNode, targetNode);
                }

                GetNeighbors(currentNode, _neighbourCache);
                foreach (var neighbor in _neighbourCache)
                {
                    if (!neighbor.Walkable || _closedSet.Contains(neighbor))
                        continue;

                    var newMovementCostToNeighbor = currentNode.GCost + 
                        RandomizedDistance(currentNode, neighbor, randomnessFactor);
                    
                    if (!_openSetHash.Contains(neighbor) || newMovementCostToNeighbor < neighbor.GCost)
                    {
                        neighbor.GCost = newMovementCostToNeighbor;
                        neighbor.HCost = RandomizedDistance(neighbor, targetNode, randomnessFactor);
                        neighbor.Parent = currentNode;

                        if (!_openSetHash.Contains(neighbor))
                        {
                            _openSetList.Add(neighbor);
                            _openSetHash.Add(neighbor);
                        }
                    }
                }
            }

            return null;
        }
    }
}