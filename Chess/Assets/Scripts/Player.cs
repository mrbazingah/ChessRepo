using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject selectedPiece;
    GameObject lastSelectedPiece;

    BoardManager boardManager;
    GameManager gameManager;

    void Start()
    {
        boardManager = FindAnyObjectByType<BoardManager>();
        gameManager = FindAnyObjectByType<GameManager>();
    }

    void Update()
    {
        if (selectedPiece != lastSelectedPiece)
        {
            lastSelectedPiece = selectedPiece;
            boardManager.HideHighlights();
        }
    }

    public void OnAttack(InputValue value)
    {
        if (!value.isPressed) { return; }

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPos, Vector2.zero);

        RaycastHit2D hit = default;
        foreach (RaycastHit2D h in hits)
        {
            if (h.collider.gameObject.tag != "Cell") { hit = h; break; }
        }
        if (hit.collider == null && hits.Length > 0) { hit = hits[0]; }

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.tag != "Cell")
            {
                Piece hitPiece = hit.collider.gameObject.GetComponent<Piece>();
                Piece.PieceColor hitColor = hitPiece.GetPieceColor();

                if (gameManager.GetTurn() == GameManager.Turn.Light && hitColor != Piece.PieceColor.Light
                    || gameManager.GetTurn() == GameManager.Turn.Dark && hitColor != Piece.PieceColor.Dark)
                {
                    if (selectedPiece != null)
                    {
                        Piece piece = selectedPiece.GetComponent<Piece>();
                        if (piece.IsValidMove(hitPiece.GetCurrentCell()))
                        {
                            Pawn pawn = selectedPiece.GetComponent<Pawn>();
                            bool isPromotion = pawn != null && boardManager.CanPromote(hitPiece.GetCurrentCell(), pawn);
                            piece.MovePiece(hitPiece.GetCurrentCell(), !isPromotion);
                        }
                    }
                    selectedPiece = null;
                    return;
                }

                // If the king is selected and the user clicks a rook, try to castle
                if (selectedPiece != null)
                {
                    King king = selectedPiece.GetComponent<King>();
                    if (king != null && king.TryCastle(hit.collider.gameObject))
                    {
                        selectedPiece = null;
                        return;
                    }
                }

                selectedPiece = hit.collider.gameObject;
            }
            else if (selectedPiece != null)
            {
                bool changeTurn = true;

                Pawn pawn = selectedPiece.GetComponent<Pawn>();
                if (pawn != null && boardManager.CanPromote(hit.collider.gameObject, pawn))
                {
                    changeTurn = false;
                }

                Piece piece = selectedPiece.GetComponent<Piece>();
                if (piece.IsValidMove(hit.collider.gameObject))
                {
                    piece.MovePiece(hit.collider.gameObject, changeTurn);
                }

                selectedPiece = null;
            }
        }
    }

    public GameObject GetSelectedPiece()
    {
        return selectedPiece;
    }
}
