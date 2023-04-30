using System;
using Multiplayer.Models.BoardState;

namespace Multiplayer.Models.Movement
{
    public class Move
    {
        private int _startPosition;
        private int _endPosition;

        private bool _willResultInCapture;
        private ChessPieceTypes _capturedPiece;
        private TeamColor _colorToMove;
        private bool _hasSecondMove = false;
        private Move _secondMove = null;
        private MoveTypes _type;

        public bool HasSecondMove => _hasSecondMove;
        public MoveTypes MoveType => _type;

        public Move SecondMove => _secondMove;

  
        public Move(MoveTypes type, int startPosition, int endPosition, TeamColor colorToMove)
        {
            _type = type;
            _startPosition = startPosition;
            _endPosition = endPosition;
            _colorToMove = colorToMove;
        }
        
        public Move(MoveTypes type, int startPosition, int endPosition, TeamColor colorToMove, Move secondMove)
        {
            _type = type;
            _startPosition = startPosition;
            _endPosition = endPosition;
            _colorToMove = colorToMove;
            _secondMove = secondMove;
            _hasSecondMove = true;
        }

        public Move(string NetworkText)
        {
            ParseNetworkData(NetworkText);
        }

        public void SetColorToMove(TeamColor color)
        {
            _colorToMove = color;
        }

        public void SetWillResultInCapture(bool value)
        {
            _willResultInCapture = value;
        }
        
        public int EndPosition => _endPosition;
        public bool WillResultInCapture => _willResultInCapture;
        public ChessPieceTypes CapturedPiece => _capturedPiece;
        public TeamColor ColorToMove => _colorToMove;

        public int StartPosition
        {
            get => _startPosition;
        }

        public string GetNetworkData()
        {
            string secondMoveData = _hasSecondMove ? $"|{_secondMove.GetNetworkData()}" : "";
            return $"{_type}|{_startPosition}|{_endPosition}|{_colorToMove}|{_willResultInCapture}{secondMoveData}";
        }

        private void ParseNetworkData(string input)
        {
            var moveText = input.Split('|', StringSplitOptions.None);
            _type = Enum.Parse<MoveTypes>(moveText[0]);
            _startPosition = Int32.Parse(moveText[1]);
            _endPosition = Int32.Parse(moveText[2]);
            _colorToMove = Enum.Parse<TeamColor>(moveText[3]);
            _willResultInCapture = Boolean.Parse(moveText[4]);

            if (moveText.Length > 5)
            {
                _hasSecondMove = true;
                _secondMove = new Move(moveText[5]);
            }
            else
            {
                _hasSecondMove = false;
                _secondMove = null;
            }
        }

        public override string ToString()
        {
            return
                $"{_colorToMove}  Move from: {ChessGrid.GetKeyFromIndex(_startPosition)} To: {ChessGrid.GetKeyFromIndex(_endPosition)} / Capture?: {_willResultInCapture} : {_capturedPiece} ";
        }

        public static bool CheckEqual(Move move, Move moveToTest)
        {
            if (move.HasSecondMove != moveToTest.HasSecondMove)
            {
                return false;
            }
            if (moveToTest.HasSecondMove && move.HasSecondMove)
            {
                if (!CheckEqual(moveToTest.SecondMove, move.SecondMove))
                {
                    return false;
                }
            }
            return move.StartPosition == moveToTest.StartPosition && move._endPosition == moveToTest._endPosition
                 && move.ColorToMove == moveToTest.ColorToMove;
        }
    }
}