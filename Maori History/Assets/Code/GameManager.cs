using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public HexGrid hexGrid;
    public string presetPath = "Assets/Chunks/ChunkPreset1.asset";
    
    void Start(){
        if (hexGrid != null){
            hexGrid.LoadPreset(presetPath);
        }

    }
            
    
}

