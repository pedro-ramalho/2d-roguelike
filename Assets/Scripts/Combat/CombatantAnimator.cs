using System;
using System.Collections;
using Board;
using Core;
using UnityEngine;

namespace Combat
{
    public class CombatantAnimator : MonoBehaviour
    {
        [Header("Death Handling")]
        [SerializeField]
        private bool m_DestroyOnDeath = true;

        [Header("Sprite Facing")]
        [SerializeField]
        private bool m_IsSpriteFacingRight = true;

        // Private references
        private Combatant m_Combatant;
        private CombatantStats m_Stats;
        private TurnManager m_TurnManager;

        // Components
        private SpriteRenderer m_SpriteRenderer;
        private Animator m_Animator;
        private Color m_OriginalColor;
        private bool m_IsHurt;

        // Coroutines
        private Coroutine m_AttackCoroutine;
        private Coroutine m_WalkCoroutine;
        private Coroutine m_HurtCoroutine;
        private Coroutine m_DeathCoroutine;

        private const float m_AttackNudgeDistance = 0.2f;
        private const float m_AttackAnimationDuration = 0.15f;
        private const float m_WalkAnimationDuration = 0.20f;
        private const float m_HurtAnimationDuration = 0.10f;
        private const float m_DeathAnimationDuration = 0.2f;
        private const float m_ProjectileAnimationDuration = 0.5f;

        private static readonly int m_AttackHash = Animator.StringToHash("Attack");

        public float WalkDuration => m_WalkAnimationDuration;
        public bool IsBusy =>
            m_WalkCoroutine != null
            || m_AttackCoroutine != null
            || m_HurtCoroutine != null
            || m_DeathCoroutine != null;

        void Awake()
        {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_Animator = GetComponent<Animator>();
            m_OriginalColor = m_SpriteRenderer.color;

            m_TurnManager = GameManager.Instance.TurnManager;
            m_Combatant = GetComponent<Combatant>();
            m_Stats = GetComponent<CombatantStats>();

            m_TurnManager.Register(this);

            m_Combatant.AttackPerformed += OnCombatantAttack;
            m_Combatant.Defeated += OnCombatantDeath;
            m_Combatant.Moved += OnCombatantMove;

            m_Stats.Damaged += OnCombatantHurt;
        }

        void OnDisable()
        {
            m_WalkCoroutine = null;
            m_AttackCoroutine = null;
            m_HurtCoroutine = null;
            m_DeathCoroutine = null;
        }

        void OnDestroy()
        {
            if (m_Combatant != null)
            {
                m_Combatant.Moved -= OnCombatantMove;
                m_Combatant.AttackPerformed -= OnCombatantAttack;
                m_Combatant.Defeated -= OnCombatantDeath;
            }

            if (m_Stats != null)
                m_Stats.Damaged -= OnCombatantHurt;

            if (m_TurnManager != null)
                m_TurnManager.Unregister(this);
        }

        IEnumerator MoveCoroutine(Vector3 start, Vector3 target, float duration)
        {
            float elapsed = 0;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float t = elapsed / duration;

                transform.position = Vector3.Lerp(start, target, t);

                yield return null;
            }

            transform.position = target;
        }

        IEnumerator WalkAnimationCoroutine(Vector3 target)
        {
            Vector3 start = transform.position;

            yield return MoveCoroutine(start, target, m_WalkAnimationDuration);

            m_WalkCoroutine = null;
        }

        IEnumerator AttackNudgeCoroutine(Vector2Int direction)
        {
            Vector3 start = transform.position;
            Vector3 target =
                start + new Vector3(direction.x, direction.y, 0) * m_AttackNudgeDistance;

            float duration = m_AttackAnimationDuration / 2;

            // Ascent
            yield return MoveCoroutine(start, target, duration);

            // Descent
            yield return MoveCoroutine(target, start, duration);

            m_AttackCoroutine = null;
        }

        IEnumerator HurtAnimationCoroutine()
        {
            if (!m_IsHurt)
                m_OriginalColor = m_SpriteRenderer.color;

            m_IsHurt = true;

            m_SpriteRenderer.color = Color.red;

            yield return new WaitForSeconds(m_HurtAnimationDuration);

            m_SpriteRenderer.color = m_OriginalColor;

            m_IsHurt = false;

            m_HurtCoroutine = null;
        }

        IEnumerator DeathAnimationCoroutine()
        {
            float elapsed = 0;

            Color startColor = m_SpriteRenderer.color;

            while (elapsed <= m_DeathAnimationDuration)
            {
                Color c = startColor;

                float t = elapsed / m_DeathAnimationDuration;

                c.a = 1 - t;

                m_SpriteRenderer.color = c;

                elapsed += Time.deltaTime;

                yield return null;
            }

            m_DeathCoroutine = null;

            if (m_DestroyOnDeath)
                Destroy(gameObject);
        }

        public void OnCombatantMove(Vector3 targetPosition, Vector2Int direction)
        {
            if (m_WalkCoroutine != null)
                StopCoroutine(m_WalkCoroutine);

            SetSpriteFacing(direction);
            AudioManager.Instance.PlayCombatantFootstepSFX();
            m_WalkCoroutine = StartCoroutine(WalkAnimationCoroutine(targetPosition));
        }

        public void SetSpriteFacing(Vector2Int direction)
        {
            if (direction == Vector2Int.left)
                m_SpriteRenderer.flipX = m_IsSpriteFacingRight;
            if (direction == Vector2Int.right)
                m_SpriteRenderer.flipX = !m_IsSpriteFacingRight;
        }

        public void OnCombatantHurt(DamageResult result)
        {
            if (result.BlockLost > 0)
                AudioManager.Instance.PlayCombatantBlockedSFX();

            if (result.HPLost > 0)
            {
                AudioManager.Instance.PlayCombatantHurtSFX();

                if (m_HurtCoroutine != null)
                    StopCoroutine(m_HurtCoroutine);

                m_HurtCoroutine = StartCoroutine(HurtAnimationCoroutine());
            }
        }

        public void OnCombatantDeath()
        {
            if (m_DeathCoroutine != null)
                StopCoroutine(m_DeathCoroutine);

            AudioManager.Instance.PlayCombatantDeathSFX();
            m_DeathCoroutine = StartCoroutine(DeathAnimationCoroutine());
        }

        public void OnCombatantAttack(Vector2Int direction)
        {
            if (m_AttackCoroutine != null)
                StopCoroutine(m_AttackCoroutine);

            SetSpriteFacing(direction);
            m_Animator.SetTrigger(m_AttackHash);
            m_AttackCoroutine = StartCoroutine(AttackNudgeCoroutine(direction));
        }

        public void PlayProjectileAnimation(
            Projectile.Projectile prefab,
            Vector3 target,
            Action onArrive
        )
        {
            Projectile.Projectile p = Instantiate(prefab);
            p.Launch(transform.position, target, m_ProjectileAnimationDuration, onArrive);
        }
    }
}
