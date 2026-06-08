using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace LucasWarwick02.UnityAssets
{
    /// <summary>
    /// Detects the platform type once at startup, then tracks the active input
    /// scheme based on the most recent real button press.
    ///
    /// PlatformType is set once and never changes. InputScheme changes at runtime
    /// and is persisted via PlayerPrefs.
    ///
    /// Uses InputSystem.onEvent to avoid InputActionChange overhead and analog noise.
    /// Persists across scenes and auto-instantiates at application startup.
    /// </summary>
    public sealed class InputManager : MonoBehaviour
    {
        const string PREF_KEY = "InputManager.InputScheme";

        /// <summary>
        /// The platform/hardware category this session is running on.
        /// </summary>
        public static PlatformType Platform { get; private set; }

        /// <summary>
        /// The input scheme the player is currently using.
        /// </summary>
        public static InputScheme CurrentScheme { get; private set; }

        /// <summary>
        /// Fired whenever the player switches between gamepad and mouse/keyboard.
        /// </summary>
        public static event Action SchemeChanged;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
            Platform = DetectPlatform();
            CurrentScheme = LoadScheme();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSceneLoad()
        {
            // Debug.Log($"[InputManager] Detected platform: {Platform}, starting scheme: {CurrentScheme}");

            InputSystem.onEvent += OnInputEvent;

#if UNITY_EDITOR
            UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += Cleanup;
#endif
        }

        /// <summary>
        /// Identifies the platform category from SystemInfo and Application.platform.
        /// Only runs once at startup.
        /// </summary>
        private static PlatformType DetectPlatform()
        {
            string model = SystemInfo.deviceModel.ToLower();

            return model switch
            {
                // --- Valve ---
                var m when m.Contains("jupiter") || m.Contains("steam deck")
                    => PlatformType.HandheldPC,

                // --- ASUS ROG ---
                var m when m.Contains("rog ally")
                    => PlatformType.HandheldPC,

                // --- Lenovo ---
                var m when m.Contains("legion go")
                    => PlatformType.HandheldPC,

                // --- MSI ---
                var m when m.Contains("msi claw")
                    => PlatformType.HandheldPC,

                // --- GPD ---
                var m when m.Contains("gpd win")
                    => PlatformType.HandheldPC,

                // --- AYANEO ---
                var m when m.Contains("ayaneo") || m.Contains("aya neo")
                    => PlatformType.HandheldPC,

                // --- Consoles ---
                var m when m.Contains("playstation") || m.Contains("orbis")
                    => PlatformType.Console,
                var m when m.Contains("xbox") || m.Contains("scorpio")
                    => PlatformType.Console,

                // --- Mobile ---
                _ when Application.platform == RuntimePlatform.Android
                    => PlatformType.Mobile,
                _ when Application.platform == RuntimePlatform.IPhonePlayer
                    => PlatformType.Mobile,

                _ => PlatformType.Desktop
            };
        }

        // -------------------------------------------------------------------------
        // Scheme Persistence
        // -------------------------------------------------------------------------

        /// <summary>
        /// Loads the last used scheme from PlayerPrefs, or infers a sensible
        /// default from the detected platform on first launch.
        /// </summary>
        private static InputScheme LoadScheme()
        {
            if (PlayerPrefs.HasKey(PREF_KEY))
                return (InputScheme)PlayerPrefs.GetInt(PREF_KEY);

            // First launch — pick a default based on platform
            var defaultScheme =  Platform switch
            {
                PlatformType.HandheldPC => InputScheme.Gamepad,
                PlatformType.Console    => InputScheme.Gamepad,
                PlatformType.Mobile     => InputScheme.MouseKeyboard, // extend if you add touch
                _                       => InputScheme.MouseKeyboard
            };

            SaveScheme(defaultScheme);
            return defaultScheme;
        }

        private static void SaveScheme(InputScheme scheme)
        {
            PlayerPrefs.SetInt(PREF_KEY, (int)scheme);
            PlayerPrefs.Save();
        }

        // -------------------------------------------------------------------------
        // Runtime Tracking
        // -------------------------------------------------------------------------

        private static void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
        {
            if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
                return;

            var newScheme = device is Gamepad
                ? InputScheme.Gamepad
                : InputScheme.MouseKeyboard;

            if (newScheme == CurrentScheme)
                return;

            CurrentScheme = newScheme;
            SaveScheme(CurrentScheme);
            SchemeChanged?.Invoke();
        }

        private static void Cleanup()
        {
            InputSystem.onEvent -= OnInputEvent;
        }
    }
}