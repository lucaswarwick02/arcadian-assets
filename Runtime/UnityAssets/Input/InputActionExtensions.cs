using UnityEngine.InputSystem;

namespace LucasWarwick02.UnityAssets
{
    /// <summary>
    /// Extension methods for InputAction.
    /// </summary>
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
                    continue;

                if (!IsBindingCompatible(binding))
                    continue;

                return action.GetBindingDisplayString(i);
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns true if the binding's path matches any currently connected device.
        /// </summary>
        private static bool IsBindingCompatible(InputBinding binding)
        {
            if (string.IsNullOrEmpty(binding.effectivePath))
                return false;

            foreach (var device in InputSystem.devices)
            {
                if (InputControlPath.Matches(binding.effectivePath, device))
                    return true;
            }

            return false;
        }
    }
}