using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace LucasWarwick02.UnityAssets
{
    /// <summary>
    /// A Unity component that moves an object smoothly along a series of <c>Node</c> positions, providing movement speed control, current velocity tracking, and an event callback when the final target node is reached. Ideal for pathfinding agents or grid-based movement systems.
    /// </summary>
    [ExecuteAlways, AddComponentMenu("Lucas's Unity Assets/Pathfinding/Grid Movement")]
    public class GridMovement : MonoBehaviour
    {
        /// <summary>
        /// Invoked when the target node has been reached.
        /// </summary>
        public event Action TargetReached;

        /// <summary>
        /// Speed to move the object (units/s).
        /// </summary>
        [Tooltip("Speed to move the object (units/s)."), BoxGroup("Settings")]
        public float speed = 2;
        
        /// <summary>
        /// How much randomness to add to generated paths (0-1).
        /// </summary>
        [Range(0f, 1f), Tooltip("How much randomness to add to generated paths."), BoxGroup("Settings")]
        public float pathRandomness = 0.1f;
        
        /// <summary>
        /// Is the object currently moving?
        /// </summary>
        public bool IsMoving { private set; get; }

        /// <summary>
        /// What is the current velocity of the object?
        /// </summary>
        public Vector2 Velocity { private set; get; }

        private Queue<Node> pathQueue = new Queue<Node>();
        private Coroutine moveCoroutine;

        /// <summary>
        /// What was the last velocity of the object?
        /// </summary>
        public Vector2 LastVelocity { private set; get; }

        /// <summary>
        /// Set and start the path traversal.
        /// </summary>
        /// <param name="path">Path of nodes to iterate over.</param>
        public void SetPath(IEnumerable<Node> path)
        {
            // Check if path is null
            if (path == null)
            {
                return;
            }

            // Clear current path and add new nodes
            pathQueue.Clear();
            foreach (var node in path)
            {
                pathQueue.Enqueue(node);
            }

            // Start movement if not already moving
            if (moveCoroutine == null)
            {
                moveCoroutine = StartCoroutine(MoveAlongPath());
            }
        }
        
        private IEnumerator MoveAlongPath()
        {
            IsMoving = true;
            
            while (pathQueue.Count > 0)
            {
                var currentNode = pathQueue.Dequeue();
                yield return MoveToNode(currentNode);
            }

            IsMoving = false;
            Velocity = Vector2.zero;
            moveCoroutine = null;
            
            TargetReached?.Invoke();
        }

        /// <summary>
        /// Stop the current movement and clear the path.
        /// </summary>
        public void StopMovement()
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                moveCoroutine = null;
            }
            
            pathQueue.Clear();
            IsMoving = false;
            Velocity = Vector2.zero;
        }

        private IEnumerator MoveToNode(Node node)
        {
            const float thresholdSqr = 0.0125f * 0.0125f; // Pre-calculate squared threshold
            Vector3 targetPosition = node.WorldPosition;
            
            while (true)
            {
                Vector3 currentPosition = transform.position;
                Vector3 requiredMovement = targetPosition - currentPosition;
                
                // Use sqrMagnitude instead of Magnitude to avoid sqrt calculation
                if (requiredMovement.sqrMagnitude <= thresholdSqr)
                    break;
                
                // Cache normalized direction and reuse for both velocity and movement
                Vector3 direction = requiredMovement.normalized;
                Velocity = direction;
                LastVelocity = Velocity;

                // Move using cached direction, avoid creating new Vector3
                transform.position = currentPosition + direction * (speed * Time.deltaTime);

                yield return null;
            }

            transform.position = targetPosition;
            Velocity = Vector2.zero; // Stop at the node
        }
    }
}