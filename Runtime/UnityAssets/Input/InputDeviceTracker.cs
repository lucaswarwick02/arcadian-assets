using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

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
    [RequireComponent(typeof(PlayerInput))]
    public sealed class InputDeviceTracker : MonoBehaviour
    {
        /// <summary>
        /// High-level classification of supported input device types.
        /// </summary>
        public enum InputDeviceType
        {
            KeyboardMouse,
            Gamepad,
        }

        /// <summary>
        /// The most recently used input device type.
        /// </summary>
        public static InputDeviceType CurrentDevice { get; private set; }

        /// <summary>
        /// The currently active PlayerInput control scheme.
        /// </summary>
        public static string CurrentScheme { get; private set; }

        /// <summary>
        /// Raised whenever the active input device type changes.
        /// </summary>
        public static event Action<InputDeviceType> DeviceChanged;

        private static InputDeviceTracker _instance;
        private static PlayerInput _playerInput;

        // Disposable returned by onAnyButtonPress.Subscribe(...)
        private static IDisposable _anyButtonPressSubscription;

        private static bool HasInstance => _instance != null && _instance;

        /// <summary>
        /// Ensures the tracker exists before any scene loads.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            if (HasInstance)
                return;

            if (UnityAssetsSettings.GetOrCreate().inputActionsType == null)
                return;

            var go = new GameObject("[Lucas's Unity Assets] Input Device Tracker");
            _instance = go.AddComponent<InputDeviceTracker>();
            DontDestroyOnLoad(go);
        }

        private void Awake()
        {
            if (HasInstance && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            // Cached once — never queried from hot paths
            _playerInput = FindObjectOfType<PlayerInput>();
            _playerInput.actions = UnityAssetsSettings.GetOrCreate().inputActionsType;

            SetInitialDevice();

            // Use CallOnce() instead of Subscribe() - it accepts a callback
            _anyButtonPressSubscription =
                InputSystem.onAnyButtonPress.CallOnce(OnAnyButtonPress);
        }

        /// <summary>
        /// Called once per real button press on any device.
        /// Allocation-free and immune to analog noise.
        /// </summary>
        private static void OnAnyButtonPress(InputControl control)
        {
            var device = control.device;

            if (device is Gamepad)
            {
                if (CurrentDevice != InputDeviceType.Gamepad)
                    SetDevice(InputDeviceType.Gamepad);
            }
            else if (device is Keyboard || device is Mouse)
            {
                if (CurrentDevice != InputDeviceType.KeyboardMouse)
                    SetDevice(InputDeviceType.KeyboardMouse);
            }
            
            // Re-subscribe for the next button press
            _anyButtonPressSubscription?.Dispose();
            _anyButtonPressSubscription =
                InputSystem.onAnyButtonPress.CallOnce(OnAnyButtonPress);
}

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;

            _anyButtonPressSubscription?.Dispose();
            _anyButtonPressSubscription = null;
        }

        /// <summary>
        /// Sets the initial device based on currently connected hardware.
        /// </summary>
        private static void SetInitialDevice()
        {
            if (Gamepad.current != null)
            {
                SetDevice(InputDeviceType.Gamepad);
                return;
            }

            if (Keyboard.current != null || Mouse.current != null)
            {
                SetDevice(InputDeviceType.KeyboardMouse);
            }
        }

        /// <summary>
        /// Updates the active device and control scheme.
        /// Invokes DeviceChanged only on actual transitions.
        /// </summary>
        private static void SetDevice(InputDeviceType device)
        {
            CurrentDevice = device;

            // PlayerInput is authoritative for scheme resolution
            if (_playerInput != null)
                CurrentScheme = _playerInput.currentControlScheme;

            DeviceChanged?.Invoke(device);
        }
    }

    public static class InputActionExtensions
    {
        /// <summary>
        /// Gets the display string for the binding associated with the
        /// currently active control scheme.
        /// </summary>
        public static string GetBindingString(this InputAction inputAction)
        {
            var scheme = InputDeviceTracker.CurrentScheme;
            if (string.IsNullOrEmpty(scheme))
                return string.Empty;

            var bindings = inputAction.bindings;
            for (int i = 0; i < bindings.Count; i++)
            {
                if (bindings[i].groups == scheme)
                    return bindings[i].ToDisplayString();
            }

            return string.Empty;
        }
    }
}
