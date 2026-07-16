using System.Collections;
using UnityEngine;

public class CombatantAnimator : MonoBehaviour
{   
    private ICombatant m_Combatant;

    // Components
    private SpriteRenderer m_SpriteRenderer;

    // Coroutines
    private Coroutine m_WalkCoroutine;
    private Coroutine m_HurtCoroutine; 

    private readonly float m_WalkAnimationDuration = 0.25f;
    private readonly float m_HurtAnimationDuration = 0.15f;
    private readonly float m_DeathAnimationDuration = 0.3f;

    void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnDestroy()
    {
        m_Combatant.Damaged -= PlayHurtAnimation;
    }

    IEnumerator WalkAnimationCoroutine(Vector2Int targetCell)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = GameManager.Instance.BoardManager.CellToWorld(targetCell);

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
        yield return null;
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

        Destroy(gameObject);
    }

    public void PlayWalkAnimation(Vector2Int targetCell)
    {
        if (m_WalkCoroutine != null)
            StopCoroutine(m_WalkCoroutine);

        m_WalkCoroutine = StartCoroutine(WalkAnimationCoroutine(targetCell));
    }

    public void PlayHurtAnimation(DamageResult result)
    {
        if (m_HurtCoroutine != null)
            StopCoroutine(m_HurtCoroutine);

        if (result.HPLost > 0)
            m_HurtCoroutine = StartCoroutine(HurtAnimationCoroutine());
    }

    public void PlayDeathAnimation() => StartCoroutine(DeathAnimationCoroutine());

    public void PlayAttackAnimation(Vector2Int direction) => StartCoroutine(AttackNudgeCoroutine(direction));

    public void Bind(ICombatant combatant)
    {
        m_Combatant = combatant;

        m_Combatant.AttackPerformed += PlayAttackAnimation;
        m_Combatant.Damaged += PlayHurtAnimation;
        m_Combatant.Defeated += PlayDeathAnimation;
    }
}
