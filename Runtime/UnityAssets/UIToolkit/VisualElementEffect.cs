using UnityEngine;
using UnityEngine.UIElements;

namespace LucasWarwick02.UnityAssets.UIToolkit
{
    /// <summary>
    /// Template for continuous visual element effects.
    /// </summary>
    public abstract class VisualElementEffect
    {
        public VisualElement VisualElement { set; get; }

        public float Timer { private set; get; }

        private IVisualElementScheduledItem _scheduledItem;

        /// <summary>
        /// Start the effect
        /// </summary>
        public virtual void Start(VisualElement el)
        {
            VisualElement = el;
            Timer = 0f;

            _scheduledItem = VisualElement.schedule.Execute(Tick).Every(0);
        }

        /// <summary>
        /// What logic to run each frame.
        /// </summary>
        public virtual void Tick(TimerState state)
        {
            if (VisualElement == null) return;

            Timer += state.deltaTime / 1000f;
        }

        /// <summary>
        /// Stop the effect.
        /// </summary>
        public virtual void Stop()
        {
            _scheduledItem?.Pause();
            _scheduledItem = null;
        }
    }
}