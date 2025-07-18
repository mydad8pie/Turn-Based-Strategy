using UnityEngine;
using UnityEngine.UI;

public class HexGridChunk : MonoBehaviour
{
    HexCell[] cells;

    HexMesh hexMesh;
    Canvas gridCanvas;

    void Awake(){
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();

        cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
        ShowUI(false);
    }


    public void AddCell (int index, HexCell cell){
        cells[index]=cell;
        cell.chunk = this;
        cell.transform.SetParent(transform, false);
        cell.uiRect.SetParent(gridCanvas.transform, false);    
    }

  public HexCell GetCell(HexCoordinates coordinates)
    {
    int index = coordinates.X + coordinates.Z * HexMetrics.chunkSizeX + coordinates.Z / 2;
    if (index >= 0 && index < cells.Length)
    {
        return cells[index];
    }
    return null;
    
    }

    
    public void Refresh(){
        enabled = true;
       
    }
    void LateUpdate(){
        hexMesh.Triangulate(cells);
        enabled = false;
    }

    public void ShowUI(bool visible){
        gridCanvas.gameObject.SetActive(visible);
    }


}