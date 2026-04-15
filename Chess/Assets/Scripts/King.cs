using UnityEngine;

public class King : Piece
{
    public override void CalculateMoves()
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(-1, -1),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 0)
        };

        possibleMoves = boardManager.CellCalculator(currentCell, directions, pieceColor);

        if (!hasMoved)
        {
            AddCastlingMoves();
        }
    }

    void AddCastlingMoves()
    {
        Vector2Int kingCoords = boardManager.GetCellCoords(currentCell);
        Rook[] rooks = FindObjectsByType<Rook>(FindObjectsSortMode.None);

        foreach (Rook rook in rooks)
        {
            if (rook.GetPieceColor() != pieceColor) continue;
            if (rook.HasMoved()) continue;

            Vector2Int rookCoords = boardManager.GetCellCoords(rook.GetCurrentCell());
            Vector2Int diff = rookCoords - kingCoords;

            // Rook must be axis-aligned with the king
            if (diff.x != 0 && diff.y != 0) continue;

            Vector2Int dir = new Vector2Int(
                diff.x == 0 ? 0 : (diff.x > 0 ? 1 : -1),
                diff.y == 0 ? 0 : (diff.y > 0 ? 1 : -1)
            );

            // Check every cell between king and rook is empty
            bool pathClear = true;

            Vector2Int check = kingCoords + dir;
            while (check != rookCoords)
            {
                GameObject checkCell = boardManager.GetCell(check.x, check.y);

                if (boardManager.IsCellOccupied(checkCell)) 
                { 
                    pathClear = false; 
                    break; 
                }

                check += dir;
            }

            if (!pathClear) continue;

            // Highlight the cell 2 squares toward the rook
            Vector2Int kingDest = kingCoords + dir * 2;

            GameObject destCell = boardManager.GetCell(kingDest.x, kingDest.y);
            if (destCell != null)
            {
                possibleMoves[kingDest.x, kingDest.y] = destCell;
            }
        }
    }

    public override void MovePiece(GameObject newCell, bool changeTurn = true)
    {
        Vector2Int oldCoords = boardManager.GetCellCoords(currentCell);
        Vector2Int newCoords = boardManager.GetCellCoords(newCell);
        Vector2Int diff = newCoords - oldCoords;

        bool isCastling = (Mathf.Abs(diff.x) == 2 && diff.y == 0) || (Mathf.Abs(diff.y) == 2 && diff.x == 0);

        base.MovePiece(newCell, changeTurn);

        if (isCastling)
        {
            Vector2Int dir = new Vector2Int(
                diff.x == 0 ? 0 : (diff.x > 0 ? 1 : -1),
                diff.y == 0 ? 0 : (diff.y > 0 ? 1 : -1)
            );

            // Rook lands on the square just behind the king's destination
            Vector2Int rookDestCoords = newCoords - dir;
            GameObject rookDestCell = boardManager.GetCell(rookDestCoords.x, rookDestCoords.y);

            // Find the rook by continuing past the king's landing in the same direction
            Rook[] rooks = FindObjectsByType<Rook>(FindObjectsSortMode.None);
            Vector2Int search = newCoords + dir;

            while (true)
            {
                GameObject searchCell = boardManager.GetCell(search.x, search.y);
                if (searchCell == null) { break; }

                bool found = false;
                foreach (Rook rook in rooks)
                {
                    if (rook.GetPieceColor() != pieceColor) continue;
                    if (rook.GetCurrentCell() == searchCell)
                    {
                        if (rookDestCell != null) { rook.MovePiece(rookDestCell, false); }

                        found = true;

                        break;
                    }
                }

                if (found) { break; }

                search += dir;
            }
        }
    }

    // Called from Player when the user clicks a rook while the king is selected
    public bool TryCastle(GameObject rookObject)
    {
        if (hasMoved) return false;

        Rook rook = rookObject.GetComponent<Rook>();
        if (rook == null || rook.HasMoved()) { return false; }

        Vector2Int kingCoords = boardManager.GetCellCoords(currentCell);
        Vector2Int rookCoords = boardManager.GetCellCoords(rook.GetCurrentCell());
        Vector2Int diff = rookCoords - kingCoords;

        // Rook must be axis-aligned with the king
        if (diff.x != 0 && diff.y != 0) return false;

        Vector2Int dir = new Vector2Int(
            diff.x == 0 ? 0 : (diff.x > 0 ? 1 : -1),
            diff.y == 0 ? 0 : (diff.y > 0 ? 1 : -1)
        );

        // Check every cell between king and rook is empty
        Vector2Int check = kingCoords + dir;
        while (check != rookCoords)
        {
            GameObject checkCell = boardManager.GetCell(check.x, check.y);

            if (boardManager.IsCellOccupied(checkCell)) { return false; }

            check += dir;
        }

        // King moves 2 squares, rook lands just behind king's destination
        Vector2Int kingDest = kingCoords + dir * 2;
        Vector2Int rookDest = kingDest - dir;

        GameObject kingDestCell = boardManager.GetCell(kingDest.x, kingDest.y);
        GameObject rookDestCell = boardManager.GetCell(rookDest.x, rookDest.y);

        if (kingDestCell == null || rookDestCell == null) { return false; }

        base.MovePiece(kingDestCell);
        rook.MovePiece(rookDestCell, false);

        return true;
    }
}
