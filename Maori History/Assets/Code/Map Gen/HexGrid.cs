using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Xml.Serialization;
using UnityEditor;
using System.Diagnostics;
using System.IO;
using Unity.VisualScripting; // For AssetDatabase (Editor-only code)

public class HexGrid : MonoBehaviour
{
    
    public Color defaultColor = Color.white;
    public Color touchedColor = Color.magenta;

    public GameObject Building;

    public int chunkCountX = 4, chunkCountZ = 3;

    public HexCell cellPrefab;

    public HexCell[] cells;

    public Text cellLabelPrefab;

    public HexGridChunk chunkPrefab;

    public Texture2D noiseSource;

    HexGridChunk[] chunks;

    // Add these as class-level variables
    int cellCountX, cellCountZ;

    private Dictionary<HexCell, IUnit> unitsOnCells = new Dictionary<HexCell, IUnit>();

    public HexCell[] GetAllCells()
    {
        return cells;
    }

    void OnEnable()
    {
        HexMetrics.noiseSource = noiseSource;
    }

    void Awake()
    {
        HexMetrics.noiseSource = noiseSource;

        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

        CreateChunks();
        CreateCells();
    }

    void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for (int z = 0, i = 0; z < chunkCountZ; z++)
        {
            for (int x = 0; x < chunkCountX; x++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }

    void CreateCells()
    {
        cells = new HexCell[cellCountX * cellCountZ];

        for (int z = 0, i = 0; z < cellCountZ; z++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate(cellPrefab);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.Color = defaultColor;

        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }
        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                if (x < cellCountX - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                }
            }
        }

        Text label = Instantiate(cellLabelPrefab);
        label.rectTransform.anchoredPosition =
            new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();

        cell.uiRect = label.rectTransform;

        addCellToChunk(x, z, cell);
    }

    void addCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
    }

    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        return GetCell(coordinates);
    }

    public HexCell GetCell(HexCoordinates coordinates)
    {
        int z = coordinates.Z;
        if (z < 0 || z >= cellCountZ)
        {
            return null;
        }
        int x = coordinates.X + z / 2;
        if (x < 0 || x >= cellCountX)
        {
            return null;
        }
        return cells[x + z * cellCountX];
    }

    public void ShowUI(bool visible)
    {
        for (int i = 0; i < chunks.Length; i++)
        {
            chunks[i].ShowUI(visible);
        }
    }

    public void ApplyChunkPreset(HexGridChunk chunk, ChunkPreset preset)
    {
        foreach (HexCellData cellData in preset.cells)
        {
            HexCell cell = chunk.GetCell(cellData.coordinates);
            cell.Color = cellData.color;
            cell.Elevation = cellData.elevation;
        }
        chunk.Refresh();
    }

    public void SaveChunkPreset(HexGridChunk chunk, string presetName)
    {
       ChunkPreset preset = ScriptableObject.CreateInstance<ChunkPreset>();
        List<HexCellData> cellDataList = new List<HexCellData>();

        foreach (HexCell cell in chunk.GetComponentsInChildren<HexCell>())
        {
            HexCellData cellData = new HexCellData
            {
                coordinates = cell.coordinates,
                color = cell.Color,
                elevation = cell.Elevation
            };
            cellDataList.Add(cellData);
        }

        preset.cells = cellDataList.ToArray();

        string path = $"Assets/Chunks/{presetName}.asset";
        System.IO.Directory.CreateDirectory("Assets/Chunks"); // Ensure the directory exists
        AssetDatabase.CreateAsset(preset, path);
        AssetDatabase.SaveAssets();
    }

    public void SaveGrid(string presetName){

         GridPreset preset = ScriptableObject.CreateInstance<GridPreset>();
        List<HexCellData> cellDataList = new List<HexCellData>();

        foreach (HexCell cell in cells)
        {
            HexCellData cellData = new HexCellData
            {
                coordinates = cell.coordinates,
                color = cell.Color,
                elevation = cell.Elevation
            };
            cellDataList.Add(cellData);
        }

        preset.cells = cellDataList.ToArray();

        string path = $"Assets/Chunks/{presetName}.asset";
        System.IO.Directory.CreateDirectory("Assets/Chunks"); // Ensure the directory exists
        AssetDatabase.CreateAsset(preset, path);
        AssetDatabase.SaveAssets();
    }

    public void ApplyGridPreset(GridPreset preset){
       foreach (HexCellData cellData in preset.cells)
        {
            HexCell cell = GetCell(cellData.coordinates);
            if (cell != null)
            {
                cell.Color = cellData.color;
                cell.Elevation = cellData.elevation;
            }
        }

        foreach (HexGridChunk chunk in chunks)
        {
            chunk.Refresh();
        }
    }

    public void LoadPreset(string presetPath){
        GridPreset preset = AssetDatabase.LoadAssetAtPath<GridPreset>(presetPath);

        if (preset != null)
        {
            ApplyGridPreset(preset);
        }
    }

    public void RegisterUnit(HexCell cell, IUnit unit){
        if(unitsOnCells.ContainsKey(cell)){
            UnityEngine.Debug.LogWarning("Cell already has a unit");
            return;
        }

        unitsOnCells[cell] = unit;
        unit.CurrentCell = cell;
    }

    public void UnregisterUnit(HexCell cell){
        if(unitsOnCells.ContainsKey(cell)){
            unitsOnCells[cell].CurrentCell = null;
            unitsOnCells.Remove(cell);
        }
    }
    public bool IsCellOccupied(HexCell cell){
        return unitsOnCells.ContainsKey(cell);
    }

    public IUnit GetUnitAtCell(HexCell cell){
        unitsOnCells.TryGetValue(cell, out var unit);
        return unit;
    }

    public int GetDistance(HexCell a, HexCell b){
        int dx = Mathf.Abs(a.coordinates.X - b.coordinates.X);
        int dy = Mathf.Abs(a.coordinates.Y - b.coordinates.Y);
        int dz = Mathf.Abs(a.coordinates.Z - b.coordinates.Z);

        return Mathf.Max(dx, dy, dz);
    }
    
}
