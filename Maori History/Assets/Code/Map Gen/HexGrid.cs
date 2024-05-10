using UnityEngine.UI;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    
    public int width = 6;
    public int height = 6;

    public HexCell cellPrefab;

    HexCell[] cells;

    public Text cellLabelPrefab;

    Canvas gridCanvas;

    void Awake() {
        cells = new HexCell[height * width];
        gridCanvas = GetComponentInChildren<Canvas>();

        for (int z = 0, i = 0; z < height; z++){
            for (int x = 0; x < width; x++){
                CreateCell(x, z, i ++);
            }
            
        }
        
    }
    void CreateCell (int x, int z, int i){
        Vector3 position;
        position.x = x * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);
        
        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;

        Text Label = Instantiate<Text>(cellLabelPrefab);
        Label.rectTransform.SetParent(gridCanvas.transform, false);
        Label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        Label.text = x.ToString() + "\n" + z.ToString();

    }
    
}
