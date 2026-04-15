using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject cam;
    [SerializeField] List<GameObject> pieces;
    [SerializeField] float flipDelay;

    Turn turn;

    public enum Turn
    {
        Light,
        Dark,
    }

    void Start()
    {
        turn = Turn.Light;
    }

    public void AddToPieces(GameObject piece) 
    {
        pieces.Add(piece);
    }

    public void RemoveFromPieces(GameObject piece)
    {
        pieces.Remove(piece);
    }

    public void ChangeTurn()
    {
        turn = turn == Turn.Light ? Turn.Dark : Turn.Light;
        StartCoroutine(FlipBoard());
    }

    IEnumerator FlipBoard()
    {
        yield return new WaitForSeconds(flipDelay);

        cam.transform.Rotate(0, 0, 180);
        foreach (GameObject piece in pieces)
        {
            piece.transform.Rotate(0, 0, 180);
        }
    }

    public Turn GetTurn()
    {
        return turn;
    }
}
