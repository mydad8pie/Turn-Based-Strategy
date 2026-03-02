using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IUnitMovement
{
    GameObject GameObject { get; }
    HexCell CurrentCell { get; set; }

    void ClearReachableCells();
    void HighlightReachableCells();
    global::System.Boolean IsPlayerControlled();
    global::System.Boolean IsPlayerTurn();
    global::System.Boolean IsReachable(HexCell cell);
    void MoveTo(HexCell targetCell);
    void MoveTowardCell(HexCell targetCell);
    List<HexCell> ReachableCells();
    void ResetCellColor(HexCell cell);
    void ResetMoveRange();
}

public class UnitMovement : MonoBehaviour, IUnit, IUnitMovement
{
    public GameObject GameObject => gameObject;
    public HexCell CurrentCell { get; set; }
    public HexGrid hexGrid;
    public int maxMoveRange; // Set by derived classes

    public int ownerIndex = 0; // 0 = player, 1 = computer


    public int currentMoveRange;
    private List<HexCell> reachableCells = new List<HexCell>();
    public List<HexCell> ReachableCells()
    {
        return reachableCells;
    }
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

        // Register this unit on its starting cell
        hexGrid.RegisterUnit(CurrentCell, this as IUnit);
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
                //check if there is an enemy uniit on the target cell
                IUnit occupant = hexGrid.GetUnitAtCell(targetCell);
                if (occupant != null)
                {
                    UnitMovement occupantMovement = occupant.GameObject.GetComponent<UnitMovement>();
                    if (occupantMovement != null && occupantMovement.ownerIndex != ownerIndex)
                    {
                        // Attack instead of moveing
                        currentMoveRange -= distance;
                        CombatManager.Instance.TryAttack(this, occupantMovement);
                        ClearReachableCells();
                        return;
                    }
                }
                currentMoveRange -= distance;

                // Unregister from old cell
                hexGrid.UnregisterUnit(CurrentCell);

                transform.position = targetCell.Position + Constants.UnitOffset;

                if (CurrentCell != null)
                {
                    ResetCellColor(CurrentCell);
                }

                CurrentCell = targetCell;
                // Register on new cell
                hexGrid.RegisterUnit(CurrentCell, this as IUnit);

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
                        // skip blue (water) cells
                        if (IsColorMatch(neighbor.Color, new Color(0f, 0.16f, 1f)))
                        {
                            continue; // skips blue cells
                        }
                        // Skip cells that already have a unit on them or has a emey unit
                        if (hexGrid.IsCellOccupied(neighbor))
                        {
                            IUnit occupant = hexGrid.GetUnitAtCell(neighbor);
                            UnitMovement occupantMovement = occupant.GameObject.GetComponent<UnitMovement>();

                            //if it is an enemy show it as reachable (so player can attack)
                            // if it friendly, skip it
                            if (occupantMovement != null && occupantMovement.ownerIndex != ownerIndex)
                            {
                                reachableCells.Add(neighbor);

                                if (!originalColors.ContainsKey(neighbor))
                                {
                                    originalColors[neighbor] = neighbor.Color;
                                }

                            }

                            continue;
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
        if (ownerIndex == 0)
        {
            return TurnManager.Instance.currentPlayerIndex == 0 && !TurnManager.Instance.playerHasCompletedTurn && !PauseManager.Instance.IsPaused;
        }
        else
        {
            return TurnManager.Instance.currentPlayerIndex == ownerIndex;

        }



    }

    public bool IsPlayerControlled()
    {

        return ownerIndex == 0;

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

    void OnDestroy()
    {
        if (CurrentCell != null && hexGrid != null)
        {
            hexGrid.UnregisterUnit(CurrentCell);
        }
    }

    public void MoveTowardCell(HexCell targetCell)
    {
        if (CurrentCell == null || targetCell == null) return;

        HighlightReachableCells();

        //Find the reachable cell that is closest to the target cell
        HexCell bestCell = null;
        int bestDistance = int.MaxValue;

        foreach (HexCell cell in ReachableCells())
        {
            int distance = hexGrid.GetDistance(cell, targetCell);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestCell = cell;
            }
        }

        if (bestCell != null)
        {
            MoveTo(bestCell);
        }

        ClearReachableCells();
    }
}
