using UnityEngine;

namespace Arcadian.System
{
    [CreateAssetMenu(fileName = "ArcadianAssetsConfig", menuName = "Arcadian/Assets Config")]
    public class ArcadianAssetsConfig : ScriptableObject
    {
        [field: SerializeField] public string FloatingTextPath { private set; get; }
        [field: SerializeField] public string TransitionEffectPath { private set; get; }
        [field: SerializeField] public string SoundEffectInstancePath { private set; get; }
    }
}