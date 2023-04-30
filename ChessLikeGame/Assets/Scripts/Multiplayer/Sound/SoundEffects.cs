using UnityEngine;
using UnityEngine.Serialization;

namespace Multiplayer.Sound
{
    public class SoundEffects : AbstractSoundController
    {
        [SerializeField] private AudioClip ChessPieceDownSound;
        [SerializeField] private AudioClip ChessPieceCaptureSound;

        public void PlayChessPieceDownSound()
        {
            audioSource.clip = ChessPieceDownSound;
            audioSource.Play();
        }
        public void PlayChessPieceCaptureSound()
        {
            audioSource.clip = ChessPieceCaptureSound;
            audioSource.Play();
        }

    }
}