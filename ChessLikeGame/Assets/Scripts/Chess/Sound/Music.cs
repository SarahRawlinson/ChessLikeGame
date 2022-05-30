using UnityEngine;

namespace Chess.Sound
{
    public class Music : MonoBehaviour
    {
        
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] audioClips;
        private int _currentClip = -1;

        private void Update()
        {
            if (!audioSource.isPlaying)
            {
                PlayNextClip();
            }
        }

        private void PlayNextClip()
        {
            _currentClip += 1;
            if (_currentClip > audioClips.Length -1 || _currentClip < 0)
            {
                _currentClip = 0;
            }
            audioSource.clip = audioClips[_currentClip];
            audioSource.Play();
        }
    }
}