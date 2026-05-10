using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace LucasWarwick02.UnityAssets
{
    /// <summary>
    /// A lightweight utility for displaying temporary animated text in world space, ideally for effects like damage numbers or notifications. It spawns a <c>TextMeshPro</c> object, animates it upward with fade-in/out transitions, then destroys it after completion.
    /// </summary>
    public class FloatingText : MonoBehaviour
    {
        private TMP_Text tmp;

        /// <summary>
        /// Create an animated floating text object at a given position.
        /// </summary>
        /// <param name="text">Text to display.</param>
        /// <param name="position">Position to start the animation.</param>
        /// <param name="fontSize">Size of the font to use.</param>
        /// <param name="duration">Time to hold once the animation is done.</param>
        /// <param name="animationDuration">Time to perform the fade in/out animations.</param>
        /// <param name="offset">Height to float to from the offset.</param>
        /// <param name="rotation">Maximum offset for rotation.</param>
        /// <param name="sortingLayer">Sorting layer value for the SortingGroup component.</param>
        public static void Instantiate(
            string text,
            Vector3 position,
            float fontSize = 8,
            float duration = 0.5f,
            float animationDuration = 0.25f,
            float offset = 0.5f,
            float rotation = 10f,
            int sortingLayer = 10,
            Action<TMP_Text> tmpSetup = null)
        {
            var obj = new GameObject();
            obj.transform.position = position;
            obj.transform.Rotate(new Vector3(0, 0, MathfExtensions.signedValue * rotation));

            var fp = obj.AddComponent<FloatingText>();

            fp.tmp = obj.AddComponent<TextMeshPro>();
            fp.tmp.text = text;
            fp.tmp.fontSize = fontSize;
            fp.tmp.font = UnityAssetsSettings.GetOrCreate().floatingTextFont;
            fp.tmp.enableWordWrapping = false;
            fp.tmp.overflowMode = TextOverflowModes.Overflow;
            fp.tmp.alignment = TextAlignmentOptions.Center;
            tmpSetup?.Invoke(fp.tmp);

            var rt = obj.transform as RectTransform;
            rt.sizeDelta = Vector2.zero;

            var sortingGroup = obj.AddComponent<SortingGroup>();
            sortingGroup.sortingOrder = sortingLayer;

            fp.StartCoroutine(fp.Animation(duration, animationDuration, offset));
        }

        private IEnumerator Animation(float duration, float animationDuration, float offset)
        {
            // Fade in and move up
            var basePos = transform.position;
            var offsetPos = basePos + (Vector2.up * offset).ToVector3();

            yield return this.Tween(
                duration: animationDuration,
                onUpdate: t =>
                {
                    transform.position = Vector3.Lerp(basePos, offsetPos, t);
                    tmp.alpha = Mathf.Lerp(0f, 1f, t);
                },
                curve: Curves.In
            );

            // Hold position
            yield return new WaitForSeconds(duration);

            // Fade out
            yield return this.Tween(
                duration: animationDuration,
                onUpdate: t =>
                {
                    tmp.alpha = Mathf.Lerp(1f, 0f, t);
                },
                curve: Curves.In
            );

            Destroy(gameObject);
        }
    }
}