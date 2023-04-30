namespace Multiplayer.Models.Movement
{
    public enum MoveValidationTypes
    {
        CheckForClearPath,
        CheckEmpty,
        CheckOccupied,
        CheckHasNotMoved,
        CheckIsKingOrRook
    }
}