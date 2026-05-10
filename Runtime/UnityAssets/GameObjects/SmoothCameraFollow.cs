using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using NaughtyAttributes;

namespace LucasWarwick02.UnityAssets
{
    /// <summary>
    /// A reusable Unity component for smoothly following a target (e.g., player) with damping and optional screen shake. Useful for creating dynamic, responsive camera motion that feels natural while maintaining focus on the target.
    /// </summary>
    [DisallowMultipleComponent, AddComponentMenu("Lucas's Unity Assets/Smooth Camera Follow")]
    public class SmoothCameraFollow : MonoBehaviour
    {
        private static SmoothCameraFollow _instance;

        /// <summary>
        /// Target transform to follow.
        /// </summary>
        [Tooltip("Target transform to follow."), BoxGroup("Positional")]
        public Transform target;

        /// <summary>
        /// Offset to use from the target.
        /// </summary>
        [Tooltip("Offset to use from the target."), BoxGroup("Positional")]
        public Vector3 offset;

        /// <summary>
        /// If true, ignores following the z-axis.
        /// </summary>
        [Tooltip("If true, ignores following the z-axis."), BoxGroup("Settings")]
        public bool ignoreZ = true;

        /// <summary>
        /// The time it takes to smooth.
        /// </summary>
        [Tooltip("The time it takes to smooth."), BoxGroup("Settings")]
        public float smoothTime = 0.3f;

        /// <summary>
        /// Maximum distance to delay behind the target.
        /// </summary>
        [Tooltip("Maximum distance to delay behind the target."), BoxGroup("Settings")]
        public float maxDistance = 10f;

        /// <summary>
        /// Speed multiplier when far from target.
        /// </summary>
        [Tooltip("Speed multiplier when far from target."), BoxGroup("Settings")]
        public float speedMultiplier = 2f;

        private Vector3 _velocity = Vector3.zero;
        private Vector3 _originalPosition;
        private bool _isShaking;

        private void Awake()
        {
            if (_instance && _instance != this)
            {
                Debug.LogWarning("Multiple SmoothCameraFollow instances found. Destroying duplicate");
                Destroy(_instance);
                return;
            }

            _instance = this;
        }

        protected virtual void FixedUpdate()
        {
            // Only move the camera if we aren't shaking
            // if (_isShaking) return;

            // Return if no target is present
            if (target == null) return;

            var targetPosition = target.position + offset;
            var currentPosition = transform.position;

            // Calculate distance to target
            var distance = Vector3.Distance(currentPosition, targetPosition);

            // Adjust smooth time based on distance
            var adjustedSmoothTime = smoothTime;
            if (distance > maxDistance)
            {
                adjustedSmoothTime /= speedMultiplier;
            }

            // Calculate new position
            var newPos = Vector3.SmoothDamp(currentPosition, targetPosition, ref _velocity, adjustedSmoothTime);

            // Clamp to max distance if needed
            if (distance > maxDistance)
            {
                newPos = Vector3.ClampMagnitude(newPos - targetPosition, maxDistance) + targetPosition;
            }

            // Ignore Z if specified
            if (ignoreZ)
            {
                newPos.z = currentPosition.z;
            }

            transform.position = newPos;
        }

        /// <summary>
        /// Move the camera instantly to a position, regardless of the target.
        /// </summary>
        /// <param name="position">The position to move the camera to.</param>
        public static void MoveToPosition(Vector3 position)
        {
            if (_instance == null) return;

            _instance.transform.position = position;
            _instance._velocity = Vector3.zero;
        }

        /// <summary>
        /// Shake the camera.
        /// </summary>
        /// <param name="shakeStrength">Strength enumerator.</param>
        /// <param name="duration">Time to shake for.</param>
        public static void Shake(ShakeStrength shakeStrength, float duration)
        {
            if (_instance._isShaking) return;

            _instance.StartCoroutine(_instance.ShakeCoroutine(shakeStrength, duration));
        }

        private IEnumerator ShakeCoroutine(ShakeStrength shakeStrength, float duration)
        {
            _isShaking = true;
            _originalPosition = transform.localPosition;
            var elapsed = 0f;

            while (elapsed < duration)
            {
                var x = Random.Range(-1f, 1f) * GetShakeIntensity(shakeStrength);
                var y = Random.Range(-1f, 1f) * GetShakeIntensity(shakeStrength);

                transform.localPosition += new Vector3(x, y, 0);

                elapsed += Time.deltaTime;

                yield return null;
            }

            _isShaking = false;
        }

        private static float GetShakeIntensity(ShakeStrength strength)
        {
            return strength switch
            {
                ShakeStrength.Low => 0.03f,
                ShakeStrength.Medium => 0.06f,
                ShakeStrength.High => 0.09f,
                _ => GetShakeIntensity(ShakeStrength.Low)
            };
        }
    }
    
    /// <summary>
    /// Strength values to shake by.
    /// </summary>
    public enum ShakeStrength
    {
        Low,
        Medium,
        High
    }
}