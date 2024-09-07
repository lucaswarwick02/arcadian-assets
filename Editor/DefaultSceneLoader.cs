using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.SceneManagement;

    [InitializeOnLoad]
    public static class DefaultSceneLoader
    {
        private const string ToggleKey = "DefaultSceneLoaderEnabled";

        static DefaultSceneLoader()
        {
            if (EditorPrefs.GetBool(ToggleKey, true))
            {
                EditorApplication.playModeStateChanged += LoadDefaultScene;
            }
        }

        [MenuItem("Arcadian Assets/Toggle Default Scene Loader %#d", false, 1)]
        private static void ToggleDefaultSceneLoader()
        {
            var isEnabled = EditorPrefs.GetBool(ToggleKey, true);
            isEnabled = !isEnabled;
            EditorPrefs.SetBool(ToggleKey, isEnabled);

            if (isEnabled)
            {
                EditorApplication.playModeStateChanged += LoadDefaultScene;
                Debug.Log("Default Scene Loader enabled");
            }
            else
            {
                EditorApplication.playModeStateChanged -= LoadDefaultScene;
                Debug.Log("Default Scene Loader disabled");
            }
            
            Menu.SetChecked("Arcadian Assets/Toggle Default Scene Loader", isEnabled);
        }

        [MenuItem("Tools/Toggle Default Scene Loader", true)]
        private static bool ToggleDefaultSceneLoaderValidate()
        {
            Menu.SetChecked("Tools/Toggle Default Scene Loader", EditorPrefs.GetBool(ToggleKey, true));
            return true;
        }

        private static void LoadDefaultScene(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            }

            if (state != PlayModeStateChange.EnteredPlayMode) return;
            // Clear the console log
            ClearConsole();

            // Load the default scene
            SceneManager.LoadScene(0);
        }

        private static void ClearConsole()
        {
            // This uses reflection to clear the console
            var assembly = System.Reflection.Assembly.GetAssembly(typeof(SceneView));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            if (method != null) method.Invoke(new object(), null);
        }
    }
#endif
}