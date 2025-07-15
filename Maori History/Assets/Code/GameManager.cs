using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public HexGrid hexGrid;
    public string presetPath = "Assets/Chunks/ChunkPreset1.asset";
    public GameObject settlerPrefab; // Reference to the settler prefab
    public GameObject AnimalPrefab; // Reference to the animal prefab
    public Camera MainCamera; // Reference to the main camera

    void Start()
    {
        if (hexGrid != null)
        {
            hexGrid.LoadPreset(presetPath);
            SpawnSettler();
            SpawnAnimalsOnGreenCells(10); // Spawning 10 animals on green cells
        }
        else
        {
            Debug.LogError("HexGrid is not assigned.");
        }
    }

    void SpawnSettler()
    {
        if (settlerPrefab == null)
        {
            Debug.LogError("Settler prefab is not assigned.");
            return;
        }

        List<HexCell> yellowCells = FindYellowCells();
        if (yellowCells.Count > 0)
        {
            HexCell randomYellowCell = yellowCells[Random.Range(0, yellowCells.Count)];
            Vector3 spawnPosition = randomYellowCell.Position + new Vector3(0, 3f, 0);
            GameObject settler = Instantiate(settlerPrefab, spawnPosition, Quaternion.identity);

            SettlerMovement settlerMovement = settler.GetComponent<SettlerMovement>();
            if (settlerMovement != null)
            {
                settlerMovement.hexGrid = hexGrid;
                settlerMovement.CurrentCell = randomYellowCell;
            }

            FocusCameraOnSettler(settler);
        }
        else
        {
            Debug.LogWarning("No yellow cells found for spawning the settler.");
        }
    }

    void FocusCameraOnSettler(GameObject settler)
    {
        if (MainCamera == null)
        {
            Debug.LogError("MainCamera is not assigned.");
            return;
        }

        Vector3 settlerPosition = settler.transform.position;
        Vector3 cameraPosition = settlerPosition + new Vector3(0, 30f, -30f); // Adjust distance as needed
        MainCamera.transform.position = cameraPosition;
    }

    void SpawnAnimalsOnGreenCells(int numberOfAnimals)
    {
        if (AnimalPrefab == null)
        {
            Debug.LogError("Animal prefab is not assigned.");
            return;
        }

        List<HexCell> greenCells = FindGreenCells();
        if (greenCells.Count > 0)
        {
            for (int i = 0; i < numberOfAnimals; i++)
            {
                HexCell randomGreenCell = greenCells[Random.Range(0, greenCells.Count)];
                Vector3 spawnPosition = randomGreenCell.Position + new Vector3(0, 1f, 0);
                Instantiate(AnimalPrefab, spawnPosition, Quaternion.identity);
            }
        }
        else
        {
            Debug.LogWarning("No green cells found for spawning animals.");
        }
    }

    List<HexCell> FindYellowCells()
    {
        List<HexCell> yellowCells = new List<HexCell>();
        Color yellowColor = new Color(1f, 0.772f, 0f); // FFC500 in RGB

        foreach (HexCell cell in hexGrid.GetAllCells())
        {
            if (IsColorMatch(cell.Color, yellowColor))
            {
                yellowCells.Add(cell);
            }
        }
        return yellowCells;
    }

    List<HexCell> FindGreenCells()
    {
        List<HexCell> greenCells = new List<HexCell>();
        Color greenColor = new Color(0f, 0.47f, 0.02f); // 007804 in RGB

        foreach (HexCell cell in hexGrid.GetAllCells())
        {
            if (IsColorMatch(cell.Color, greenColor))
            {
                greenCells.Add(cell);
            }
        }
        return greenCells;
    }

    private bool IsColorMatch(Color a, Color b, float tolerance = 0.01f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }
}
