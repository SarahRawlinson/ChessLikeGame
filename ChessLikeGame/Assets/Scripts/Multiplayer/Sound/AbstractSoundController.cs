using System.Globalization;
using UnityEngine;

namespace Multiplayer.Sound
{
    public abstract class AbstractSoundController : MonoBehaviour
    {
        [SerializeField] internal AudioSource audioSource;
        private float _volume = 1;
        private bool _volumeOn = true;

        public string GetVolume()
        {
            return audioSource.volume.ToString(CultureInfo.CurrentCulture);
        }
        
        public string GetOn()
        {
            return _volumeOn?"On":"Off";
        }

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

    }
}