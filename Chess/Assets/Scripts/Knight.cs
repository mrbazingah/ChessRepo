using UnityEngine;

public class Knight : Piece
{
    public override void CalculateMoves()
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 2),
            new Vector2Int(2, 1),
            new Vector2Int(-1, 2),
            new Vector2Int(-2, 1),
            new Vector2Int(1, -2),
            new Vector2Int(2, -1),
            new Vector2Int(-1, -2),
            new Vector2Int(-2, -1)
        };

        possibleMoves = boardManager.CellCalculator(currentCell, directions);
    }
}