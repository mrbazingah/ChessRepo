using UnityEngine;

public class Pawn : Piece
{
    public override void CalculateMoves()
    {
        Vector2Int[] directions;
        int forward = pieceColor == PieceColor.Light ? 1 : -1;

        if (!hasMoved)
        {
            directions = new Vector2Int[]
            {
                new Vector2Int(0, forward),
                new Vector2Int(0, forward * 2)
            };
        }
        else
        {
            directions = new Vector2Int[]
            {
                new Vector2Int(0, forward)
            };
        }

        possibleMoves = boardManager.CellCalculator(currentCell, directions);
    }
}