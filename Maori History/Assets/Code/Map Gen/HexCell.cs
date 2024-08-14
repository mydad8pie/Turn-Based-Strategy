using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HexCell : MonoBehaviour
{
    public Material defaultMaterial;
    public Material highlightMaterial;

    private Renderer cellRenderer;
    public HexCoordinates coordinates;



    public RectTransform uiRect;

    public HexGridChunk chunk;

    int elevation = int.MinValue;

    private Color color;


    public Color Color{
        get{
            return color;
        }
        set{
            if (color == value){
                return;
            }
            color = value;
            Refresh();
        }
        
    }
   // void Start(){
   //     cellRenderer = GetComponentInChildren<Renderer>();
   //     cellRenderer.material.color = defaultMaterial.color;
   // }
//
//   // public void Highlight(bool highlight){
    //    cellRenderer.material = highlight ? highlightMaterial : defaultMaterial;
    //}
    

    public int Elevation
    {
        get
        {
            return elevation;
        }
        set
        {
            if (elevation == value)
            {
                return;
            }
            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = elevation * -position.y;
            uiRect.localPosition = uiPosition;

            Refresh();
        }
    }

    public HexEdgeType GetEdgeType (HexDirection direction){
		return HexMetrics.GetEdgeType(elevation, neighbors[(int)direction].elevation);
	}

    public HexEdgeType GetEdgeType(HexCell otherCell)
    {
        return HexMetrics.GetEdgeType(elevation, otherCell.elevation);
    }

    [SerializeField] HexCell[] neighbors;

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }
    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    public Vector3 Position
    {
        get
        {
            return transform.localPosition;
        }
    }

    void Refresh (){
        if (chunk){
            chunk.Refresh();
            for (int i = 0; i < neighbors.Length; i++){
                HexCell neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk){
                    neighbor.chunk.Refresh();
                }
            }
        }
    }

    

    
  
}


