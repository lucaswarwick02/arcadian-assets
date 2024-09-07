using UnityEngine;

namespace Arcadian.System
{
    public static class ArcadianAssets
    {
        private static ArcadianAssetsConfig _config;

        public static ArcadianAssetsConfig Config
        {
            get
            {
                if (_config) return _config;
                
                _config = Resources.Load<ArcadianAssetsConfig>("ArcadianAssetsConfig");
                if (!_config)
                {
                    Debug.LogError("ArcadianAssetsConfig not found. Please create one in the Resources folder.");
                }
                
                return _config;
            }
        }
    }
}