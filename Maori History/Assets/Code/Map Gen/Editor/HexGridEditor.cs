using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexGrid))]
public class HexGridEditor : Editor
{
    private string presetName = "GridPreset1"; // Default preset name
    private HexGridChunk chunk;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        HexGrid hexGrid = (HexGrid)target;

        // Get the chunk from the scene
        chunk = (HexGridChunk)EditorGUILayout.ObjectField("Hex Grid Chunk", chunk, typeof(HexGridChunk), true);
        presetName = EditorGUILayout.TextField("Preset Name", presetName);

        if (GUILayout.Button("Apply Chunk Preset"))
        {
            // Load the preset from the predefined path
            ChunkPreset preset = AssetDatabase.LoadAssetAtPath<ChunkPreset>($"Assets/Chunks/{presetName}.asset");

            if (preset != null && chunk != null)
            {
                hexGrid.ApplyChunkPreset(chunk, preset);
                EditorUtility.SetDirty(hexGrid); // Mark HexGrid as dirty to ensure changes are saved
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please ensure the Chunk Preset path is correct and a Hex Grid Chunk is assigned.", "OK");
            }
        }

        if (GUILayout.Button("Save Chunk Preset"))
        {
            if (chunk != null)
            {
                hexGrid.SaveChunkPreset(chunk, presetName);
                AssetDatabase.SaveAssets();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please assign a Hex Grid Chunk.", "OK");
            }
        }

        if (GUILayout.Button("Save Grid Preset"))
        {
            hexGrid.SaveGrid(presetName);
            AssetDatabase.SaveAssets();
        }

        if (GUILayout.Button("Load Grid Preset"))
        {
            GridPreset preset = AssetDatabase.LoadAssetAtPath<GridPreset>($"Assets/Chunks/{presetName}.asset");

            if (preset != null)
            {
                hexGrid.ApplyGridPreset(preset);
                }
            }
            
        }
        
}
