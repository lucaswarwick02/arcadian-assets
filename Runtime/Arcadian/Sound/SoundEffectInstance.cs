using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Arcadian.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEffectInstance : MonoBehaviour
    {
        private AudioSource _audioSource;
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void SetClip(AudioClip audioClip)
        {
            _audioSource.clip = audioClip;
        }

        public void SetMixerGroup(AudioMixerGroup audioMixerGroup)
        {
            _audioSource.outputAudioMixerGroup = audioMixerGroup;
        }

        public void SetClipLength(float targetLength)
        {
            _audioSource.pitch = _audioSource.clip.length / targetLength;
        }

        public void Play()
        {
            _audioSource.Play();

            StartCoroutine(LifetimeCheck());
        }

        private IEnumerator LifetimeCheck()
        {
            while (_audioSource.isPlaying)
            {
                yield return null;
            }
            
            // Clip has finished
            Destroy(_audioSource.gameObject);
        }
    }
}