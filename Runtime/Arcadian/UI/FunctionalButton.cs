using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Arcadian.UI
{
    public abstract class FunctionalButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        public abstract void OnButtonHover();
        public abstract void OnButtonHoverEnd();

        public abstract void OnButtonPress([CanBeNull] PointerEventData eventData = null);

        public bool IsFocused { private set; get;}
        
        /// <summary>
        /// Use this callback to detect pointer enter events
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            IsFocused = true;
            OnButtonHover();
        }

        public void OnSelect(BaseEventData eventData)
        {
            IsFocused = true;
            OnButtonHover();
        }

        /// <summary>
        /// Use this callback to detect pointer exit events
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            IsFocused = false;
            OnButtonHoverEnd();
        }

        /// <summary>
        /// Called by the EventSystem when a new object is being selected.
        /// </summary>
        public void OnDeselect(BaseEventData eventData)
        {
            IsFocused = false;
            OnButtonHoverEnd();
        }

        /// <summary>
        /// Use this callback to detect clicks.
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            OnButtonPress(eventData);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            OnButtonPress();
        }
    }
}