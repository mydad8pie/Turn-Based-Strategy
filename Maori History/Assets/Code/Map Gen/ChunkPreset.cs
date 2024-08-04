using UnityEngine;

[CreateAssetMenu(fileName = "ChunkPreset", menuName = "HexGrid/ChunkPreset")]
public class ChunkPreset : ScriptableObject
{
    public HexCellData[] cells;
}

[System.Serializable]
public class HexCellData
{
    public HexCoordinates coordinates;
    public Color color;
    public int elevation;
}
