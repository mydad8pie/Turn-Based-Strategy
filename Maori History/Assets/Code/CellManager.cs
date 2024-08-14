using UnityEngine;

public class CellManager : MonoBehaviour
{
    public static CellManager Instance { get; private set; }

    private HexCell previousCell = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void HighlightCell(HexCell cell)
    {
        if (cell != null)
        {
            // cell.Highlight(true);
        }
    }

    public void ResetHighlight()
    {
        if (previousCell != null)
        {
            // previousCell.Highlight(false);
        }
    }
}
