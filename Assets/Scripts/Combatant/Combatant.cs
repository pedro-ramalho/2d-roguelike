using UnityEngine;

public abstract class Combatant : MonoBehaviour
{
    [SerializeField]
    private int m_MaxHP = 10;

    public int HP { get; private set; }
    public int Block { get; private set; }

    void Awake()
    {
        HP = m_MaxHP;
        Block = 0;
    }

    public void ChangeHealth(int amount) =>HP = Mathf.Clamp(HP + amount, 0, m_MaxHP);

    public void ChangeBlock(int amount) => Block = Mathf.Max(Block + amount, 0);
}
