using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LucasWarwick02.UnityAssets
{
    /// <summary>
    /// A utility for smooth scene loading with fade an optional prefab-based animations. It overlays a full-screen canvas, fades to black, loads the new scene, optionally plays a custom transition prefab implementing <c>ISceneTransitionAnimation</c>, then fades back out. The static <c>LoadScene</c> methods support both scene names and built indices for flexible use.
    /// </summary>
    public class SceneTransition : MonoBehaviour
    {
        public static event Action OnSceneTransitionEnd;
        public static bool IsTransitioning { get; private set; }

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public static void LoadScene(string sceneName, bool usePrefab = true)
        {
            var sceneTransition = new GameObject { name = "[Lucas's Unity Assets] Scene Transition" }.AddComponent<SceneTransition>();
            sceneTransition.StartCoroutine(sceneTransition.Animation(() => SceneManager.LoadScene(sceneName), usePrefab));
        }

        public static void LoadScene(int sceneID, bool usePrefab = true)
        {
            var sceneTransition = new GameObject { name = "[Lucas's Unity Assets] Scene Transition" }.AddComponent<SceneTransition>();
            sceneTransition.StartCoroutine(sceneTransition.Animation(() => SceneManager.LoadScene(sceneID), usePrefab));
        }

        public IEnumerator Animation(Action changeScene, bool usePrefab)
        {
            IsTransitioning = true;

            // Add a canvas it counts as a UI
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;
            gameObject.AddComponent<GraphicRaycaster>();

            var canvasGroup = gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            gameObject.AddComponent<Image>().color = Color.black;

            // Fade image in
            yield return this.Tween(duration: 0.5f, onUpdate: (t) => canvasGroup.alpha = Mathf.Lerp(0, 1, t), useUnscaledTime: true);

            // Change scene
            changeScene.Invoke();

            // Show user 
            yield return this.Tween(duration: 0.5f, useUnscaledTime: true);

            // Optional: Custom user animation
            GameObject prefab;
            if (usePrefab && (prefab = UnityAssetsSettings.GetOrCreate().sceneTransitionPrefab) != null)
            {
                var child = Instantiate(prefab, transform);
                if (child.TryGetComponent<ISceneTransitionAnimation>(out var anim)) yield return anim.Play();
            }

            // Fade image out
            yield return this.Tween(duration: 0.5f, onUpdate: (t) => canvasGroup.alpha = Mathf.Lerp(1, 0, t), useUnscaledTime: true);

            IsTransitioning = false;
            Destroy(gameObject);
            OnSceneTransitionEnd?.Invoke();
        }
    }

    /// <summary>
    /// An interface for defining custom scene transition animations used by <c>SceneTransition</c>. Implementations should perform visual effects (like text pop-ups) in a coroutine, returning control once complete.
    /// </summary>
    public interface ISceneTransitionAnimation
    {
        /// <summary>
        /// Animation to run in between scenes.
        /// </summary>
        /// <returns></returns>
        public IEnumerator Play();
    }
}