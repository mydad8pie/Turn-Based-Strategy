using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{

   MeshCollider meshCollider;
   Mesh hexMesh;
   static List<Vector3> vertices = new List<Vector3>();
   static List<Color> colors = new List<Color>();
   static List<int> triangles = new List<int>();
   

  

   void Awake() {
      GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
      meshCollider = gameObject.AddComponent<MeshCollider>();
      hexMesh.name = "Hex Mesh";
   }

   public void Triangulate (HexCell[] cells){
      hexMesh.Clear();
      vertices.Clear();
      colors.Clear();
      triangles.Clear();
      for (int i = 0; i < cells.Length; i++){
         Triangulate(cells[i]);
      }
      hexMesh.vertices = vertices.ToArray();
      hexMesh.colors = colors.ToArray();
      hexMesh.triangles = triangles.ToArray();
      hexMesh.RecalculateNormals();
      meshCollider.sharedMesh = hexMesh;
        
    }

    void Triangulate(HexCell cell){
      for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++){
         Triangulate(d, cell);
                }
    }
    void Triangulate(HexDirection direction, HexCell cell){
      Vector3 center = cell.Position;
      EdgeVertices e = new EdgeVertices(
         center + HexMetrics.GetFirstSolidCorner(direction), 
         center + HexMetrics.GetSecondSolidCorner(direction)
         );
      TriangulateEdgeFan(center, e, cell.Color);

      

      if (direction <= HexDirection.SE){
         TriangulateConnection(direction, cell, e);
      }
    }

    void TriangulateConnection (HexDirection direction, HexCell cell, EdgeVertices e1){
      HexCell neighbor = cell.GetNeighbor(direction);
      if (neighbor == null){
         return;
      }

      Vector3 bridge = HexMetrics.GetBridge(direction);
      bridge.y = neighbor.Position.y - cell.Position.y;
      EdgeVertices e2 = new EdgeVertices(
         e1.v1 + bridge,
         e1.v4 + bridge
      );


      

      if (cell.GetEdgeType(direction) == HexEdgeType.Slope){
         TriangulateEdgeTerraces(e1, cell, e2, neighbor);
      }
      else{
         TriangulateEdgeStrip(e1, cell.Color, e2, neighbor.Color);

      }

      HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
      if (direction <= HexDirection.E && nextNeighbor != null){
         Vector3 v5 = e1.v4 + HexMetrics.GetBridge(direction.Next());
         v5.y = nextNeighbor.Position.y;

         if (cell.Elevation <= neighbor.Elevation){
            if (cell.Elevation <= nextNeighbor.Elevation){
               TriangulateCorner(e1.v4, cell, e2.v4, neighbor, v5, nextNeighbor);
            }
            else{
               TriangulateCorner(v5, nextNeighbor, e1.v4, cell, e2.v4, neighbor);
            }
         }
         else if (neighbor.Elevation <= nextNeighbor.Elevation){
            TriangulateCorner(e2.v4, neighbor, v5, nextNeighbor, e1.v4, cell);
         }
         else{
            TriangulateCorner(v5, nextNeighbor, e2.v4, cell, e1.v4, neighbor);
         }
         AddTriangle(e1.v4, e2.v4, v5);
         AddTriangleColor(cell.Color, neighbor.Color, nextNeighbor.Color);
      }
    }

    void TriangulateEdgeTerraces (
      EdgeVertices begin, HexCell beginCell, 
    EdgeVertices end, HexCell endCell)
    {
      EdgeVertices e2 = EdgeVertices.TerraceLerp(begin, end, 1);
      Color c2 = HexMetrics.TerraceLerp(beginCell.Color, endCell.Color, 1);

      TriangulateEdgeStrip(begin, beginCell.Color, e2, c2);

      for (int i = 2; i < HexMetrics.terraceSteps; i++){
         EdgeVertices e1 = e2;
         Color c1 = c2;
         e2 = EdgeVertices.TerraceLerp(begin, end, i);
         c2 = HexMetrics.TerraceLerp(beginCell.Color, endCell.Color, i);
         TriangulateEdgeStrip(e1, c1, e2, c2);
      }

      TriangulateEdgeStrip(e2, c2, end, endCell.Color);
    }

    void AddTriangleColor(Color c1, Color c2 , Color c3){
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
    } 

   public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3){
      int vertexIndex = vertices.Count;
      vertices.Add(Perturb(v1));
      vertices.Add(Perturb(v2));
      vertices.Add(Perturb(v3));
      triangles.Add(vertexIndex);
      triangles.Add(vertexIndex + 1);
      triangles.Add(vertexIndex + 2);
   }

   void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4){
      int vertexIndex = vertices.Count;
      vertices.Add(Perturb(v1));
      vertices.Add(Perturb(v2));
      vertices.Add(Perturb(v3));
      vertices.Add(Perturb(v4));
      triangles.Add(vertexIndex);
      triangles.Add(vertexIndex + 2);
      triangles.Add(vertexIndex + 1);
      triangles.Add(vertexIndex + 1);
      triangles.Add(vertexIndex + 2);
      triangles.Add(vertexIndex + 3);
   }

   void AddQuadColor(Color c1, Color c2){
      colors.Add(c1);
      colors.Add(c1);
      colors.Add(c2);
      colors.Add(c2);
   }
   void AddQuadColor(Color c1, Color c2, Color c3, Color c4){
      colors.Add(c1);
      colors.Add(c2);
      colors.Add(c3);
      colors.Add(c4);
   }

    void TriangulateCorner (
      Vector3 bottom, HexCell bottomCell,
      Vector3 left, HexCell leftCell,
      Vector3 right, HexCell rightCell
    ){

      HexEdgeType leftEdgeType = bottomCell.GetEdgeType(leftCell);
      HexEdgeType rightEdgeType = bottomCell.GetEdgeType(rightCell);

      if (leftEdgeType == HexEdgeType.Slope){
         if (rightEdgeType == HexEdgeType.Slope){
            TriangulateCornerTerraces(bottom, bottomCell, left, leftCell, right, rightCell);
            
         }
         else if (rightEdgeType == HexEdgeType.Flat){
            TriangulateCornerTerraces(left, leftCell, right, rightCell, bottom, bottomCell);
         }
         else {
            TriangulateCornerTerracesCliff(bottom, bottomCell, left, leftCell, right, rightCell);
         }
      }
      else if (rightEdgeType == HexEdgeType.Slope){
         if (leftEdgeType == HexEdgeType.Flat){
            TriangulateCornerTerraces(right, rightCell, bottom, bottomCell, left, leftCell);
            
         }
         else {TriangulateCornerCliffTerraces(bottom, bottomCell, left, leftCell, right, rightCell);
         }
         
      }

      else if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope){
         if (leftCell.Elevation < rightCell.Elevation){
            TriangulateCornerCliffTerraces(right, rightCell, bottom, bottomCell, left, leftCell);
         }
         else{
            TriangulateCornerTerracesCliff(left, leftCell, right, rightCell, bottom, bottomCell);
         }
      }
      else{
         AddTriangle(bottom, left, right);
         AddTriangleColor(bottomCell.Color, leftCell.Color, rightCell.Color);
      }

      
    }

    void TriangulateCornerTerraces ( Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
    {
      Vector3 v3 = HexMetrics.TerraceLerp(begin, left, 1);
      Vector3 v4 = HexMetrics.TerraceLerp(begin, right, 1);
      Color c3 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, 1);
      Color c4 = HexMetrics.TerraceLerp(beginCell.Color, rightCell.Color, 1);

      AddTriangle(begin, v3, v4);
      AddTriangleColor(beginCell.Color, c3, c4);

      for (int i = 2; i < HexMetrics.terraceSteps; i++){
         Vector3 v1 = v3;
         Vector3 v2 = v4;
         Color c1 = c3;
         Color c2 = c4;
         v3 = HexMetrics.TerraceLerp(begin, left, i);
         v4 = HexMetrics.TerraceLerp(begin, right, i);
         c3 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, i);
         c4 = HexMetrics.TerraceLerp(beginCell.Color, rightCell.Color, i);
         AddQuad(v1, v2, v3, v4);
         AddQuadColor(c1, c2, c3, c4);
      }

      AddQuad(v3, v4, left, right);
      AddQuadColor(c3, c4, leftCell.Color, rightCell.Color);
    }

    void TriangulateCornerTerracesCliff (
      Vector3 begin, HexCell beginCell,
      Vector3 left, HexCell leftCell,
      Vector3 right, HexCell rightCell
    ){

      float b = 1f / (rightCell.Elevation - beginCell.Elevation);
      if (b < 0){
         b = -b;
      }
      Vector3 boundary = Vector3.Lerp(Perturb(begin), Perturb(right), b);
      Color boundaryColor = Color.Lerp(beginCell.Color, rightCell.Color, b);

      TriangulateBoundaryTriangle(begin, beginCell, left, leftCell, boundary, boundaryColor);

      if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope){
         TriangulateBoundaryTriangle(left, leftCell, right, rightCell, boundary, boundaryColor);
      }
      else{
         AddTriangleUnperturbed(Perturb(left), Perturb(right), boundary);
         AddTriangleColor(leftCell.Color, rightCell.Color, boundaryColor);
      }

    }

    void TriangulateBoundaryTriangle(
      Vector3 begin, HexCell beginCell,
      Vector3 left, HexCell leftCell,
      Vector3 boundary, Color boundaryColor
    ){
      Vector3 v2 = Perturb(HexMetrics.TerraceLerp(begin, left, 1));
      Color c2 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, 1);

      AddTriangleUnperturbed(Perturb(begin), v2, boundary);
      AddTriangleColor(beginCell.Color, c2, boundaryColor);

      for (int i = 2; i < HexMetrics.terraceSteps; i++){
         Vector3 v1 = v2;
         Color c1 = c2;
         v2 = Perturb(HexMetrics.TerraceLerp(begin, left, i));
         c2 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, i);
         AddTriangleUnperturbed(v1, v2, boundary);
         AddTriangleColor(c1, c2, boundaryColor);
      }

      AddTriangleUnperturbed(v2, Perturb(left), boundary);
      AddTriangleColor(c2, leftCell.Color, boundaryColor);
    }
    
    void TriangulateCornerCliffTerraces (
      Vector3 begin, HexCell beginCell,
      Vector3 left, HexCell leftCell,
      Vector3 right, HexCell rightCell
    ){
      float b = 1f / (leftCell.Elevation - beginCell.Elevation);
      if (b < 0){
         b = -b;
      }
      Vector3 boundary = Vector3.Lerp(Perturb(begin), Perturb(left), b);
      Color boundaryColor = Color.Lerp(beginCell.Color, leftCell.Color, b);

      TriangulateBoundaryTriangle(right, rightCell, begin, beginCell, boundary, boundaryColor);

      if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope){
         TriangulateBoundaryTriangle(left, leftCell, right, rightCell, boundary, boundaryColor);
      }
      else{
         AddTriangleUnperturbed(Perturb(left), Perturb(right), boundary);
         AddTriangleColor(leftCell.Color, rightCell.Color, boundaryColor);
      }
    }

    void TriangulateEdgeFan (Vector3 center, EdgeVertices edge, Color color){
      AddTriangle(center, edge.v1, edge.v2);
      AddTriangleColor(color, color, color);
      AddTriangle(center, edge.v2, edge.v3);
      AddTriangleColor(color, color, color);
      AddTriangle(center, edge.v3, edge.v4);
      AddTriangleColor(color, color, color);
    }
    
    void TriangulateEdgeStrip (EdgeVertices e1, Color c1, EdgeVertices e2, Color c2){
      AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
      AddQuadColor(c1, c2);
      AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
      AddQuadColor(c1, c2);
      AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
      AddQuadColor(c1, c2);
      
    }

    Vector3 Perturb(Vector3 position){
      Vector4 sample = HexMetrics.SampleNoise(position);
      position.x += (sample.x * 2f - 1f) * HexMetrics.cellPerturbStrength;    
      //position.y += (sample.y * 2f - 1f) * HexMetrics.cellPerturbStrength;
      position.z += (sample.z * 2f - 1f) * HexMetrics.cellPerturbStrength;
      return position;
      
      }

   void AddTriangleUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3){
      int vertexIndex = vertices.Count;
      vertices.Add(v1);
      vertices.Add(v2);
      vertices.Add(v3);
      triangles.Add(vertexIndex);
      triangles.Add(vertexIndex + 1);
      triangles.Add(vertexIndex + 2);
   }

  
   
  
}
