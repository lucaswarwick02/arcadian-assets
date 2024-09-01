using UnityEditor;

namespace Arcadian.System
{
    public static class ArcadianAssetsConfig
    {
        public static string FloatingTextPath
        {
            get => EditorPrefs.GetString("ArcadianAssets_FloatingTextPath", "");
            set => EditorPrefs.SetString("ArcadianAssets_FloatingTextPath", value);
        }

        public static string TransitionEffectPath
        {
            get => EditorPrefs.GetString("ArcadianAssets_TransitionEffectPath", "");
            set => EditorPrefs.SetString("ArcadianAssets_TransitionEffectPath", value);
        }
    }
}