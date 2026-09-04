#if UNITY_EDITOR
using Combat;
using Core;
using Level;
using Status;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Debug
{
    public class DebugController : MonoBehaviour
    {
        [SerializeField]
        private Combatant m_Player;

        void Update()
        {
            /*
            -- KEYS EXPLANATION --
            -> V: apply EMPOWERED with duration of 2
            -> B: apply VULNERABLE with duration of 3
            -> N: apply WEAK with duration of 4
            -> M: apply STUNNED with duration of 1
            -> O: take 1 damage
            -> P: heal 1 point
            -> G: skip to the next level
            -> H: skip to the next band
            -> J: toggle God mode (no damage)
            -> L: reload current level
            -> Y: die
        */
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

            if (Keyboard.current.yKey.wasPressedThisFrame)
                m_Player.TakeDamage(m_Player.MaxHP);
        }

        void SkipToNextBand()
        {
            LevelBand currentBand = GameManager.Instance.LevelManager.CurrentBand;
            if (currentBand == null)
                return;

            GameManager.Instance.LevelManager.GoToLevel(currentBand.MaxLevel + 1);
        }
    }
}

#endif
