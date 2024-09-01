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

            var newFloatingTextPath = EditorGUILayout.TextField("Floating Text Path", ArcadianAssetsConfig.FloatingTextPath);
            var newTransitionEffectPath = EditorGUILayout.TextField("Transition Effect Path", ArcadianAssetsConfig.TransitionEffectPath);

            if (EditorGUI.EndChangeCheck())
            {
                ArcadianAssetsConfig.FloatingTextPath = newFloatingTextPath;
                ArcadianAssetsConfig.TransitionEffectPath = newTransitionEffectPath;
            }
        }
    }
#endif
}