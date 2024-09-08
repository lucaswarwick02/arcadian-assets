using System.Collections;
using Arcadian.Extensions;
using Arcadian.Sound;
using UnityEngine;

namespace Arcadian.UI
{
    public abstract class AbstractUI : MonoBehaviour
    {
        [Header("Abstract UI")]
        [SerializeField] private SoundEffect openSound;
        [SerializeField] private SoundEffect closeSound;
        [Space]

        private const float SizeMultiplier = 1.075f;
        private const float AnimationLength = 0.125f;

        private Coroutine _animation;

        public bool IsOpen => gameObject.activeSelf;

        public bool IsClosed => !gameObject.activeSelf;
    
        public virtual void Open()
        {
            if (IsOpen) return;
            
            gameObject.SetActive(true);
            if (openSound) openSound.Play();
            _animation = StartCoroutine(OpenAnimation());
            
            this.RunEndOnFrame(AfterOpen);
        }

        private IEnumerator OpenAnimation()
        {
            var timer = 0f;

            while (timer < AnimationLength)
            {
                timer += Time.unscaledDeltaTime;
                
                var percentage = Mathf.Sin(timer / AnimationLength * Mathf.PI);
                transform.localScale = Vector3.one * Mathf.Lerp(1f, SizeMultiplier, percentage);

                yield return null;
            }

            transform.localScale = Vector3.one;
        }
    
        public virtual void Close()
        {
            if (IsClosed) return;
            
            if (closeSound) closeSound.Play();
            if (_animation != null) StopCoroutine(_animation);
            transform.localScale = Vector3.one;
            gameObject.SetActive(false);
        }

        public virtual void AfterOpen()
        {
            
        }
    }
}
