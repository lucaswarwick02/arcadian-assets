using UnityEngine;
using NaughtyAttributes;

namespace LucasWarwick02.UnityAssets
{
    /// <summary>
    /// A simple Unity component that makes a GameObject smoothly pulse in size. Useful for drawing attention to UI elements or objects with a looping, easing-based scale effect.
    /// </summary>
    [AddComponentMenu("Lucas's Unity Assets/Pulse")]
    public class Pulse : MonoBehaviour
    {
        /// <summary>
        /// The speed multiplier to in the oscillation calculation.
        /// </summary>
        [Tooltip("The speed multiplier to in the oscillation calculation."), BoxGroup("Settings")]
        public float speed = 5;

        /// <summary>
        /// The max scale to increase the size by.
        /// </summary>
        [Tooltip("The max scale to increase the size by."), BoxGroup("Settings")]
        public float scale = 1.15f;

        /// <summary>
        /// Whether or not to use unscaled time.
        /// </summary>
        [Tooltip("Whether or not to use unscaled time."), BoxGroup("Settings")]
        public bool useUnscaledTime = false;

        private void Update()
        {
            var t = Curves.In.Evaluate(Mathf.PingPong((useUnscaledTime ? Time.unscaledTime : Time.time) * speed, 1f));
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * scale, t);
        }

        private void OnDisable()
        {
            transform.localScale = Vector3.one;
        }
    }
}