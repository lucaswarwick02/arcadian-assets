using UnityEngine;
using UnityEngine.UIElements;

namespace LucasWarwick02.UnityAssets.UIToolkit
{
    /// <summary>
    /// Increase and decrease in a loop the scale.
    /// </summary>
    public class PulseEffect : VisualElementEffect
    {
        /// <summary>
        /// Speed multiplier against deltaTime.
        /// </summary>
        public float Speed { set; get; } = 2f;

        /// <summary>
        /// Minimum scale value.
        /// </summary>
        public float MinScale { set; get; } = 1f;

        /// <summary>
        /// Maximum scale value.
        /// </summary>
        public float MaxScale { set; get; } = 1.15f;

        /// <inheritdoc/>
        public override void Stop()
        {
            base.Stop();

            if (VisualElement == null) return;

            VisualElement.style.scale = Vector3.one;
            VisualElement = null;
        }

        /// <inheritdoc/>
        public override void Tick(TimerState state)
        {
            base.Tick(state);

            float t = (Mathf.Sin(Timer * Speed * Mathf.PI * 2f) + 1f) / 2f;
            float scale = Mathf.Lerp(MinScale, MaxScale, t);

            VisualElement.style.scale = new Vector3(scale, scale, 1f);
        }
    }
}