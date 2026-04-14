using UnityEngine;

public class Rook : Piece
{
    public override void CalculateMoves()
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        possibleMoves = boardManager.CellCalculatorRay(currentCell, directions);
    }
}
