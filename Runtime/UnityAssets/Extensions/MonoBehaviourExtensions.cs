using System;
using System.Collections;
using UnityEngine;

namespace LucasWarwick02.UnityAssets
{
    /// <summary>
    /// A set of <c>MonoBehaviour</c> extension methods for timing and scheduling actions. Useful for invoking callbacks at the end of a frame, or smoothly running logic over a given duration without writing custom coroutines.
    /// </summary>
    public static class MonoBehaviourExtensions
    {
        /// <summary>
        /// Invoke an action at the end of a frame.
        /// </summary>
        /// <param name="mono">MonoBehaviour script to create the coroutine from.</param>
        /// <param name="action">Action to be invoked.</param>
        public static void InvokeEndOnFrame(this MonoBehaviour mono, Action action)
        {
            if (mono == null || action == null) return;

            mono.StartCoroutine(InvokeEndOfFrameCoroutine(action));
        }

        private static IEnumerator InvokeEndOfFrameCoroutine(Action action)
        {
            yield return new WaitForEndOfFrame();

            action();
        }

        /// <summary>
        /// Invoke functions before, after, and during a given duration.
        /// Allows for both scaled and unscaled time, and custom animation curves (Lerp, SmoothStep, etc)
        /// </summary>
        /// <param name="duration">Total length to run it for.</param>
        /// <param name="onStart">Invoked at the start.</param>
        /// <param name="onUpdate">Invoked during, with an evaluated percentage.</param>
        /// <param name="onComplete">Invoked at the end.</param>
        /// <param name="curve">Custom interpolation function. Defaults to linear.</param>
        /// <param name="useUnscaledTime">Whether or not to use unscaled time.</param>
        /// <returns></returns>
        public static IEnumerator Tween(
            this MonoBehaviour _,
            float duration,
            Action onStart = null,
            Action<float> onUpdate = null,
            Action onComplete = null,
            AnimationCurve curve = null,
            bool useUnscaledTime = false)
        {
            // If not specified, use a linear increase
            curve ??= AnimationCurve.Linear(0f, 0f, 1f, 1f);

            // Start with a unique action
            onStart?.Invoke();

            var timer = 0f;
            while (timer < duration)
            {
                // Update the timer
                timer += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

                // Invoke the update function using the percentage duration, evaluated against the curve
                onUpdate?.Invoke(curve.Evaluate(Mathf.Clamp01(timer / duration)));

                yield return null;
            }

            // One final update with a full duration
            onUpdate?.Invoke(curve.Evaluate(1f));

            // Finally invoke the completion action
            onComplete?.Invoke();
        }

        public static IEnumerator Tween(
            float duration,
            Action onStart = null,
            Action<float> onUpdate = null,
            Action onComplete = null,
            AnimationCurve curve = null,
            bool useUnscaledTime = false)
        {
            // If not specified, use a linear increase
            curve ??= AnimationCurve.Linear(0f, 0f, 1f, 1f);

            // Start with a unique action
            onStart?.Invoke();

            var timer = 0f;
            while (timer < duration)
            {
                // Update the timer
                timer += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

                // Invoke the update function using the percentage duration, evaluated against the curve
                onUpdate?.Invoke(curve.Evaluate(Mathf.Clamp01(timer / duration)));

                yield return null;
            }

            // One final update with a full duration
            onUpdate?.Invoke(curve.Evaluate(1f));

            // Finally invoke the completion action
            onComplete?.Invoke();
        }

        /// <summary>
        /// Restarts a coroutine by stopping the existing one (if any) and starting a new one with the provided enumerator.
        /// This allows for resuming or reinitializing coroutines without recreating the entire logic from scratch.
        /// </summary>
        /// <param name="monoBehaviour">MonoBehaviour script to manage the coroutine.</param>
        /// <param name="coroutine">Reference to the existing coroutine that will be stopped if not null.</param>
        /// <param name="enumerator">The IEnumerator instance representing the new coroutine to start.</param>
        public static void RestartCoroutine(this MonoBehaviour monoBehaviour, ref Coroutine coroutine, IEnumerator enumerator)
        {
            if (coroutine != null) monoBehaviour.StopCoroutine(coroutine);
            coroutine = monoBehaviour.StartCoroutine(enumerator);
        } 
    }
}