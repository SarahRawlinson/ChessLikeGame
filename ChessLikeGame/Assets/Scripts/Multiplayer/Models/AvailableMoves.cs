using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Multiplayer.Models
{
    public static class AvailableMovesFinder
	{
		public static IEnumerable<Point> GetPossibleMoves(List<MultiPiece> allies, List<MultiPiece> opponent, MultiPiece piece)
		{
			switch (piece.GetType())
			{
				case ChessPieceTypes.NONE:
					break;
				case ChessPieceTypes.PAWN:
					return GetPawnMoves(opponent, allies, piece.X, piece.Y, piece.Colour==TeamColor.White, piece.HasMoved());
					break;
				case ChessPieceTypes.BISHOP:
					return GetBishopMoves(opponent, allies, piece.X, piece.Y);
					break;
				case ChessPieceTypes.KNIGHT:
					return GetKnightMoves(allies, piece.X, piece.Y);
					break;
				case ChessPieceTypes.ROOK:
					return GetRookMoves(opponent, allies, piece.X, piece.Y);
					break;
				case ChessPieceTypes.QUEEN:
					return GetQueenMoves(opponent, allies, piece.X, piece.Y);
					break;
				case ChessPieceTypes.KING:
					return GetKingMoves(allies, piece.X, piece.Y);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			return new List<Point>();
		}

		private static IEnumerable<Point> GetKingMoves(List<MultiPiece> allies, int x, int y)
		{
			List<MultiPiece> possiblePieces =
					allies.Where(piece => piece.X <= x + 1 && piece.X >= x - 1 && piece.Y >= y - 1 && piece.Y <= y + 1)
								.ToList();
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (x + i >= 0 && x + i <= 7 && y + j >= 0 && y + j <= 7 &&
							!possiblePieces.Any(piece => piece.X == x + i && piece.Y == y + j))
					{
						yield return new Point(x + i, y + j);
					}
				}
			}
		}
		
		private static IEnumerable<Point> GetQueenMoves(List<MultiPiece> opponent, List<MultiPiece> allies, int x, int y)
		{
			for (int i = 1; i < 8; i++)
			{
				if (y + i > 7 || allies.Any(piece => (piece.X == x) && (piece.Y == y + i)))
				{
					break;
				}

				yield return new Point(x, y + i);

				if (opponent.Any(piece => (piece.X == x) && (piece.Y == y + i)))
				{
					break;
				}
			}
			for (int i = 1; i < 8; i++)
			{
				if (y - i < 0 || allies.Any(piece => (piece.X == x) && (piece.Y == y - i)))
				{
					break;
				}

				yield return new Point(x, y - i);

				if (opponent.Any(piece => (piece.X == x) && (piece.Y == y - i)))
				{
					break;
				}
			}
			for (int i = 1; i < 8; i++)
			{
				if (x + i > 7 || allies.Any(piece => (piece.X == x + i) && (piece.Y == y)))
				{
					break;
				}

				yield return new Point(x + i, y);

				if (opponent.Any(piece => (piece.X == x + i) && (piece.Y == y)))
				{
					break;
				}
			}
			for (int i = 1; i < 8; i++)
			{
				if (x - i < 0 || allies.Any(piece => (piece.X == x - i) && (piece.Y == y)))
				{
					break;
				}

				yield return new Point(x - i, y);

				if (opponent.Any(piece => (piece.X == x - i) && (piece.Y == y)))
				{
					break;
				}
			}
			for (int i = 1; i < 8; i++)
			{
				if (x + i > 7 || y + i > 7 || allies.Any(piece => (piece.X == x + i) && (piece.Y == y + i)))
				{
					break;
				}

				yield return new Point(x + i, y + i);

				if (opponent.Any(piece => (piece.X == x + i) && (piece.Y == y + i)))
				{
					break;
				}
			}
			for (int i = 1; i < 8; i++)
			{
				if (x - i < 0 || y + i > 7 || allies.Any(piece => (piece.X == x - i) && (piece.Y == y + i)))
				{
					break;
				}

				yield return new Point(x - i, y + i);

				if (opponent.Any(piece => (piece.X == x - i) && (piece.Y == y + i)))
				{
					break;
				}
			}
			for (int i = 1; i < 8; i++)
			{
				if (x - i < 0 || y - i < 0 || allies.Any(piece => (piece.X == x - i) && (piece.Y == y - i)))
				{
					break;
				}

				yield return new Point(x - i, y - i);

				if (opponent.Any(piece => (piece.X == x - i) && (piece.Y == y - i)))
				{
					break;
				}
			}
			for (int i = 1; i < 8; i++)
			{
				if (x + i > 7 || y - i < 0 || allies.Any(piece => (piece.X == x + i) && (piece.Y == y - i)))
				{
					break;
				}

				yield return new Point(x + i, y - i);

				if (opponent.Any(piece => (piece.X == x + i) && (piece.Y == y - i)))
				{
					break;
				}
			}
		}

		private static IEnumerable<Point> GetBishopMoves(List<MultiPiece> opponent, List<MultiPiece> allies, int x, int y)
		{
			for (int i = 1; i < 8; i++)
			{
				if (x + i > 7 || y + i > 7 || allies.Any(piece => (piece.X == x + i) && (piece.Y == y + i)))
				{
					break;
				}

				yield return new Point(x + i, y + i);

				if (opponent.Any(piece => (piece.X == x + i) && (piece.Y == y + i)))
				{
					break;
				}
			}
			for (int i = 1; i < 8; i++)
			{
				if (x - i < 0 || y + i > 7 || allies.Any(piece => (piece.X == x - i) && (piece.Y == y + i)))
				{
					break;
				}

				yield return new Point(x - i, y + i);

				if (opponent.Any(piece => (piece.X == x - i) && (piece.Y == y + i)))
				{
					break;
				}
			}
			for (int i = 1; i < 8; i++)
			{
				if (x - i < 0 || y - i < 0 || allies.Any(piece => (piece.X == x - i) && (piece.Y == y - i)))
				{
					break;
				}

				yield return new Point(x - i, y - i);

				if (opponent.Any(piece => (piece.X == x - i) && (piece.Y == y - i)))
				{
					break;
				}
			}
			for (int i = 1; i < 8; i++)
			{
				if (x + i > 7 || y - i < 0 || allies.Any(piece => (piece.X == x + i) && (piece.Y == y - i)))
				{
					break;
				}

				yield return new Point(x + i, y - i);

				if (opponent.Any(piece => (piece.X == x + i) && (piece.Y == y - i)))
				{
					break;
				}
			}
		}

		private static IEnumerable<Point> GetKnightMoves(List<MultiPiece> allies, int x, int y)
		{
			var possiblePieces = allies.Where(piece =>
				((piece.Y == y + 2 || piece.Y == y - 2) &&
				(piece.X == x - 1 || piece.X == x + 1)) ||
				((piece.X == x + 2) || piece.X == x - 2) &&
				(piece.Y == y - 1 || piece.Y == y + 1)).ToList();


			if (x + 1 <= 7 && y + 2 <= 7 && !possiblePieces.Any(piece => piece.X == x + 1 && piece.Y == y + 2))
			{
				yield return new Point(x + 1, y + 2);
			}
			if (x + 2 <= 7 && y + 1 <= 7 && !possiblePieces.Any(piece => piece.X == x + 2 && piece.Y == y + 1))
			{
				yield return new Point(x + 2, y + 1);
			}
			if (x - 1 >= 0 && y + 2 <= 7 && !possiblePieces.Any(piece => piece.X == x - 1 && piece.Y == y + 2))
			{
				yield return new Point(x - 1, y + 2);
			}
			if (x - 2 >= 0 && y + 1 <= 7 && !possiblePieces.Any(piece => piece.X == x - 2 && piece.Y == y + 1))
			{
				yield return new Point(x - 2, y + 1);
			}
			if (x + 1 <= 7 && y - 2 >= 0 && !possiblePieces.Any(piece => piece.X == x + 1 && piece.Y == y - 2))
			{
				yield return new Point(x + 1, y - 2);
			}
			if (x + 2 <= 7 && y - 1 >= 0 && !possiblePieces.Any(piece => piece.X == x + 2 && piece.Y == y - 1))
			{
				yield return new Point(x + 2, y - 1);
			}
			if (x - 1 >= 0 && y - 2 >= 0 && !possiblePieces.Any(piece => piece.X == x - 1 && piece.Y == y - 2))
			{
				yield return new Point(x - 1, y - 2);
			}
			if (x - 2 >= 0 && y - 1 >= 0 && !possiblePieces.Any(piece => piece.X == x - 2 && piece.Y == y - 1))
			{
				yield return new Point(x - 2, y - 1);
			}
		}

		private static IEnumerable<Point> GetRookMoves(List<MultiPiece> opponent, List<MultiPiece> allies, int x, int y)
		{
			List<MultiPiece> possibleAllies = allies.Where(piece => piece.X == x || piece.Y == y).ToList();
			List<MultiPiece> possibleEnemies = opponent.Where(piece => piece.X == x || piece.Y == y).ToList();

			for (int i = 1; i < 8; i++)
			{
				if (y + i > 7 || possibleAllies.Any(piece => (piece.X == x) && (piece.Y == y + i)))
				{
					break;
				}

				yield return new Point(x, y + i);

				if (possibleEnemies.Any(piece => (piece.X == x) && (piece.Y == y + i)))
				{
					break;
				}
			}
			for (int i = 1; i < 8; i++)
			{
				if (y - i < 0 || possibleAllies.Any(piece => (piece.X == x) && (piece.Y == y - i)))
				{
					break;
				}

				yield return new Point(x, y - i);

				if (possibleEnemies.Any(piece => (piece.X == x) && (piece.Y == y - i)))
				{
					break;
				}
			}
			for (int i = 1; i < 8; i++)
			{
				if (x + i > 7 || possibleAllies.Any(piece => (piece.X == x + i) && (piece.Y == y)))
				{
					break;
				}

				yield return new Point(x + i, y);

				if (possibleEnemies.Any(piece => (piece.X == x + i) && (piece.Y == y)))
				{
					break;
				}
			}
			for (int i = 1; i < 8; i++)
			{
				if (x - i < 0 || possibleAllies.Any(piece => (piece.X == x - i) && (piece.Y == y)))
				{
					break;
				}

				yield return new Point(x - i, y);

				if (possibleEnemies.Any(piece => (piece.X == x - i) && (piece.Y == y)))
				{
					break;
				}
			}
		}

		private static IEnumerable<Point> GetPawnMoves(List<MultiPiece> opponent, List<MultiPiece> allies, int x, int y, Boolean isMovingUp, Boolean hasMoved)
		{
			if (isMovingUp)
			{
				if (!opponent.Any(piece => (piece.X == x) && (piece.Y == y + 1)) && !allies.Any(piece => (piece.X == x) && (piece.Y == y + 1)))
				{
					yield return new Point(x, y + 1);
				}
				if (!hasMoved && !opponent.Any(piece => (piece.X == x) && (piece.Y == y + 2)) && !allies.Any(piece => (piece.X == x) && (piece.Y == y + 2)) && !opponent.Any(piece => (piece.X == x) && (piece.Y == y + 1)) && !allies.Any(piece => (piece.X == x) && (piece.Y == y + 1)))
				{
					yield return new Point(x, y + 2);
				}
				if (x - 1 >= 0 && y + 1 >= 0 && opponent.Any(piece => (piece.X == x - 1) && (piece.Y == y + 1)))
				{
					yield return new Point(x - 1, y + 1);
				}
				if (x + 1 < 8 && y + 1 >= 0 && opponent.Any(piece => (piece.X == x + 1) && (piece.Y == y + 1)))
				{
					yield return new Point(x + 1, y + 1);
				}
			}
			if (!isMovingUp)
			{
				if (!opponent.Any(piece => (piece.X == x) && (piece.Y == y - 1)) && !allies.Any(piece => (piece.X == x) && (piece.Y == y - 1)))
				{
					yield return new Point(x, y - 1);
				}
				if (!hasMoved && !opponent.Any(piece => (piece.X == x) && (piece.Y == y - 2)) && !allies.Any(piece => (piece.X == x) && (piece.Y == y - 2)) && !opponent.Any(piece => (piece.X == x) && (piece.Y == y - 1)) && !allies.Any(piece => (piece.X == x) && (piece.Y == y - 1)))
				{
					yield return new Point(x, y - 2);
				}
				if (x - 1 >= 0 && y - 1 < 8 && opponent.Any(piece => (piece.X == x - 1) && (piece.Y == y - 1)))
				{
					yield return new Point(x - 1, y - 1);
				}
				if (x + 1 < 8 && y - 1 < 8 && opponent.Any(piece => (piece.X == x + 1) && (piece.Y == y - 1)))
				{
					yield return new Point(x + 1, y - 1);
				}
			}
		}
	}
}