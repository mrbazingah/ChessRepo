using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class BoardManager : MonoBehaviour
{
    [Header("Board Setup")]
    [SerializeField] GameObject cellPrefab;
    [SerializeField] GameObject cellParent;
    [SerializeField] int boardSize;
    [SerializeField] Vector2 startPos;
    [SerializeField] Color lightColor;
    [SerializeField] Color darkColor;
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

    GameObject[,] board;

    Player player;

    void Start()
    {
        player = FindAnyObjectByType<Player>();

        GenerateBoard();
        GenerateWhitePieces();
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
                cellRenderer.color = (row + col) % 2 != 0 ? lightColor : darkColor;

                board[row, col] = newCell;
            }
        }
    }

    void GenerateWhitePieces()
    {
        // Pawns
        for (int i = 0; i < pawnCount; i++)
        {
            Vector2 spawnPos = board[i, 1].transform.position;
            GameObject newPawn = Instantiate(pawnPrefab, spawnPos, Quaternion.identity);
            newPawn.transform.SetParent(pawnParent.transform);

            Piece piece = newPawn.GetComponent<Piece>();
            piece.Init(this);
            piece.SetCurrentCell(board[i, 1]);
        }

        // Knights
        for (int i = 0; i < knightCount; i++)
        {
            int row = i == 0 ? 1 : 6;
            Vector2 spawnPos = board[row, 0].transform.position;
            GameObject newKnight = Instantiate(knightPrefab, spawnPos, Quaternion.identity);
            newKnight.transform.SetParent(knightParent.transform);

            Piece piece = newKnight.GetComponent<Piece>();
            piece.Init(this);
            piece.SetCurrentCell(board[row, 0]);
        }

        // Bishops
        for (int i = 0; i < bishopCount; i++)
        {
            int row = i == 0 ? 2 : 5;
            Vector2 spawnPos = board[row, 0].transform.position;
            GameObject newBishop = Instantiate(bishopPrefab, spawnPos, Quaternion.identity);
            newBishop.transform.SetParent(bishopParent.transform);

            Piece piece = newBishop.GetComponent<Piece>();
            piece.Init(this);
            piece.SetCurrentCell(board[row, 0]);
        }
    }

    public GameObject GetCell(int row, int col)
    {
        // Gets the cell if it isn't outside the board or impossible
        if (row < 0 || row >= boardSize || col < 0 || col >= boardSize) { return null; }
        return board[row, col];
    }

    public GameObject[,] CellCalculator(GameObject currentCell, Vector2Int[] directions)
    {
        Vector2Int coords = GetCellCoords(currentCell);
        GameObject[,] result = new GameObject[boardSize, boardSize];

        for (int i = 0; i < directions.Length; i++)
        {
            int newRow = coords.x + directions[i].x;
            int newCol = coords.y + directions[i].y;
            GameObject cell = GetCell(newRow, newCol);
            if (cell != null) { result[newRow, newCol] = cell; }
        }

        return result;
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
    // at a time and stops when a piece is in the way (blocking piece not included).
    public GameObject[,] CellCalculatorRay(GameObject currentCell, Vector2Int[] unitDirections)
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
                if (IsCellOccupied(cell)) { break; }

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
}