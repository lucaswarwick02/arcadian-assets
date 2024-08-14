using System.Collections.Generic;
using UnityEngine;

namespace Arcadian.Extensions
{
    public static class TransformExtensions
    {
        public static T GetClosest<T>(this Transform transform, List<T> monoBehaviours) where T : MonoBehaviour
        {
            T closestTransform = null;
            var closestDistance = Mathf.Infinity;

            foreach (var monoBehaviour in monoBehaviours)
            {
                var distance = Vector3.Distance(monoBehaviour.transform.position, transform.position);
                if (!(distance < closestDistance)) continue;
                
                closestTransform = monoBehaviour;
                closestDistance = distance;
            }

            return closestTransform;
        }
    }
}