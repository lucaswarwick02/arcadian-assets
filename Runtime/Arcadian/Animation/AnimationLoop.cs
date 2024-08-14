using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Arcadian.Animation
{
    /// <summary>
    /// Contains a single animation and loops it.
    /// </summary>
    public class AnimationLoop : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public Image image;
        public float frameDelay = 0.125f;
        public Sprite[] sprites;
        
        public bool destroyOnFinish;
        public bool playOnStart;
        public bool loop;
        public bool useUnscaledTime;

        public event Action onAnimationFinished;

        private void Awake()
        {
            if (playOnStart) SetSprite(sprites[0]);
        }

        private void Start()
        {
            if (playOnStart) Play();
        }

        /// <summary>
        /// Starts the PlayCoroutine.
        /// </summary>
        [ContextMenu("Play")]
        public void Play()
        {
            StartCoroutine(PlayCoroutine());
        }

        private IEnumerator PlayCoroutine()
        {
            foreach (var t in sprites)
            {
                SetSprite(t);
                if (useUnscaledTime)
                {
                    yield return new WaitForSecondsRealtime(frameDelay);
                }
                else
                {
                    yield return new WaitForSeconds(frameDelay);
                }
            }

            onAnimationFinished?.Invoke();
            
            if (destroyOnFinish) Destroy(gameObject);
            if (loop) StartCoroutine(PlayCoroutine());
        }
        
        private void SetSprite(Sprite sprite)
        {
            if (spriteRenderer) spriteRenderer.sprite = sprite;
            if (image) image.sprite = sprite;
        }
    }
}
