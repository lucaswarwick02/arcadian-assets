using UnityEngine;
using UnityEngine.Audio;

namespace Arcadian.Sound
{
    [CreateAssetMenu(menuName = "Misc/Sound Effect", fileName = "New Sound Effect")]
    public class SoundEffect : ScriptableObject
    {
        [field: SerializeField] public SoundEffectInstance Prefab { set; get; }
        [field: SerializeField] public AudioClip Clip { private set; get; }
        [field: SerializeField] public AudioMixerGroup MixerGroup { private set; get; }

        public void Play(float? clipLength = null)
        {
            // Instantiate the standalone object
            var audioSource = Instantiate(Prefab);

            // Attach the Variables
            audioSource.SetClip(Clip);
            audioSource.SetMixerGroup(MixerGroup);
            if (clipLength != null) audioSource.SetClipLength(clipLength.Value);

            // Play
            audioSource.Play();
        }
    }
}