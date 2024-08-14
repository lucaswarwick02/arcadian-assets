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
        [SerializeField] private RectTransform content;

        private const float AutoScrollSpeed = 10f;

        private Coroutine _autoScroll;

        private IEnumerator AutoScroll(Vector3 targetLocalPos)
        {
            while (Vector3.Distance(content.localPosition, targetLocalPos) > 0.1f)
            {
                content.localPosition =
                    Vector3.Lerp(content.localPosition, targetLocalPos, Time.unscaledDeltaTime * AutoScrollSpeed);
                
                yield return null;
            }

            content.localPosition = targetLocalPos;
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

            var offset = topView - content.localPosition.y;

            // Below rect
            if (bottomObj - offset < bottomView)
            {
                var diff = -(bottomObj - offset - bottomView);
                var contentLocalPos = content.localPosition;
                contentLocalPos.y += diff;
                
                if (_autoScroll != null) StopCoroutine(_autoScroll);
                _autoScroll = StartCoroutine(AutoScroll(contentLocalPos));
            }

            // Above Rect
            if (topObj - offset > topView)
            {
                var diff = -(topObj - offset - topView);
                
                var contentLocalPos = content.localPosition;
                contentLocalPos.y += diff;
                
                if (_autoScroll != null) StopCoroutine(_autoScroll);
                _autoScroll = StartCoroutine(AutoScroll(contentLocalPos));
            }
        }
    }
}