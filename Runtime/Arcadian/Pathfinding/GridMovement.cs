using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arcadian.Pathfinding
{
    [ExecuteAlways]
    public class GridMovement : MonoBehaviour
    {
        public event Action TargetReached;
        public float speed = 2;
        
        public bool IsMoving { private set; get; }
        public Vector2 Velocity { private set; get; }
        public Vector2 LastVelocity { private set; get; }

        public void SetPath(IEnumerable<Node> path)
        {
            StartCoroutine(Move(path));
        }
        
        private IEnumerator Move(IEnumerable<Node> path)
        {
            IsMoving = true;
            
            foreach (var node in path)
            {
                yield return MoveToNode(node);
            }

            IsMoving = false;
            Velocity = Vector2.zero;
            
            TargetReached?.Invoke();
        }

        private IEnumerator MoveToNode(Node node)
        {
            Velocity = (node.WorldPosition - transform.position).normalized;
            LastVelocity = Velocity;

            while (Vector3.Magnitude(transform.position - node.WorldPosition) > 0.0125f)
            {
                transform.position += new Vector3(Velocity.x, Velocity.y, 0) * (speed * Time.deltaTime);

                yield return null;
            }

            transform.position = node.WorldPosition;
        }
    }
}