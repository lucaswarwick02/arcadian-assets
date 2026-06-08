using UnityEngine;
using UnityEngine.UIElements;

namespace LucasWarwick02.UnityAssets.UIToolkit
{
    /// <summary>
    /// Increases to max scale then returns to normal scale once, then stops.
    /// </summary>
    public class OpenEffect : VisualElementEffect
    {   
        /// <summary>
        /// Speed multiplier against deltaTime.
        /// </summary>
        public float Speed { set; get; } = 2f;

        /// <summary>
        /// Minimum scale value.
        /// </summary>
        public float NormalScale { set; get; } = 1f;

        /// <summary>
        /// Maximum scale value.
        /// </summary>
        public float MaxScale { set; get; } = 1.15f;

        /// <inheritdoc/>
        public override void Stop()
        {
            base.Stop();
        }

        /// <inheritdoc/>
        public override void Tick(TimerState state)
        {
            base.Tick(state);

            // Sin goes 0 -> 1 -> 0 over the range [0, PI], giving us one arch.
            // Timer * Speed controls how fast we traverse that arch.
            float progress = Timer * Speed * Mathf.PI;

            // Once we've completed the half-cycle (progress >= PI), stop.
            if (progress >= Mathf.PI)
            {
                Stop();
                return;
            }

            float t = Mathf.Sin(progress);
            float scale = Mathf.Lerp(NormalScale, MaxScale, t);

            VisualElement.style.scale = new Vector3(scale, scale, 1f);
        }
    }
}