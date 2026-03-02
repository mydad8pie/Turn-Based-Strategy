using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerAI : MonoBehaviour
{
    public static ComputerAI Instance { get; private set; }

    private UnitManager unitManager;
    private HexGrid hexGrid;

    // Track which villages are currently training
    private List<ComputerTrainingVillage> trainingVillages = new List<ComputerTrainingVillage>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        unitManager = FindObjectOfType<UnitManager>();
        hexGrid = FindObjectOfType<HexGrid>();
    }

    public void ExecuteTurn()
    {
        StartCoroutine(ExecuteTurnRoutine());
    }

    IEnumerator ExecuteTurnRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        // Handle village training first
        CheckCompletedTraining();
        StartTrainingInVillages();

        // Then move units
        UnitMovement[] allUnits = FindObjectsOfType<UnitMovement>();

        foreach (UnitMovement unit in allUnits)
        {
            // Check if unit still exists (may have been destroyed during the turn)
            if (unit == null) continue;
            if (unit.ownerIndex != 1) continue; // Only move computer units

            yield return new WaitForSeconds(0.3f); // Small delay between unit actions

            if (unit == null) continue; // Check again after delay

            if (unit.GameObject.name == "Settler(Clone)")
            {
                HandleSettler(unit);
            }
            else if (unit.GameObject.name == "Warrior(Clone)")
            {
                HandleWarrior(unit);
            }
            else if (unit.GameObject.name == "Builder(Clone)")
            {
                HandleBuilder(unit);
            }
        
        }

        TurnManager.Instance.EndComputerTurn();
    }

    // Find all computer villages and start training if not already training
    void StartTrainingInVillages()
    {
        Village[] allVillages = FindObjectsOfType<Village>();

        foreach (Village village in allVillages)
        {
            if (village.ownerIndex == 1)
            {
                if (!IsVillageTraining(village.gameObject))
                {
                    // Decide what to train based on priority:
                    // 1. Builder, 2. Warrior, 3. Settler
                    string unitToTrain = DecideUnitToTrain();
                    int duration = GetTrainingDuration(unitToTrain);

                    ComputerTrainingVillage tv = new ComputerTrainingVillage
                    {
                        village = village.gameObject,
                        unitType = unitToTrain,
                        startTurn = TurnManager.Instance.turnCounter,
                        trainingDuration = duration
                    };

                    trainingVillages.Add(tv);
                    Debug.Log("Computer started training " + unitToTrain + " in " + village.gameObject.name);
                }
            }
        }
    }

    // Check if any training is complete and spawn the unit
    void CheckCompletedTraining()
    {
        for (int i = trainingVillages.Count - 1; i >= 0; i--)
        {
            ComputerTrainingVillage tv = trainingVillages[i];

            if (TurnManager.Instance.turnCounter > tv.startTurn + tv.trainingDuration)
            {
                SpawnTrainedUnit(tv);
                trainingVillages.RemoveAt(i);
            }
        }
    }

    void SpawnTrainedUnit(ComputerTrainingVillage tv)
    {
        Village villageComponent = tv.village.GetComponent<Village>();
        if (villageComponent == null) return;

        HexCell spawnCell = FindEmptyCellNearVillage(villageComponent.CurrentCell);
        if (spawnCell == null)
        {
            Debug.Log("No empty cell near computer village to spawn unit.");
            return;
        }

        switch (tv.unitType)
        {
            case "Builder":
                unitManager.SpawnBuilder(spawnCell, 1);
                break;
            case "Warrior":
                unitManager.SpawnWarrior(spawnCell, 1);
                break;
            case "Settler":
                unitManager.SpawnSettler(spawnCell, 1);
                break;
        }

        Debug.Log("Computer finished training " + tv.unitType);
    }

    // Find an empty neighbouring cell near the village to spawn the unit
    HexCell FindEmptyCellNearVillage(HexCell villageCell)
    {
        foreach (HexDirection direction in System.Enum.GetValues(typeof(HexDirection)))
        {
            HexCell neighbor = villageCell.GetNeighbor(direction);
            if (neighbor != null && !hexGrid.IsCellOccupied(neighbor))
            {
                return neighbor;
            }
        }
        return null;
    }

    string DecideUnitToTrain()
    {
        // Count existing computer units
        int builderCount = 0;
        int warriorCount = 0;
        int settlerCount = 0;

        UnitMovement[] allUnits = FindObjectsOfType<UnitMovement>();
        foreach (UnitMovement unit in allUnits)
        {
            if (unit.ownerIndex == 1)
            {
                if (unit.gameObject.name == "Builder(Clone)") builderCount++;
                else if (unit.gameObject.name == "Warrior(Clone)") warriorCount++;
                else if (unit.gameObject.name == "Settler(Clone)") settlerCount++;
            }
        }

        // Priority: Builder first, then Warrior, then Settler
        // But dont over stack one type
        if (builderCount <= warriorCount) return "Builder";
        if (warriorCount <= settlerCount) return "Warrior";
        return "Settler";
    }

    int GetTrainingDuration(string unitType)
    {
        // Same durations as player
        switch (unitType)
        {
            case "Builder": return 0;
            case "Settler": return 1;
            case "Warrior": return 2;
            default: return 1;
        }
    }

    bool IsVillageTraining(GameObject village)
    {
        return trainingVillages.Exists(tv => tv.village == village);
    }

    void HandleSettler(UnitMovement settler)
    {
        if (settler.CurrentCell == null) return;

        HexCell bestCell = FindGoodSettleCell(settler.CurrentCell);

        if (bestCell != null)
        {
            settler.MoveTowardCell(bestCell);

            if (settler.currentMoveRange == 0)
            {
                PlaceComputerVillage(settler);
            }
        }
        else
        {
            PlaceComputerVillage(settler);
        }
    }

    void HandleWarrior(UnitMovement warrior)
    {
        if (warrior.CurrentCell == null) return;

        UnitMovement nearestEnemy = FindNearestEnemy(warrior);

        if (nearestEnemy != null)
        {
            warrior.MoveTowardCell(nearestEnemy.CurrentCell);
        }
    }

    void HandleBuilder(UnitMovement builder)
    {
        if (builder.CurrentCell == null) return;
        MoveRandomly(builder);
    }

    HexCell FindGoodSettleCell(HexCell from)
    {
        HexCell bestCell = null;
        int bestDistance = 0;

        foreach (HexCell cell in hexGrid.GetAllCells())
        {
            if (IsColorMatch(cell.Color, new Color(0f, 0.16f, 1f))) continue;
            if (hexGrid.IsCellOccupied(cell)) continue;

            int distance = hexGrid.GetDistance(from, cell);
            if (distance > bestDistance)
            {
                bestDistance = distance;
                bestCell = cell;
            }
        }

        return bestCell;
    }

    UnitMovement FindNearestEnemy(UnitMovement unit)
    {
        UnitMovement nearest = null;
        int nearestDistance = int.MaxValue;

        UnitMovement[] allUnits = FindObjectsOfType<UnitMovement>();
        foreach (UnitMovement other in allUnits)
        {
            if (other.CurrentCell == null) continue;
            if (unit.CurrentCell == null) continue;

            if (other.ownerIndex == 0)
            {
                int distance = hexGrid.GetDistance(unit.CurrentCell, other.CurrentCell);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = other;
                }
            }
        }

        return nearest;
    }

    void MoveRandomly(UnitMovement unit)
    {
        unit.HighlightReachableCells();
        List<HexCell> reachable = unit.ReachableCells();
        if (reachable.Count > 0)
        {
            HexCell randomCell = reachable[Random.Range(0, reachable.Count)];
            unit.MoveTo(randomCell);
        }
        unit.ClearReachableCells();
    }

    void PlaceComputerVillage(UnitMovement settler)
    {
        GameObject villagePrefab = FindObjectOfType<SelectionManager>().villagePrefab;
        if (villagePrefab != null)
        {
            GameObject newVillage = Instantiate(villagePrefab, settler.CurrentCell.Position + Constants.UnitOffset, Quaternion.identity);
            Village villageComponent = newVillage.GetComponent<Village>();
            if (villageComponent != null)
            {
                villageComponent.hexGrid = hexGrid;
                villageComponent.CurrentCell = settler.CurrentCell;
                villageComponent.ownerIndex = 1;
            }
            Destroy(settler.gameObject);
        }
    }

    private bool IsColorMatch(Color a, Color b, float tolerance = 0.01f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }

    // Training village tracker class
    class ComputerTrainingVillage
    {
        public GameObject village;
        public string unitType;
        public int startTurn;
        public int trainingDuration;
    }
}