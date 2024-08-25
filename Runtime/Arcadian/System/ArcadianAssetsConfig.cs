using UnityEditor;

namespace Arcadian.System
{
    public static class ArcadianAssetsConfig
    {
        public static string FloatingTextPath
        {
            get => EditorPrefs.GetString("PackageConfig_FloatingTextPath", "");
            set => EditorPrefs.SetString("PackageConfig_FloatingTextPath", value);
        }
    }
}