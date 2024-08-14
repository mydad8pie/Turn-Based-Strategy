using UnityEngine;

public class WarriorMovement : UnitMovement
{
    protected override void Start()
    {
        maxMoveRange = 3; // Set range specific to Warrior
        base.Start();
    }
}
