using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Village : MonoBehaviour

{

    public HexCell CurrentCell { get; set; }
    public HexGrid hexGrid;

    public int ownerIndex = 0; // 0 = player, 1 = computer


    void Start()
    {
        
    }

}
