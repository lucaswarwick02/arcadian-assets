using Arcadian.System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

namespace Arcadian.Sound
{
    [CreateAssetMenu(menuName = "Misc/Sound Effect", fileName = "New Sound Effect")]
    public class SoundEffect : ScriptableObject
    {
        [field: SerializeField] public AudioClip Clip { private set; get; }
        [field: SerializeField] public AudioMixerGroup MixerGroup { private set; get; }

        public void Play(float? clipLength = null)
        {
            Addressables.InstantiateAsync(
                        ArcadianAssets.Config.SoundEffectInstancePath,
                        Vector3.zero,
                        Quaternion.identity)
                    .Completed +=
                handle =>
                {
                    // Get the instance
                    var soundEffectInstance = handle.Result.GetComponent<SoundEffectInstance>();

                    // Attach the Variables
                    soundEffectInstance.SetClip(Clip);
                    soundEffectInstance.SetMixerGroup(MixerGroup);
                    if (clipLength != null) soundEffectInstance.SetClipLength(clipLength.Value);

                    // Play
                    soundEffectInstance.Play();
                };
        }
    }
}