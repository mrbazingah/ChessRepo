using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] protected GameObject currentCell;
    [SerializeField] protected GameObject[,] possibleMoves;

    protected bool hasMoved = false;
    protected BoardManager boardManager;

    public void Init(BoardManager manager)
    {
        boardManager = manager;
    }

    public virtual void CalculateMoves() { }

    public void SetCurrentCell(GameObject cell)
    {
        currentCell = cell;
        CalculateMoves();
    }

    public bool IsValidMove(GameObject cell)
    {
        CalculateMoves();
        for (int row = 0; row < possibleMoves.GetLength(0); row++)
        {
            for (int col = 0; col < possibleMoves.GetLength(1); col++)
            {
                if (possibleMoves[row, col] == cell) { return true; }
            }
        }
        return false;
    }

    public virtual void MovePiece(GameObject newCell)
    {
        hasMoved = true;
        SetCurrentCell(newCell);
        transform.position = newCell.transform.position;
    }

    public GameObject[,] GetPossibleMoves()
    {
        CalculateMoves();
        return possibleMoves;
    }

    public GameObject GetCurrentCell()
    {
        return currentCell;
    }

    public bool HasMoved()
    {
        return hasMoved;
    }
}