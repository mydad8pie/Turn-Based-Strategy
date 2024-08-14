using System.Collections.Generic;
using UnityEngine;

public interface IUnit
{
    HexCell CurrentCell { get; set; }
    void MoveTo(HexCell targetCell);
    void HighlightReachableCells();
    bool IsReachable(HexCell cell);
    void ClearReachableCells();
    GameObject GameObject { get; }
}
