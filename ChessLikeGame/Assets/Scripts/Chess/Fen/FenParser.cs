using Chess.Board;

namespace Chess.Fen
{
        public class FenParser
    {
	    #region Fields

        /// <summary>
        /// The unparsed FEN string.
        /// </summary>
        private string Fen { get; set; }

        /// <summary>
        /// A FEN substring representing the position of the white and black pieces on the board.
        /// </summary>
        private string PiecePlacementString { get; set; }

        /// <summary>
        /// A FEN substring representing the active player.
        /// </summary>
        private string ActiveColorString { get; set; }

        /// <summary>
        /// A FEN substring representing the availability of castling for each player.
        /// </summary>
        private string CastlingAvailabilityString { get; set; }

        /// <summary>
        /// A FEN substring representing the square which is currently available for "en passant" capture ('-' if a square is not available).  
        /// </summary>
        private string EnPassantSquareString { get; set; }

        /// <summary>
        /// A FEN substring representing the number of half moves since the last pawn advance or piece capture (used to determine stalemate).
        /// </summary>
        private string HalfmoveClockString { get; set; }

        /// <summary>
        /// A FEN substring representing the game turn (incremented after Black moves).
        /// </summary>
        private string FullmoveNumberString { get; set; }

        public BoardStateData BoardStateData { get; set; }

        #endregion

        #region Methods

        public void ParseFenSubstrings(string[] fenSubstrings)
        {
            PiecePlacementString = fenSubstrings[0];
            ActiveColorString = fenSubstrings[1];
            CastlingAvailabilityString = fenSubstrings[2];
            EnPassantSquareString = fenSubstrings[3];
            HalfmoveClockString = fenSubstrings[4];
            FullmoveNumberString = fenSubstrings[5];
        }

        #endregion

        #region Constructors


        public FenParser(string fen)
        {
            Fen = fen;
            ParseFenSubstrings(Fen.Split(' '));

            BoardStateData = new BoardStateData(PiecePlacementString, ActiveColorString, CastlingAvailabilityString,
                EnPassantSquareString, HalfmoveClockString, FullmoveNumberString);
        }
        #endregion
    }

}