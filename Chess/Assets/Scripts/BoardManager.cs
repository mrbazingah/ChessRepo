using System.Collections.Generic;
using UnityEngine;
using static Piece;

public class BoardManager : MonoBehaviour
{
    [Header("Board Setup")]
    [SerializeField] GameObject cellPrefab;
    [SerializeField] GameObject cellParent;
    [SerializeField] int boardSize;
    [SerializeField] Vector2 startPos;
    [SerializeField] Color lightCellColor;
    [SerializeField] Color darkCellColor;
    [Header("Pawn")]
    [SerializeField] GameObject pawnPrefab;
    [SerializeField] GameObject pawnParent;
    [SerializeField] int pawnCount;
    [Header("Knight")]
    [SerializeField] GameObject knightPrefab;
    [SerializeField] GameObject knightParent;
    [SerializeField] int knightCount;
    [Header("Bishop")]
    [SerializeField] GameObject bishopPrefab;
    [SerializeField] GameObject bishopParent;
    [SerializeField] int bishopCount;
    [Header("Rook")]
    [SerializeField] GameObject rookPrefab;
    [SerializeField] GameObject rookParent;
    [SerializeField] int rookCount;
    [Header("Queen")]
    [SerializeField] GameObject queenPrefab;
    [SerializeField] GameObject queenParent;
    [SerializeField] int queenCount;
    [Header("King")]
    [SerializeField] GameObject kingPrefab;
    [SerializeField] GameObject kingParent;
    [SerializeField] int kingCount;
    [Space]
    [SerializeField] Color lightPieceColor;
    [SerializeField] Color darkPieceColor;
    [Header("Promotion")]
    [SerializeField] GameObject promotionUI;

    GameObject[,] board;

    Player player;
    GameManager gameManager;
    Pawn promotionPawn;

    void Start()
    {
        player = FindAnyObjectByType<Player>();
        gameManager = FindAnyObjectByType<GameManager>();

        ClosePromotionUI();

        GenerateBoard();
        GenerateWhitePieces();
        GenerateBlackPieces();
    }

    void GenerateBoard()
    {
        board = new GameObject[boardSize, boardSize];

        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                Vector2 spawnPos = new Vector2(row + startPos.x, col + startPos.y);
                GameObject newCell = Instantiate(cellPrefab, spawnPos, Quaternion.identity);
                newCell.transform.SetParent(cellParent.transform);

                newCell.name = $"Cell {row}, {col}";

                SpriteRenderer cellRenderer = newCell.GetComponent<SpriteRenderer>();
                cellRenderer.color = (row + col) % 2 != 0 ? lightCellColor : darkCellColor;

                board[row, col] = newCell;
            }
        }
    }

    void GenerateWhitePieces()
    {
        // Pawns
        for (int i = 0; i < pawnCount; i++)
        {
            SpawnPiece(i, 1, pawnPrefab, pawnParent, Piece.PieceColor.Light);
        }

        // Knights
        for (int i = 0; i < knightCount; i++)
        {
            int row = i == 0 ? 1 : 6;
            SpawnPiece(row, 0, knightPrefab, knightParent, Piece.PieceColor.Light);
        }

        // Bishops
        for (int i = 0; i < bishopCount; i++)
        {
            int row = i == 0 ? 2 : 5;
            SpawnPiece(row, 0, bishopPrefab, bishopParent, Piece.PieceColor.Light);
        }

        // Rooks
        for (int i = 0; i < rookCount; i++)
        {
            int row = i == 0 ? 0 : 7;
            SpawnPiece(row, 0, rookPrefab, rookParent, Piece.PieceColor.Light);
        }

        // Queen
        for (int i = 0; i < queenCount; i++)
        {
            int row = 3;
            SpawnPiece(row, 0, queenPrefab, queenParent, Piece.PieceColor.Light);
        }

        // King
        for (int i = 0; i < kingCount; i++)
        {
            int row = 4;
            SpawnPiece(row, 0, kingPrefab, kingParent, Piece.PieceColor.Light);
        }
    }

    void GenerateBlackPieces()
    {
        // Pawns
        for (int i = 0; i < pawnCount; i++)
        {
            SpawnPiece(i, 6, pawnPrefab, pawnParent, Piece.PieceColor.Dark);
        }

        // Knights
        for (int i = 0; i < knightCount; i++)
        {
            int row = i == 0 ? 1 : 6;
            SpawnPiece(row, 7, knightPrefab, knightParent, Piece.PieceColor.Dark);
        }

        // Bishops
        for (int i = 0; i < bishopCount; i++)
        {
            int row = i == 0 ? 2 : 5;
            SpawnPiece(row, 7, bishopPrefab, bishopParent, Piece.PieceColor.Dark);
        }

        // Rooks
        for (int i = 0; i < rookCount; i++)
        {
            int row = i == 0 ? 0 : 7;
            SpawnPiece(row, 7, rookPrefab, rookParent, Piece.PieceColor.Dark);
        }

        // Queen
        for (int i = 0; i < queenCount; i++)
        {
            int row = 3;
            SpawnPiece(row, 7, queenPrefab, queenParent, Piece.PieceColor.Dark);
        }

        // King
        for (int i = 0; i < kingCount; i++)
        {
            int row = 4;
            SpawnPiece(row, 7, kingPrefab, kingParent, Piece.PieceColor.Dark);
        }
    }

    void SpawnPiece(int row, int col, GameObject piecePrefab, GameObject pieceParent, Piece.PieceColor pieceColor)
    {
        Vector2 spawnPos = board[row, col].transform.position;
        GameObject newPiece = Instantiate(piecePrefab, spawnPos, Quaternion.identity);
        newPiece.transform.SetParent(pieceParent.transform);

        SpriteRenderer pieceRenderer = newPiece.GetComponent<SpriteRenderer>();
        pieceRenderer.color = pieceColor == Piece.PieceColor.Light ? lightPieceColor : darkPieceColor;

        Piece piece = newPiece.GetComponent<Piece>();
        piece.Init(this, gameManager);
        piece.SetPieceColor(pieceColor);
        piece.SetCurrentCell(board[row, col]);
    }

    public GameObject GetCell(int row, int col)
    {
        // Gets the cell if it isn't outside the board or impossible
        if (row < 0 || row >= boardSize || col < 0 || col >= boardSize) { return null; }
        return board[row, col];
    }

    public GameObject[,] CellCalculator(GameObject currentCell, Vector2Int[] directions, Piece.PieceColor? pieceColor = null)
    {
        Vector2Int coords = GetCellCoords(currentCell);
        GameObject[,] result = new GameObject[boardSize, boardSize];

        for (int i = 0; i < directions.Length; i++)
        {
            int newRow = coords.x + directions[i].x;
            int newCol = coords.y + directions[i].y;
            GameObject cell = GetCell(newRow, newCol);
            if (cell == null) { continue; }

            Piece occupant = GetPieceOnCell(cell);
            if (occupant == null || (pieceColor != null && occupant.GetPieceColor() != pieceColor))
            {
                result[newRow, newCol] = cell;
            }
        }

        return result;
    }

    public Piece GetPieceOnCell(GameObject cell)
    {
        Piece[] pieces = FindObjectsByType<Piece>(FindObjectsSortMode.None);
        foreach (Piece piece in pieces)
        {
            if (piece.GetCurrentCell() == cell) { return piece; }
        }
        return null;
    }

    public Vector2Int GetCellCoords(GameObject cell)
    {
        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                if (board[row, col] == cell) { return new Vector2Int(row, col); }
                    
            }
        }
        return -Vector2Int.one;
    }

    void Update()
    {
        DisplayPieceMoves();
    }

    void DisplayPieceMoves()
    {
        GameObject selectedPiece = player.GetSelectedPiece();
        if (selectedPiece == null ) { return; }

        Piece piece = selectedPiece.GetComponent<Piece>();
        GameObject[,] possibleMoves = piece.GetPossibleMoves();

        for (int row = 0; row < possibleMoves.GetLength(0); row++)
        {
            for (int col = 0; col < possibleMoves.GetLength(1); col++)
            {
                if (possibleMoves[row, col] == null) { continue; }

                possibleMoves[row, col].GetComponent<Cell>().GetHighlight().SetActive(true);
            }
        }
    }

    public bool IsCellOccupied(GameObject cell)
    {
        Piece[] pieces = FindObjectsByType<Piece>(FindObjectsSortMode.None);
        foreach (Piece piece in pieces)
        {
            if (piece.GetCurrentCell() == cell) { return true; }
        }
        return false;
    }

    // Like CellCalculator but treats each direction as a ray: steps one square
    // at a time and stops when a piece is in the way. Enemy pieces are included
    // as capture targets when pieceColor is provided.
    public GameObject[,] CellCalculatorRay(GameObject currentCell, Vector2Int[] unitDirections, Piece.PieceColor? pieceColor = null)
    {
        Vector2Int coords = GetCellCoords(currentCell);
        GameObject[,] result = new GameObject[boardSize, boardSize];

        foreach (Vector2Int dir in unitDirections)
        {
            int row = coords.x + dir.x;
            int col = coords.y + dir.y;

            while (true)
            {
                GameObject cell = GetCell(row, col);
                if (cell == null) { break; }

                Piece occupant = GetPieceOnCell(cell);
                if (occupant != null)
                {
                    if (pieceColor != null && occupant.GetPieceColor() != pieceColor)
                    {
                        result[row, col] = cell;
                    }
                    break;
                }

                result[row, col] = cell;
                row += dir.x;
                col += dir.y;
            }
        }

        return result;
    }

    public void HideHighlights()
    {
        for (int row = 0; row < board.GetLength(0); row++)
        {
            for (int col = 0; col < board.GetLength(1); col++)
            {
                board[row, col].GetComponent<Cell>().GetHighlight().SetActive(false);
            }
        }
    }

    public void OpenPromotionUI(Pawn pawn)
    {
        promotionPawn = pawn;
        promotionUI.SetActive(true);
    }

    public void ClosePromotionUI()
    {
        promotionUI.SetActive(false);
    }

    public void Promote(string pieceType)
    {
        int row = 0;
        int col = 0;
        bool breaking = false;

        for (row = 0; row < board.GetLength(0); row++)
        {
            for (col = 0; col < board.GetLength(1); col++)
            {
                if (board[row, col] == promotionPawn.GetCurrentCell())
                {
                    breaking = true;
                    break;
                }
            }

            if (breaking) { break; }
        }

        GameObject piecePrefab = null;
        GameObject pieceParent = null;

        switch (pieceType)
        {
            case "Queen":
                piecePrefab = queenPrefab;
                pieceParent = queenParent;
                break;
            case "Rook":
                piecePrefab = rookPrefab;
                pieceParent = rookParent;
                break;
            case "Bishop":
                piecePrefab = bishopPrefab;
                pieceParent = bishopParent;
                break;
            case "Knight":
                piecePrefab = knightPrefab;
                pieceParent = knightParent;
                break;
        }

        Piece.PieceColor color = promotionPawn.GetPieceColor();

        SpawnPiece(row, col, piecePrefab, pieceParent, color);

        if (promotionPawn != null)
        {
            gameManager.RemoveFromPieces(promotionPawn.gameObject);
        }

        promotionPawn.gameObject.SetActive(false);
        Destroy(promotionPawn.gameObject);

        ClosePromotionUI();
        gameManager.ChangeTurn();
    }

    public bool CanPromote(GameObject currentCell, Pawn newPawn)
    {
        for (int row = 0; row < board.GetLength(0); row++)
        {
            for (int col = 0; col < board.GetLength(1); col++)
            {
                if (board[row, col] == currentCell)
                {
                    bool shouldPromote = newPawn.GetPieceColor() == PieceColor.Light
                        ? col == board.GetLength(1) - 1
                        : col == 0;
                    if (shouldPromote) OpenPromotionUI(newPawn);
                    return shouldPromote;
                }
            }
        }

        return false;
    }

    public GameObject[,] GetBoard()
    {
        return board;
    }
}