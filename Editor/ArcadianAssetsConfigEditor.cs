using Arcadian.System;
using UnityEditor;

namespace Editor
{
#if UNITY_EDITOR
    [CustomEditor(typeof(ArcadianAssetsConfig))]
    public class ArcadianAssetsConfigEditor : EditorWindow
    {
        [MenuItem("Arcadian Assets/Package Configuration")]
        public static void ShowWindow()
        {
            GetWindow<ArcadianAssetsConfigEditor>("Package Config");
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            var newPrefabPath = EditorGUILayout.TextField("Floating Text Path", ArcadianAssetsConfig.FloatingTextPath);
            // Add fields for other configuration variables

            if (EditorGUI.EndChangeCheck())
            {
                ArcadianAssetsConfig.FloatingTextPath = newPrefabPath;
                // Set other configuration values
            }
        }
    }
#endif
}