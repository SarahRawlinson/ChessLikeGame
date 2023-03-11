using Unity.Netcode;
using Unity.VisualScripting;

namespace Multiplayer.Models
{
    public class Move
    {
        private int _startPosition;
        private int _endPosition;

        private bool _willResultInCapture;
        private ChessPieceTypes _capturedPiece;
        private TeamColor _colorToMove;
      
        public int StartPosition
        {
            get => _startPosition;
            set => _startPosition = value;
        }

        public int EndPosition => _endPosition;
        public bool WillResultInCapture => _willResultInCapture;
        public ChessPieceTypes CapturedPiece => _capturedPiece;
        public TeamColor ColorToMove => _colorToMove;


        public Move()
        {
        }

        public Move(int startPosition)
        {
            _startPosition = startPosition;
        }

        public override string ToString()
        {
            return
                $"{_colorToMove}  Move from:{_startPosition} To:{_endPosition} / Capture?:{_willResultInCapture} : {_capturedPiece} ";
        }
    }
}