#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugController : MonoBehaviour
{
    [SerializeField]
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
            GameManager.Instance.LevelManager.NewLevel();

        if (Keyboard.current.hKey.wasPressedThisFrame)
            SkipToNextBand();

        if (Keyboard.current.jKey.wasPressedThisFrame)
            m_Player.IsGodMode = !m_Player.IsGodMode;

        if (Keyboard.current.lKey.wasPressedThisFrame)
            GameManager.Instance.LevelManager.ReloadCurrentLevel();
    }

    void SkipToNextBand()
    {
        LevelBand currentBand = GameManager.Instance.LevelManager.CurrentBand;
        if (currentBand == null)
            return;

        GameManager.Instance.LevelManager.GoToLevel(currentBand.MaxLevel + 1);
    }
}

#endif
