using System.Linq;
using LucasWarwick02.UnityAssets;
using UnityEngine;
using UnityEngine.UIElements;

namespace LucasWarwick02.UnityAssets.UIToolkit
{
    /// <summary>
    /// Extension methods for the UI Toolkit VisualElement class.
    /// </summary>
    public static class UIToolkitExtensions
    {
        /// <summary>
        /// Start an effect
        /// </summary>
        public static T StartEffect<T>(this VisualElement el, T effect) where T : VisualElementEffect
        {
            effect.Start(el);
            return effect;
        }

        /// <summary>
        /// Focus on the element if we are using a Gamepad.
        /// </summary>
        /// <param name="element"></param>
        public static void GamepadFocus(this VisualElement element)
        {
            if (InputManager.CurrentScheme != InputScheme.Gamepad) return;

            element.Focus();
        }

        /// <summary>
        /// Attempt to perform Q.
        /// </summary>
        public static bool TryQ<T>(this VisualElement element, out T result, string name = null, string className = null)
        where T : VisualElement
        {
            result = element.Q<T>(name, className);
            return result != null;
        }

        /// <summary>
        /// Positions the card adjacent to a trigger element, with intelligent placement to avoid screen edges.
        /// </summary>
        public static void PositionNextTo(this VisualElement element, VisualElement trigger, VisualElement rootContainer, float offset = 50f)
        {
            void Position()
            {
                var tb = trigger.worldBound;
                var scale = trigger.resolvedStyle.scale.value;
                var pw = element.worldBound.width;
                var ph = element.worldBound.height;
                var sw = rootContainer.worldBound.width;
                var sh = rootContainer.worldBound.height;

                // Reconstruct unscaled bounds from center
                float centerX = tb.center.x;
                float centerY = tb.center.y;
                float unscaledHalfW = tb.width / scale.x / 2;
                float unscaledHalfH = tb.height / scale.y / 2;

                float unscaledXMin = centerX - unscaledHalfW;
                float unscaledXMax = centerX + unscaledHalfW;

                bool triggerOnLeft = centerX < sw / 2;
                float x = triggerOnLeft ? unscaledXMax + offset : unscaledXMin - pw - offset;
                float y = centerY - (ph / 2);
                y = Mathf.Clamp(y, 8f, sh - ph - 8f);

                element.style.left = x;
                element.style.top = y;
            }

            EventCallback<GeometryChangedEvent> callback = null;
            callback = _ =>
            {
                Position();
                element.UnregisterCallback(callback);
            };
            element.RegisterCallback(callback);

            // Fallback in case geometry doesn't change (card already laid out)
            element.schedule.Execute(() =>
            {
                element.UnregisterCallback(callback);
                Position();
            }).StartingIn(50);
        }

        /// <summary>
        /// Instantiate the asset, optionally setting the parent (via `Add()`), unwrapping the template wrapper, and bringing to front.
        /// </summary>
        public static VisualElement Instantiate(this VisualTreeAsset asset, VisualElement parent = null, bool unwrap = false, bool bringToFront = false)
        {
            var instance = asset.Instantiate();
            var element = unwrap ? instance.Children().First() : instance;
            parent?.Add(element);
            if (bringToFront) element.BringToFront();
            return element;
        }

        /// <summary>
        /// Instantiate with optional parent, unwrapping, bringing to front, and cast to a target type.
        /// </summary>
        public static T Instantiate<T>(this VisualTreeAsset asset, VisualElement parent = null, bool unwrap = false, bool bringToFront = false) where T : VisualElement
        {
            return (T)asset.Instantiate(parent, unwrap, bringToFront);
        }

        /// <summary>
        /// Find a parent based on it's type and class.
        /// </summary>
        public static T FindAncestor<T>(this VisualElement element, string className = null) where T : VisualElement
        {
            VisualElement current = element?.parent;
            while (current != null)
            {
                if (current is T typedElement && (className == null || typedElement.ClassListContains(className)))
                {
                    return typedElement;
                }
                current = current.parent;
            }
            return null;
        }

        /// <summary>
        /// Same as <c>ScrollView.ScrollTo()</c>, but uses a smooth-step easing curve instead of instant movement.
        /// </summary>
        public static void ScrollToSmooth(this ScrollView scrollView, VisualElement target, float duration = 0.0625f)
        {
            if (scrollView == null || target == null) return;

            if (!(InputManager.CurrentScheme == InputScheme.Gamepad)) return;

            // 1. Save where we are right now
            Vector2 startOffset = scrollView.scrollOffset;

            // 2. Let Unity's native engine instantly calculate the exact destination
            scrollView.ScrollTo(target);
            Vector2 targetOffset = scrollView.scrollOffset;

            // 3. Immediately snap back to the start so the jump is invisible to the user
            scrollView.scrollOffset = startOffset;

            // If we're already where we need to be, exit
            if (Mathf.Approximately(startOffset.y, targetOffset.y)) return;

            // 4. Smoothly interpolate between the two known points using unscaled delta time
            float elapsed = 0f;

            IVisualElementScheduledItem task = null;
            task = scrollView.schedule.Execute(() =>
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                
                // Smooth-step easing curve
                t = t * t * (3f - 2f * t);

                scrollView.scrollOffset = Vector2.Lerp(startOffset, targetOffset, t);

                if (t >= 1f)
                {
                    task.Pause();
                }
            }).Every(0);
        }

        /// <summary>
        /// Focus on an element after a 1 ms delay.
        /// </summary>
        public static void UnFocus(this VisualElement element)
        {
            element.schedule.Execute(() => element.Focus()).StartingIn(1);
        }
    }
}