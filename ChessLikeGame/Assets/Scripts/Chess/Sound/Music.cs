using UnityEngine;

namespace Chess.Sound
{
    public class Music : MonoBehaviour
    {
        
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] audioClips;
        private int _currentClip = -1;
        private float _volume = 1;
        private bool _volumeOn = true;

        public void TurnVolumeDown()
        {
            if (_volumeOn) audioSource.volume -= 0.1f;
            
        }
        public void TurnVolumeUp()
        {
            if (_volumeOn) audioSource.volume += 0.1f;
        }
        public void TurnVolumeOnOff(bool on)
        {
            if (on)
            {
                audioSource.volume = _volume;
            }
            else
            {
                _volume = audioSource.volume;
                audioSource.volume = 0f;
            }
            _volumeOn = on;
        }
        
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