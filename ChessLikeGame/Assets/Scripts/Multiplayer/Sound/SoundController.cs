using System;
using Multiplayer.Sound;
using TMPro;
using UnityEngine;

namespace Chess.Sound
{
    public class SoundController : MonoBehaviour
    {
        [SerializeField] private Music music;
        [SerializeField] private TMP_Text musicVolumeText;
        [SerializeField] private TMP_Text musicOnText;
        [SerializeField] private SoundEffects effects;
        [SerializeField] private TMP_Text effectsVolumeText;
        [SerializeField] private TMP_Text effectsOnText;

        private void Awake()
        {
            musicVolumeText.text = music.GetVolume();
            musicOnText.text = music.GetOn();
            effectsVolumeText.text = effects.GetVolume();
            effectsOnText.text = effects.GetOn();
        }

        public void TurnVolumeDownMusic()
        {
            music.TurnVolumeDown();
            musicVolumeText.text = music.GetVolume();
        }
        public void TurnVolumeUpMusic()
        {
            music.TurnVolumeUp();
            musicVolumeText.text = music.GetVolume();
        }
        public void TurnVolumeOnOffMusic(bool on)
        {
            music.TurnVolumeOnOff(on);
            musicOnText.text = music.GetOn();
        }
        
        public void TurnVolumeDownEffects()
        {
            effects.TurnVolumeDown();
            effectsVolumeText.text = effects.GetVolume();
            
        }
        public void TurnVolumeUpEffects()
        {
            effects.TurnVolumeUp();
            effectsVolumeText.text = effects.GetVolume();
        }
        
        public void TurnVolumeOnOffEffects(bool on)
        {
            effects.TurnVolumeOnOff(on);
            effectsOnText.text = effects.GetOn();
        }
        
    }
}