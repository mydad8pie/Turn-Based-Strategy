using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Returns true if combat happened
    public bool TryAttack(UnitMovement attacker, UnitMovement defender)
    {
        if (attacker == null || defender == null) return false;

        // Can't attack your own units
        if (attacker.ownerIndex == defender.ownerIndex) return false;

        Debug.Log(attacker.gameObject.name + " attacks " + defender.gameObject.name);

        // For now combat is simple — attacker always wins
        // We can add health later
        KillUnit(defender);
        return true;
    }

    void KillUnit(UnitMovement unit)
    {
        Debug.Log(unit.gameObject.name + " has been defeated!");
        Destroy(unit.gameObject);
    }
}