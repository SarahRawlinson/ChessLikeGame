namespace Multiplayer.Models.Movement
{
    public class Move
    {
        private int _startPosition;
        private int _endPosition;

        private bool _willResultInCapture;
        private ChessPieceTypes _capturedPiece;
        private TeamColor _colorToMove;

        public Move(int startPosition, int endPosition, TeamColor colorToMove)
        {
            _startPosition = startPosition;
            _endPosition = endPosition;
            _colorToMove = colorToMove;
        }


        public int EndPosition => _endPosition;
        public bool WillResultInCapture => _willResultInCapture;
        public ChessPieceTypes CapturedPiece => _capturedPiece;
        public TeamColor ColorToMove => _colorToMove;

        public int StartPosition
        {
            get => _startPosition;
        }


        public override string ToString()
        {
            return
                $"{_colorToMove}  Move from:{_startPosition} To:{_endPosition} / Capture?:{_willResultInCapture} : {_capturedPiece} ";
        }
    }
}