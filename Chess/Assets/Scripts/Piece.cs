using UnityEngine;

public class Piece : MonoBehaviour
{
    protected GameObject currentCell;
    protected GameObject[,] possibleMoves;

    protected bool hasMoved = false;
    protected BoardManager boardManager;
    protected GameManager gameManager;

    public enum PieceColor
    {
        Light,
        Dark,
    }

    protected PieceColor pieceColor;

    public void Init(BoardManager boardManager, GameManager gameManager)
    {
        this.boardManager = boardManager;
        this.gameManager = gameManager;
    }

    public void SetPieceColor(PieceColor color)
    {
        pieceColor = color;
    }

    public PieceColor GetPieceColor()
    {
        return pieceColor;
    }

    void Start()
    {
        gameManager.AddToPieces(gameObject);
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

    public virtual void MovePiece(GameObject newCell, bool changeTurn = true)
    {
        hasMoved = true;

        Piece captured = boardManager.GetPieceOnCell(newCell);
        if (captured != null) 
        { 
            gameManager.RemoveFromPieces(captured.gameObject);
            captured.gameObject.SetActive(false);
            Destroy(captured.gameObject); 
        }

        SetCurrentCell(newCell);
        transform.position = newCell.transform.position;

        if (changeTurn) { gameManager.ChangeTurn(); }
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