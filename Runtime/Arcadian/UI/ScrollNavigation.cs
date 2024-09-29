using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Arcadian.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollNavigation : MonoBehaviour
    {
        [SerializeField] private LayoutGroup layoutGroup;
        [SerializeField] private RectTransform viewport;
        [SerializeField] private ScrollNavigationContent scrollNavigationContent;

        private const float AutoScrollSpeed = 10f;

        private Coroutine _autoScroll;
        
        private RectTransform Content => scrollNavigationContent.transform as RectTransform;

        private void Awake()
        {
            if (!scrollNavigationContent)
            {
                Debug.LogError("You must also attach and set the ScrollNavigationContent script to the main content of the ScrollRect.");
            }
        }

        private IEnumerator AutoScroll(Vector3 targetLocalPos)
        {
            while (Vector3.Distance(Content.localPosition, targetLocalPos) > 0.1f)
            {
                Content.localPosition =
                    Vector3.Lerp(Content.localPosition, targetLocalPos, Time.unscaledDeltaTime * AutoScrollSpeed);
                
                yield return null;
            }

            Content.localPosition = targetLocalPos;
            _autoScroll = null;
        }

        public void Select(GameObject selectedGameObject)
        {
            var rectTransform = selectedGameObject.transform as RectTransform;
            if (!rectTransform) return;

            var localPos = rectTransform.localPosition;
            var topObj = localPos.y + ((1 - rectTransform.pivot.y) * rectTransform.rect.height);
            var bottomObj = localPos.y - (rectTransform.pivot.y * rectTransform.rect.height);

            if (layoutGroup)
            {
                topObj += layoutGroup.padding.top;
                bottomObj -= layoutGroup.padding.bottom;
            }

            var topView = 0;
            var bottomView = topView - viewport.rect.height;

            var offset = topView - Content.localPosition.y;

            // Below rect
            if (bottomObj - offset < bottomView)
            {
                var diff = -(bottomObj - offset - bottomView);
                var contentLocalPos = Content.localPosition;
                contentLocalPos.y += diff;
                
                if (_autoScroll != null) StopCoroutine(_autoScroll);
                _autoScroll = StartCoroutine(AutoScroll(contentLocalPos));
            }

            // Above Rect
            if (topObj - offset > topView)
            {
                var diff = -(topObj - offset - topView);
                
                var contentLocalPos = Content.localPosition;
                contentLocalPos.y += diff;
                
                if (_autoScroll != null) StopCoroutine(_autoScroll);
                _autoScroll = StartCoroutine(AutoScroll(contentLocalPos));
            }
        }
    }
}