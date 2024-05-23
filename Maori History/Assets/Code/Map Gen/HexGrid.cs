using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections.Generic;

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

        Text Label = Instantiate<Text>(cellLabelPrefab);
        Label.rectTransform.SetParent(gridCanvas.transform, false);
        Label.rectTransform.anchoredPosition = 
            new Vector2(position.x, position.z);
        Label.text = cell.coordinates.ToStringOnSeparateLines();

    }

    void Start () {
        hexMesh.Triangulate(cells);
        
    }

 
    public void ColorCell(Vector3 position, Color color){
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        HexCell cell = cells[index];
        cell.color = color;
        hexMesh.Triangulate(cells);
        
    }

    
}
