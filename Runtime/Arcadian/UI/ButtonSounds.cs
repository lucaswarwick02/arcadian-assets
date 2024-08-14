using System;
using Arcadian.Sound;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Arcadian.UI
{
    public class ButtonSounds : MonoBehaviour, ISelectHandler, IPointerClickHandler, ISubmitHandler, IPointerEnterHandler
    {
        [SerializeField] private SoundEffect selectSound;
        [SerializeField] private SoundEffect pressSound;

        /// <summary>
        /// Gamepad - Select.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnSelect(BaseEventData eventData)
        {
            if (selectSound) selectSound.Play();
        }
        
        /// <summary>
        /// Gamepad - Submit.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnSubmit(BaseEventData eventData)
        {
            if (pressSound) pressSound.Play();
        }
        
        /// <summary>
        /// Mouse - Select.
        /// </summary>
        /// <param name="eventData"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (selectSound) selectSound.Play();
        }

        /// <summary>
        /// Mouse - Submit.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (pressSound) pressSound.Play();
        }
    }
}