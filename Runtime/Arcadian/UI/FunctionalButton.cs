using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public abstract class FunctionalButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        public abstract void OnButtonHover();
        public abstract void OnButtonHoverEnd();

        public abstract void OnButtonPress();
        
        /// <summary>
        /// Use this callback to detect pointer enter events
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            OnButtonHover();
        }

        /// <summary>
        /// Use this callback to detect pointer exit events
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            OnButtonHoverEnd();
        }

        public void OnSelect(BaseEventData eventData)
        {
            OnButtonHover();
        }

        /// <summary>
        /// Called by the EventSystem when a new object is being selected.
        /// </summary>
        public void OnDeselect(BaseEventData eventData)
        {
            OnButtonHoverEnd();
        }

        /// <summary>
        /// Use this callback to detect clicks.
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            OnButtonPress();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            OnButtonPress();
        }
    }
}