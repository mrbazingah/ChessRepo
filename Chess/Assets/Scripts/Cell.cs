using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] GameObject highlight;

    public GameObject GetHighlight()
    {
        return highlight;
    }
}
