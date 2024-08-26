using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arcadian.Pathfinding
{
    public class GridMovement : MonoBehaviour
    {
        [SerializeField] private bool canMove = true;
        
        [Header("Required Components")]
        [SerializeField] private GridPathfinder gridPathfinder;
        
        [Header("Movement Settings")]
        [SerializeField] private Vector3 targetPosition;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float pathUpdateInterval = 0.5f;
        [SerializeField] private float reachDistance = 0.1f;

        private readonly List<Vector3> _pathPositions = new();

        private List<Node> _path;
        private int _currentWaypointIndex;

        private void Start()
        {
            StartCoroutine(UpdatePathRoutine());
        }

        private void Update()
        {
            if (!canMove || _path == null || _currentWaypointIndex >= _path.Count) return;
            
            var targetPosLocal = _path[_currentWaypointIndex].WorldPosition;
            var position = transform.position;
            targetPosLocal.z = position.z; // Keep Z-position constant for 2D

            position = Vector3.MoveTowards(position, targetPosLocal, moveSpeed * Time.deltaTime);
            transform.position = position;

            if (Vector3.Distance(transform.position, targetPosLocal) < reachDistance)
            {
                _currentWaypointIndex++;
            }
        }

        public void SetTarget(Vector3 newTarget)
        {
            targetPosition = newTarget;
            UpdatePath();
        }

        private void UpdatePath()
        {
            if (!gridPathfinder) return;
            
            _path = gridPathfinder.FindPath(transform.position, targetPosition);
            _currentWaypointIndex = 0;
            
            _pathPositions.Clear();
            if (_path == null) return;
            
            foreach (var node in _path)
            {
                _pathPositions.Add(node.WorldPosition);
            }
        }

        private IEnumerator UpdatePathRoutine()
        {
            while (true)
            {
                UpdatePath();
                yield return new WaitForSeconds(pathUpdateInterval);
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}