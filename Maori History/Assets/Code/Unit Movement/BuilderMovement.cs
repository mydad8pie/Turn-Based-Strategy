using UnityEngine;

public class BuilderMovement : UnitMovement
{
    protected override void Start()
    {
        maxMoveRange = 2; // Set range specific to Builder
        base.Start();
    }
}
