using UnityEngine;

namespace Chess.Sound
{
    public class SoundEffects : MonoBehaviour
    {
        [SerializeField] private AudioClip ChessPieceDownSound;
        [SerializeField] private AudioClip ChessPieceCaptureSound;
        [SerializeField] private AudioSource _source;
        public void PlayChessPieceDownSound()
        {
            _source.clip = ChessPieceDownSound;
            _source.Play();
        }
        public void PlayChessPieceCaptureSound()
        {
            _source.clip = ChessPieceCaptureSound;
            _source.Play();
        }
    }
}