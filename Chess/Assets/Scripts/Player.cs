using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject selectedPiece;
    GameObject lastSelectedPiece;

    BoardManager boardManager;

    void Start()
    {
        boardManager = FindAnyObjectByType<BoardManager>();
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
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.tag != "Cell")
            {
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
            else if (hit.collider.gameObject.tag == "Cell" && selectedPiece != null)
            {
                Piece piece = selectedPiece.GetComponent<Piece>();
                if (piece.IsValidMove(hit.collider.gameObject))
                {
                    piece.MovePiece(hit.collider.gameObject);
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
