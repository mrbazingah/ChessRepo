using UnityEngine;

public class Pawn : Piece
{
    public override void CalculateMoves()
    {
        int forward = pieceColor == PieceColor.Light ? 1 : -1;
        Vector2Int coords = boardManager.GetCellCoords(currentCell);

        // One square forward — blocked by any piece
        possibleMoves = boardManager.CellCalculator(currentCell, new Vector2Int[] { new Vector2Int(0, forward) });

        // Two squares forward only if the first square is also clear
        if (!hasMoved)
        {
            GameObject oneAhead = boardManager.GetCell(coords.x, coords.y + forward);
            if (oneAhead != null && boardManager.GetPieceOnCell(oneAhead) == null)
            {
                GameObject twoAhead = boardManager.GetCell(coords.x, coords.y + forward * 2);
                if (twoAhead != null && boardManager.GetPieceOnCell(twoAhead) == null)
                {
                    possibleMoves[coords.x, coords.y + forward * 2] = twoAhead;
                }
            }
        }

        Vector2Int[] captureDirections = new Vector2Int[]
        {
            new Vector2Int(1, forward),
            new Vector2Int(-1, forward)
        };

        foreach (Vector2Int dir in captureDirections)
        {
            int newRow = coords.x + dir.x;
            int newCol = coords.y + dir.y;
            GameObject cell = boardManager.GetCell(newRow, newCol);
            if (cell == null) { continue; }

            Piece occupant = boardManager.GetPieceOnCell(cell);
            if (occupant != null && occupant.GetPieceColor() != pieceColor)
            {
                possibleMoves[newRow, newCol] = cell;
            }
        }
    }
}