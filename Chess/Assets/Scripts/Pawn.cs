using UnityEngine;

public class Pawn : Piece
{
    public override void CalculateMoves()
    {
        Vector2Int[] directions;

        if (!hasMoved)
        {
            directions = new Vector2Int[]
            {
                new Vector2Int(0, 1),
                new Vector2Int(0, 2)
            };
        }
        else
        {
            directions = new Vector2Int[]
            {
                new Vector2Int(0, 1)
            };
        }

        possibleMoves = boardManager.CellCalculator(currentCell, directions);
    }
}