using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitMovement : MonoBehaviour
{
    public HexCell CurrentCell { get; set; }
    public HexGrid hexGrid;
    public int maxMoveRange; // Set by derived classes


    public int currentMoveRange;
    private List<HexCell> reachableCells = new List<HexCell>();
    private Dictionary<HexCell, Color> originalColors = new Dictionary<HexCell, Color>();

    protected virtual void Start()
    {

        if (hexGrid == null)
        {
            Debug.LogError("HexGrid is not assigned.");
            return;
        }

        CurrentCell = hexGrid.GetCell(transform.position);
        if (CurrentCell == null)
        {
          //  Debug.LogError("Failed to get cell from HexGrid.");
            return;
        }

        currentMoveRange = maxMoveRange;
    }

    public void ResetMoveRange()
    {
        currentMoveRange = maxMoveRange;
    }

    public void MoveTo(HexCell targetCell)
    {
        if (IsReachable(targetCell) && IsPlayerTurn())
        {

            int distance = hexGrid.GetDistance(CurrentCell, targetCell);

            if (currentMoveRange >= distance)
            {
                currentMoveRange -= distance;

                transform.position = targetCell.Position + Constants.UnitOffset;

                if (CurrentCell != null)
                {
                    ResetCellColor(CurrentCell);
                }

                CurrentCell = targetCell;

                ClearReachableCells();
                if (CurrentCell != null)
                {
                    ResetCellColor(CurrentCell);
                }

                HighlightReachableCells();
            }
        }
        else
        {
            Debug.Log("Target cell is not reachable or it's not the player's turn.");
        }
    }

    public void HighlightReachableCells()
    {
        if (CurrentCell == null)
        {
            Debug.LogError("Current cell is null.");
            return;
        }

        reachableCells.Clear();
        originalColors.Clear();
        Queue<HexCell> frontier = new Queue<HexCell>();
        HashSet<HexCell> visited = new HashSet<HexCell>();

        frontier.Enqueue(CurrentCell);
        visited.Add(CurrentCell);

        for (int i = 0; i < currentMoveRange; i++)
        {
            int frontierSize = frontier.Count;
            for (int j = 0; j < frontierSize; j++)
            {
                HexCell current = frontier.Dequeue();
                foreach (HexDirection direction in System.Enum.GetValues(typeof(HexDirection)))
                {
                    HexCell neighbor = current.GetNeighbor(direction);
                    if (neighbor != null && !visited.Contains(neighbor))
                    { 
                        if(IsColorMatch(neighbor.Color, new Color(0f, 0.16f, 1f))){
                        continue; // skips blue cells
                    }
                        frontier.Enqueue(neighbor);
                        visited.Add(neighbor);
                        reachableCells.Add(neighbor);
                        if (!originalColors.ContainsKey(neighbor))
                        {
                            originalColors[neighbor] = neighbor.Color;
                        }
                    }
                }
            }
        }

        foreach (HexCell cell in reachableCells)
        {
            cell.Color = Color.green; // Example highlight color
        }
    }

    public bool IsReachable(HexCell cell)
    {
        return reachableCells.Contains(cell);
    }

    public void ClearReachableCells()
    {
        foreach (HexCell cell in reachableCells)
        {
            ResetCellColor(cell);
        }
        reachableCells.Clear();
        originalColors.Clear();
    }

    public void ResetCellColor(HexCell cell)
    {
        if (originalColors.ContainsKey(cell))
        {
            cell.Color = originalColors[cell]; // Reset to the original color
        }
    }

    public bool IsPlayerTurn()
    {
        return TurnManager.Instance.currentPlayerIndex == 0 && !TurnManager.Instance.playerHasCompletedTurn && !PauseManager.Instance.IsPaused;
    }

    public bool IsPlayerControlled()
    {
    // Add logic to determine if this unit belongs to the current player
    // For example, based on player index or some player ID
    return TurnManager.Instance.currentPlayerIndex == 0;
    }

    List<HexCell> FindBlueCells()
    {
        List<HexCell> blueCells = new List<HexCell>();
        Color blueColor = new Color(0f, 0.16f, 1f); // 002AFF in RGB


        foreach (HexCell cell in hexGrid.GetAllCells())
        {
            if (IsColorMatch(cell.Color, blueColor))
            {
                blueCells.Add(cell);
            }
        }
        return blueCells;
    }
    private bool IsColorMatch(Color a, Color b, float tolerance = 0.01f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }
}
