using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // References
    private BoardManager m_Board;
    private EnemyObject m_Combatant;
    private CombatantAnimator m_CombatantAnimator;

    // State
    private Vector2Int m_CellPosition;

    public EnemyObject Combatant => m_Combatant;
    public Vector2Int Cell => m_CellPosition;
    
    void Awake()
    {
        m_Combatant = GetComponent<EnemyObject>();
        m_CombatantAnimator = GetComponent<CombatantAnimator>();    
    }

    void Start()
    {
        GameManager.Instance.TurnManager.OnTick += OnTurnHappened;
    }

    void OnDestroy()
    {
        GameManager.Instance.TurnManager.OnTick += OnTurnHappened;
    }

    void OnTurnHappened()
    {
        
    }
}
