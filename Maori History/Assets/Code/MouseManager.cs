using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public UnitManager unitManager;
    public HexGrid hexGrid;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
        }
        else if (Input.GetMouseButtonDown(1))
        {
        }
    }

    void PlaceBuilder()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            HexCell cell = hexGrid.GetCell(hit.point);
            if (cell != null)
            {
                unitManager.SpawnBuilder(cell);
            }
        }
    }

    void PlaceWarrior()
    {
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            HexCell cell = hexGrid.GetCell(hit.point);
            if (cell != null)
            {
                unitManager.SpawnWarrior(cell);
            }
        }
    }
}
