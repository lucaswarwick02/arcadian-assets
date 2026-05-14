using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace LucasWarwick02.UnityAssets
{
    /// <summary>
    /// Tracks the user's currently active input device (keyboard/mouse or gamepad)
    /// based on the most recent real button press.
    ///
    /// Uses InputSystem.onAnyButtonPress to avoid InputActionChange overhead
    /// and analog noise.
    ///
    /// Persists across scenes and auto-instantiates at application startup.
    /// </summary>
    public sealed class InputDeviceTracker : MonoBehaviour
    {
        public static bool IsUsingGamepad;

        public static event Action DeviceChanged;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            InputSystem.onEvent += OnInputEvent;

#if UNITY_EDITOR
        UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += Cleanup;
#endif
        }

        private static void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
        {
            if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
            {
                return;
            }

            var newState = device is Gamepad;

            if (newState == IsUsingGamepad)
            {
                return;
            }

            IsUsingGamepad = newState;
            DeviceChanged?.Invoke();
        }

        private static void Cleanup()
        {
            InputSystem.onEvent -= OnInputEvent;
        }
    }

    public static class InputActionExtensions
    {
        /// <summary>
        /// Gets the display string for the binding associated with the
        /// currently active control scheme.
        /// </summary>
        public static string GetBindingString(this InputAction action)
        {
            for (int i = 0; i < action.bindings.Count; i++)
            {
                var binding = action.bindings[i];

                if (binding.isComposite || binding.isPartOfComposite)
                {
                    continue;
                }

                if (!IsBindingCompatible(binding))
                {
                    continue;
                }

                return action.GetBindingDisplayString(i);
            }

            return string.Empty;
        }

        private static bool IsBindingCompatible(InputBinding binding)
        {
            if (string.IsNullOrEmpty(binding.effectivePath))
            {
                return false;
            }

            foreach (var device in InputSystem.devices)
            {
                if (InputControlPath.Matches(binding.effectivePath, device))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
