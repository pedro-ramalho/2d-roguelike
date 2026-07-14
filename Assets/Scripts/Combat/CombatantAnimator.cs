using System.Collections;
using UnityEngine;

public class CombatantAnimator : MonoBehaviour
{
    private Coroutine m_WalkCoroutine;

    private readonly float m_WalkAnimationDuration = 0.5f;

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

    IEnumerator PlayAttackAnimation()
    {
        yield return null;
    }

    IEnumerator PlayHurtAnimation()
    {
        yield return null;
    }

    IEnumerator PlayDeathAnimation()
    {
        yield return null;
    }

    public void PlayWalkAnimation(Vector2Int targetCell)
    {
        if (m_WalkCoroutine != null)
            StopCoroutine(m_WalkCoroutine);

        m_WalkCoroutine = StartCoroutine(WalkAnimationCoroutine(targetCell));
    }
}
