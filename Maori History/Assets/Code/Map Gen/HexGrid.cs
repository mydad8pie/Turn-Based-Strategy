using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

public class HexGrid : MonoBehaviour
{
    
    public int width = 6;
    public int height = 6;

    public Color defaultColor = Color.white;
    public Color touchedColor = Color.magenta;

    public HexCell cellPrefab;

    HexCell[] cells;

    public Text cellLabelPrefab;

    Canvas gridCanvas;

    HexMesh hexMesh;

    void Awake() {

        cells = new HexCell[height * width];
        gridCanvas = GetComponentInChildren<Canvas>();

        hexMesh = GetComponentInChildren<HexMesh>();

        for (int z = 0, i = 0; z < height; z++){
            for (int x = 0; x < width; x++){
                CreateCell(x, z, i ++);
            }
            
        }
        
    }
    void CreateCell (int x, int z, int i){
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);
        
        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.color = defaultColor;

        if (x > 0){
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }
        if (z > 0){
            if ((z & 1) == 0){
                cell.SetNeighbor(HexDirection.SE, cells[i - width]);
                if (x > 0){
                    cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
                }

            }
            else{
                cell.SetNeighbor(HexDirection.SW, cells[i - width]);
                if (x < width - 1){
                    cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
                }
            }

        }

        Text Label = Instantiate<Text>(cellLabelPrefab);
        Label.rectTransform.SetParent(gridCanvas.transform, false);
        Label.rectTransform.anchoredPosition = 
            new Vector2(position.x, position.z);
        Label.text = cell.coordinates.ToStringOnSeparateLines();

        cell.uiRect = Label.rectTransform;

    }

    void Start () {
        hexMesh.Triangulate(cells);
        
    }

 
    public HexCell GetCell (Vector3 position){
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        return cells[index];
    }

    public void Refresh()
    {
        hexMesh.Triangulate(cells);
           
    }

    
}
