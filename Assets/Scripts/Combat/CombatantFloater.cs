using System.Collections;
using TMPro;
using UnityEngine;

public class CombatantFloater : MonoBehaviour
{
    [SerializeField] private TMP_Text m_Text;

    IEnumerator RiseAndFade()
    {
        float duration = 0.5f;
        float elapsed = 0f;

        Vector3 startPos = transform.localPosition;
        Vector3 riseAmount = new Vector3(0, 50f, 0);
        
        Color startColor = m_Text.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            
            float t = elapsed / duration;

            transform.localPosition = startPos + riseAmount * t;

            Color c = startColor;
            c.a = 1 - t;
            
            m_Text.color = c;

            yield return null;
        }

        Destroy(gameObject);
    }

    public void Show(string text, Color color)
    {
        m_Text.text = text;
        m_Text.color = color;

        StartCoroutine(RiseAndFade());
    }
}
