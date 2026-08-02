using System;
using System.Collections;
using UnityEngine;

public class CombatantAnimator : MonoBehaviour
{
    // Private references
    private ICombatant m_Combatant;
    private TurnManager m_TurnManager;
    private BoardManager m_BoardManager;

    // Components
    private SpriteRenderer m_SpriteRenderer;
    private Animator m_Animator;

    // Coroutines
    private Coroutine m_AttackCoroutine;
    private Coroutine m_WalkCoroutine;
    private Coroutine m_HurtCoroutine;
    private Coroutine m_DeathCoroutine;

    private readonly float m_AttackNudgeDistance = 0.3f;
    private readonly float m_AttackAnimationDuration = 0.2f;
    private readonly float m_WalkAnimationDuration = 0.25f;
    private readonly float m_HurtAnimationDuration = 0.15f;
    private readonly float m_DeathAnimationDuration = 0.3f;
    private readonly float m_ProjectileAnimationDuration = 0.5f;

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
            m_Combatant.AttackPerformed -= PlayAttackAnimation;
            m_Combatant.Damaged -= PlayHurtAnimation;
            m_Combatant.Defeated -= PlayDeathAnimation;
        }

        if (m_TurnManager != null)
            m_TurnManager.Unregister(this);
    }

    IEnumerator WalkAnimationCoroutine(Vector2Int targetCell)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = m_BoardManager.CellToWorld(targetCell);

        float elapsed = 0;
        float t;

        while (elapsed < m_WalkAnimationDuration)
        {
            elapsed += Time.deltaTime;

            t = elapsed / m_WalkAnimationDuration;

            transform.position = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        transform.position = endPos;

        m_WalkCoroutine = null;
    }

    IEnumerator AttackNudgeCoroutine(Vector2Int direction)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos =
            startPos + new Vector3(direction.x, direction.y, 0) * m_AttackNudgeDistance;

        float elapsed = 0;
        float t = 0;
        float singlePhaseDuration = m_AttackAnimationDuration / 2;

        // Ascent phase
        while (elapsed < singlePhaseDuration)
        {
            elapsed += Time.deltaTime;

            t = elapsed / singlePhaseDuration;

            transform.position = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        // Ascent phase complete, set position and reset elapsed & t
        transform.position = endPos;

        elapsed = 0f;
        t = 0f;

        // Begin descent phase
        while (elapsed < singlePhaseDuration)
        {
            elapsed += Time.deltaTime;

            t = elapsed / singlePhaseDuration;

            transform.position = Vector3.Lerp(endPos, startPos, t);

            yield return null;
        }

        // Descent phase complete, set position
        transform.position = startPos;

        m_AttackCoroutine = null;
    }

    IEnumerator HurtAnimationCoroutine()
    {
        Color startColor = m_SpriteRenderer.color;

        m_SpriteRenderer.color = Color.red;

        yield return new WaitForSeconds(m_HurtAnimationDuration);

        m_SpriteRenderer.color = startColor;

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

        Destroy(gameObject);
    }

    public void PlayWalkAnimation(Vector2Int targetCell, Vector2Int direction)
    {
        if (m_WalkCoroutine != null)
            StopCoroutine(m_WalkCoroutine);

        SetSpriteFacing(direction);
        m_WalkCoroutine = StartCoroutine(WalkAnimationCoroutine(targetCell));
    }

    public void SetSpriteFacing(Vector2Int direction)
    {
        if (direction == Vector2Int.left)
            m_SpriteRenderer.flipX = true;
        if (direction == Vector2Int.right)
            m_SpriteRenderer.flipX = false;
    }

    public void PlayHurtAnimation(DamageResult result)
    {
        if (m_HurtCoroutine != null)
            StopCoroutine(m_HurtCoroutine);

        if (result.HPLost > 0)
            m_HurtCoroutine = StartCoroutine(HurtAnimationCoroutine());
    }

    public void PlayDeathAnimation()
    {
        if (m_DeathCoroutine != null)
            StopCoroutine(m_DeathCoroutine);

        m_DeathCoroutine = StartCoroutine(DeathAnimationCoroutine());
    }

    public void PlayAttackAnimation(Vector2Int direction)
    {
        if (m_AttackCoroutine != null)
            StopCoroutine(m_AttackCoroutine);

        SetSpriteFacing(direction);
        m_Animator.SetTrigger(m_AttackHash);
        m_AttackCoroutine = StartCoroutine(AttackNudgeCoroutine(direction));
    }

    public void PlayProjectileAnimation(Projectile prefab, Vector3 target, Action onArrive)
    {
        Projectile p = Instantiate(prefab);
        p.Launch(transform.position, target, m_ProjectileAnimationDuration, onArrive);
    }

    public void Bind(ICombatant combatant, TurnManager turnManager, BoardManager boardManager)
    {
        m_Combatant = combatant;
        m_TurnManager = turnManager;
        m_BoardManager = boardManager;

        m_TurnManager.Register(this);

        m_Combatant.AttackPerformed += PlayAttackAnimation;
        m_Combatant.Damaged += PlayHurtAnimation;
        m_Combatant.Defeated += PlayDeathAnimation;
    }
}
