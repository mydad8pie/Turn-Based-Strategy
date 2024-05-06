using UnityEngine;

public class HexGrid : MonoBehaviour
{
    public GameObject hexagonPrefab; // Prefab of the hexagon asset
    public int numRows = 5; // Number of rows in the grid
    public int numColumns = 5; // Number of columns in the grid
    public float hexagonWidth = 1.0f; // Width of the hexagon
    public float hexagonHeight = 1.0f; // Height of the hexagon

    private void Start()
    {
        CreateHexGrid();
    }

    private void CreateHexGrid()
    {
        float xOffset = hexagonWidth * Mathf.Sqrt(3f); // Horizontal spacing between hexagons
        float yOffset = hexagonHeight * 0.75f; // Vertical spacing between hexagons

        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numColumns; col++)
            {
                // Calculate position for each hexagon based on row and column
                float xPos = col * xOffset;
                float yPos = row * yOffset;

                // Apply an offset to every other row
                if (row % 2 != 0)
                {
                    xPos += xOffset * 0.5f;
                }

                // Instantiate hexagon at calculated position
                GameObject hexagon = Instantiate(hexagonPrefab, new Vector3(xPos, 0, yPos), Quaternion.identity);
                hexagon.transform.SetParent(transform); // Set the hexagon's parent to the grid
            }
        }
    }
}
