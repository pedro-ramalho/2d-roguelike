using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusEffectSlot : MonoBehaviour
{
    [SerializeField] private Image m_Icon;
    [SerializeField] private TMP_Text m_Duration;

    private StatusEffect m_Effect;

    public StatusEffectType Type => m_Effect.Type;

    static Color ColorFor(StatusEffectType type) => type switch
    {
        StatusEffectType.Stunned => new Color(1.00f, 0.90f, 0.20f),
        StatusEffectType.Vulnerable => new Color(1.00f, 0.55f, 0.15f),
        StatusEffectType.Weak => new Color(0.55f, 0.55f, 0.55f),
        StatusEffectType.Empowered => new Color(0.90f, 0.70f, 0.20f),
        _ => Color.white  
    };

    public void Refresh() => m_Duration.text = m_Effect.Duration.ToString();

    public void Bind(StatusEffect effect)
    {
        m_Effect = effect;
        m_Icon.color = ColorFor(effect.Type);

        Refresh();
    }
}
