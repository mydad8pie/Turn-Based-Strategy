using UnityEngine;

[CreateAssetMenu(fileName = "New Grid Preset", menuName = "Hex Grid/Grid Preset")]
public class GridPreset : ScriptableObject
{
    public HexCellData[] cells;
}
