using UnityEngine;

public class EliteModifier : MonoBehaviour
{
    void Awake()
    {
        Combatant combatant = GetComponent<Combatant>();
        combatant.ApplyStatMultiplier(2f);
    }
}
