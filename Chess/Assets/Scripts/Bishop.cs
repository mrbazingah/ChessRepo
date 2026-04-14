using UnityEngine;

public class Bishop : Piece
{
    public override void CalculateMoves()
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 1),
            new Vector2Int(-1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, -1)
        };

        possibleMoves = boardManager.CellCalculatorRay(currentCell, directions);
    }
}