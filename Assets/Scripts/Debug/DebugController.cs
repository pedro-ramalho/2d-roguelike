#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugController : MonoBehaviour
{
    [SerializeReference]
    private Combatant m_Player;

    void Update()
    {
        if (Keyboard.current.vKey.wasPressedThisFrame)
            m_Player.ApplyStatusEffect(new StatusEffectEmpowered(2));

        if (Keyboard.current.bKey.wasPressedThisFrame)
            m_Player.ApplyStatusEffect(new StatusEffectVulnerable(3));

        if (Keyboard.current.nKey.wasPressedThisFrame)
            m_Player.ApplyStatusEffect(new StatusEffectWeak(4));

        if (Keyboard.current.mKey.wasPressedThisFrame)
            m_Player.ApplyStatusEffect(new StatusEffectStunned(1));

        if (Keyboard.current.oKey.wasPressedThisFrame)
            m_Player.TakeDamage(1);

        if (Keyboard.current.pKey.wasPressedThisFrame)
            m_Player.Heal(1);

        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            // Jump forward one level
            GameManager.Instance.LevelManager.NewLevel();
        }

        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            // Skip to next band
        }

        if (Keyboard.current.jKey.wasPressedThisFrame)
        {
            // Toggle god mode
        }

        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            // Reload current level
        }
    }

    void SkipToNextBand() { }
}

#endif
