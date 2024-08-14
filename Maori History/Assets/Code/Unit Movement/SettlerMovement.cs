using UnityEngine;

public class SettlerMovement : UnitMovement
{
    protected override void Start()
    {
        maxMoveRange = 1; // Set range specific to Settler
        base.Start();
    }
}
