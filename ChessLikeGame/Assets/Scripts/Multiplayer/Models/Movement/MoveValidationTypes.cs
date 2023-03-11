namespace Multiplayer.Models.Movement
{
    public enum MoveValidationTypes
    {
        CheckForClearPath,
        CheckEmpty,
        CheckOccupied,
        CheckKingCantBeTaken,
        CheckHasNotMoved,
        CheckKingSideRookHasNotMoved,
        CheckQueenSideRookHasNotMoved
    }
}