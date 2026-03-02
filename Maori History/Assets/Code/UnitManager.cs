using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public GameObject builderPrefab;
    public GameObject warriorPrefab;
    public GameObject settlerPrefab;

    private List<UnitMovement> units = new List<UnitMovement>();

    public void SpawnBuilder(HexCell cell, int ownerIndex = 0)
    {
        Vector3 spawnPosition = cell.Position + new Vector3(0, 3f, 0);
        GameObject builder = Instantiate(builderPrefab, spawnPosition, Quaternion.identity);
        BuilderMovement builderMovement = builder.GetComponent<BuilderMovement>();
        builderMovement.hexGrid = FindObjectOfType<HexGrid>();
        builderMovement.CurrentCell = cell;
        builderMovement.ownerIndex = ownerIndex;
        units.Add(builderMovement);

        // Explicitly register so the cell is blocked immediately
        builderMovement.hexGrid.RegisterUnit(cell, builderMovement);
    }

    public void SpawnWarrior(HexCell cell, int ownerIndex = 0)
    {
        Vector3 spawnPosition = cell.Position + new Vector3(0, 3f, 0);
        GameObject warrior = Instantiate(warriorPrefab, spawnPosition, Quaternion.identity);
        WarriorMovement warriorMovement = warrior.GetComponent<WarriorMovement>();
        warriorMovement.hexGrid = FindObjectOfType<HexGrid>();
        warriorMovement.CurrentCell = cell;
        warriorMovement.ownerIndex = ownerIndex;
        units.Add(warriorMovement);
        // Explicitly register so the cell is blocked immediately
        warriorMovement.hexGrid.RegisterUnit(cell, warriorMovement);
    }

    public void SpawnSettler(HexCell cell, int ownerIndex = 0)
    {
        Vector3 spawnPosition = cell.Position + new Vector3(0, 3f, 0);
        GameObject settler = Instantiate(settlerPrefab, spawnPosition, Quaternion.identity);
        SettlerMovement settlerMovement = settler.GetComponent<SettlerMovement>();
        settlerMovement.hexGrid = FindObjectOfType<HexGrid>();
        settlerMovement.CurrentCell = cell;
        settlerMovement.ownerIndex = ownerIndex;
        units.Add(settlerMovement);
        // Explicitly register so the cell is blocked immediately
        settlerMovement.hexGrid.RegisterUnit(cell, settlerMovement);
    }

    public void DeselectAllUnits()
    {
        foreach (UnitMovement unit in units)
        {
            unit.ClearReachableCells();
        }
    }
}
