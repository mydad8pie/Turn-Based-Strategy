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

        if(!attacker.gameObject.name.Contains("Warrior"))
        {
            Debug.Log(attacker.gameObject.name + " cannot attack - only Warriors can deal damage.");
            return false;
        }

        Health defenderHealth = defender.GetComponent<Health>();
        if (defenderHealth == null)
        {
            Debug.Log(defender.gameObject.name + " has no Health component and cannot be attacked.");
            return false;
        }

        Debug.Log(attacker.gameObject.name + " attacks " + defender.gameObject.name + " for 5 damage.");
        defenderHealth.TakeDamage(5);
        return true;
    }
}