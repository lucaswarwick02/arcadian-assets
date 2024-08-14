using UnityEngine;

namespace Arcadian.UI
{
    /// <summary>
    /// Helper class to position UI elements in world space.
    /// </summary>
    public class WorldSpaceUI : MonoBehaviour
    {
        /// <summary>
        /// World position to place the UI element.
        /// </summary>
        public Vector3 WorldPosition { get; set; }

        private static Camera _mainCamera;

        private void Update()
        {
            _mainCamera ??= Camera.main;
            
            if (!_mainCamera) return;
            
            transform.position = _mainCamera.WorldToScreenPoint(WorldPosition);
        }
    }
}