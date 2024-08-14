using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public GameObject builderPrefab;
    public GameObject warriorPrefab;
    public GameObject settlerPrefab;

    private List<UnitMovement> units = new List<UnitMovement>();

    public void SpawnBuilder(HexCell cell)
    {
        Vector3 spawnPosition = cell.Position + new Vector3(0, 3f, 0);
        GameObject builder = Instantiate(builderPrefab, spawnPosition, Quaternion.identity);
        BuilderMovement builderMovement = builder.GetComponent<BuilderMovement>();
        builderMovement.hexGrid = FindObjectOfType<HexGrid>();
        builderMovement.CurrentCell = cell;
        units.Add(builderMovement);
    }

    public void SpawnWarrior(HexCell cell)
    {
        Vector3 spawnPosition = cell.Position + new Vector3(0, 3f, 0);
        GameObject warrior = Instantiate(warriorPrefab, spawnPosition, Quaternion.identity);
        WarriorMovement warriorMovement = warrior.GetComponent<WarriorMovement>();
        warriorMovement.hexGrid = FindObjectOfType<HexGrid>();
        warriorMovement.CurrentCell = cell;
        units.Add(warriorMovement);
    }

    public void SpawnSettler(HexCell cell)
    {
        Vector3 spawnPosition = cell.Position + new Vector3(0, 3f, 0);
        GameObject settler = Instantiate(settlerPrefab, spawnPosition, Quaternion.identity);
        SettlerMovement settlerMovement = settler.GetComponent<SettlerMovement>();
        settlerMovement.hexGrid = FindObjectOfType<HexGrid>();
        settlerMovement.CurrentCell = cell;
        units.Add(settlerMovement);
    }

    public void DeselectAllUnits()
    {
        foreach (UnitMovement unit in units)
        {
            unit.ClearReachableCells();
        }
    }
}
