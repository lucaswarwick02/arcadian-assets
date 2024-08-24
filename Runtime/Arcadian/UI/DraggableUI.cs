using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Arcadian.UI
{
    public abstract class DraggableUI<T> : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler where T : MonoBehaviour
    {
        [Header("Draggable UI")]
        [SerializeField] private bool snapBack = true;
        [Space]
        
        private RectTransform _rectTransform;
        private Canvas _canvas;
        private Camera _camera;
        
        private Vector3 _origin;
        private readonly Collider2D[] _overlapColliders = new Collider2D[10];
        
        protected T Target { private set; get; }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();
            _camera = Camera.main;

            _origin = _rectTransform.localPosition;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Target = null;
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform,
                eventData.position,
                _canvas.worldCamera,
                out var position);

            _rectTransform.position = _canvas.transform.TransformPoint(position);
            
            var target = ClosestTarget();
            if (target != Target)
            {
                if (Target) OnTargetExit(Target);
                if (target) OnTargetEnter(target);

                Target = target;
            }
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            if (Target)
            {
                OnTargetDrop(Target);
                OnTargetExit(Target);
            }
            
            Target = null;
            
            if (snapBack) _rectTransform.localPosition = _origin;
        }

        private T ClosestTarget()
        {
            var worldPos = _camera.ScreenToWorldPoint(_rectTransform.position);
            var size = Physics2D.OverlapCircleNonAlloc(worldPos, 0.5f, _overlapColliders);
            
            var targets = _overlapColliders.Take(size).Select(c => c.GetComponent<T>()).Where(t => t != null && IsValidTarget(t));

            T closest = null;
            var closestDistance = Mathf.Infinity;

            foreach (var target in targets)
            {
                var distance = Vector2.Distance(worldPos, target.transform.position);
                if (!(distance < closestDistance)) continue;
                closest = target;
                closestDistance = distance;
            }

            return closest;
        }

        protected abstract bool IsValidTarget(T target);
        
        protected abstract void OnTargetEnter(T target);
        
        protected abstract void OnTargetExit(T target);

        protected abstract void OnTargetDrop(T target);
    }
}