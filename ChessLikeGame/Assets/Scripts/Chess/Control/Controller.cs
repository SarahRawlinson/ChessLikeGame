using System;
using System.Collections.Generic;
using Chess.Board;
using Chess.Enums;
using Chess.Interface;
using Chess.Movement;
using Chess.Pieces;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Chess.Control
{
    public class Controller : MonoBehaviour, IController
    {
        internal Team _team = Team.Black;
        [SerializeField] bool _isActive = false;
        public event Action<Controller> OnMoved;
        internal event Action OnTurn;
        internal List<ChessPiece> pieces = new List<ChessPiece>();
        internal King _king;
        internal Controller otherPlayer;
        internal BoardObject _boardObject;
        public Team GetTeam()
        {
            return _team;
        }

        public void ChessPieceTaken(ChessPiece piece)
        {
            pieces.Remove(piece);
        }
        
        public virtual void Awake()
        {
            _boardObject = FindObjectOfType<BoardObject>();
            foreach (Controller con in FindObjectsOfType<Controller>())
            {
                if (con != this)
                {
                    otherPlayer = con;
                    break;
                }
            }
        }

        bool IsCheck()
        {
            if (_king.IsUnityNull()) return false;
            foreach (Moves move in otherPlayer.PossibleMoves(this))
            {
                if (move.MoveResultPos.GetCoordinates() == _king.GetPosition().GetCoordinates())
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsActive()
        {
            return _isActive;
        }

        public void SetActive()
        {
            _isActive = true;
            OnTurn?.Invoke();
            // Debug.Log($"Set Active {_team}");
        }
        public List<Moves> AllLegalMoves(Controller con)
        {
            return PossibleMoves(con);
        }

        public List<Moves> LegalMovesList(List<Moves> movesList)
        {
            if (IsCheck())
            {
                MoveStopsCheck(ref movesList);
            }
            return movesList;
        }

        //todo work this out
        private void MoveStopsCheck(ref List<Moves> movesList)
        {
            
        }

        private List<Moves> PossibleMoves(Controller con)
        {
            List<Moves> possibleMoves = new List<Moves>();
            foreach (ChessPiece piece in pieces)
            {
                possibleMoves.AddRange(piece.GetPossibleMoves(con));
            }

            return possibleMoves;
        }

        public static List<Moves> PossibleMovesOrderedByValue(ref List<Moves> myPossibleMoves)
        {
            myPossibleMoves.Sort(Comparison);
            return myPossibleMoves;
        }
        
        public static int Comparison(Moves x, Moves y)
        {
            return (int)(x.MoveValue - y.MoveValue);
        }
        
        public static Moves HighestValueMove(List<Moves> myPossibleMoves, List<Moves> opponentsPossibleMovesList)
        {
            foreach (Moves mm in myPossibleMoves)
            {
                string mc = mm.MoveResultPos.GetCoordinates();
                foreach (Moves om in opponentsPossibleMovesList)
                {
                    if (om.MoveResultPos.GetCoordinates() == mc)
                    {
                        mm.MoveValue -= mm.Piece.pieceValue;
                        // Debug.LogAssertion($"move value changed to {mm.MoveValue}");
                    }
                }
            }
            PossibleMovesOrderedByValue(ref myPossibleMoves);
            Moves move = myPossibleMoves[^1];
            List<Moves> movesList = myPossibleMoves.FindAll(m => m.MoveValue >= move.MoveValue);
            move = movesList[Random.Range(0, movesList.Count)];
            Debug.Log($"low Val = {myPossibleMoves[0].MoveValue}, High Val = {myPossibleMoves[^1].MoveValue}, Act Val = {move.MoveValue}");
            return move;
        }

        public void PiecesCallToController(ChessPiece piece)
        {
            if (!pieces.Contains(piece))
            {
                piece.TryGetComponent(out _king);
                pieces.Add(piece);
            }
            
        }

        public void SetTeam(Team team)
        {
            _team = team;
        }

        public void MoveMade()
        {
            // Debug.Log($"Move Made for {_team}");
            OnMoved?.Invoke(this);
            _isActive = false;
        }

        
    }
}