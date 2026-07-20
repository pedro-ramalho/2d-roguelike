using System;
using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Private references
    private BoardManager m_BoardManager;
    private TurnManager m_TurnManager;
    private PlayerController m_PlayerController;
    
    // Serialized references
    [SerializeField] private LevelTransitionManager m_LevelTransitionManager;

    // Events
    public event Action<GameOverReason, int> GameOverTriggered;
    public event Action GameStartTriggered;

    // State
    private int m_CurrentLevel = 0;

    // Readonly
    private static readonly Vector2Int m_PlayerSpawnCell = new Vector2Int(1, 1);

    void Start()
    {   
        m_BoardManager = GameManager.Instance.BoardManager;
        m_TurnManager = GameManager.Instance.TurnManager;
        m_PlayerController = GameManager.Instance.PlayerController;

        m_PlayerController.PlayerStats.Depleted += OnPlayerDepleted;
        m_PlayerController.Combatant.Defeated += OnPlayerDefeated;

        NewLevel();
    }

    void OnDestroy()
    {
        if (m_PlayerController != null)
        {
            m_PlayerController.PlayerStats.Depleted -= OnPlayerDepleted;
            m_PlayerController.Combatant.Defeated -= OnPlayerDefeated;
        }
    }

    IEnumerator NewLevelCoroutine()
    {
        m_PlayerController.gameObject.SetActive(false);

        yield return m_LevelTransitionManager.FadeOutCoroutine(m_CurrentLevel + 1);

        m_BoardManager.Clean();
        m_BoardManager.Init(m_TurnManager, m_PlayerController);
        
        m_PlayerController.gameObject.SetActive(true);
        m_PlayerController.Spawn(m_BoardManager, m_PlayerSpawnCell);

        m_CurrentLevel++;

        yield return new WaitForSeconds(3f);

        yield return m_LevelTransitionManager.FadeInCoroutine();
    }

    void OnPlayerDefeated() => TriggerGameOver(GameOverReason.Defeated);
    void OnPlayerDepleted() => TriggerGameOver(GameOverReason.Depleted);
    void TriggerGameOver(GameOverReason reason)
    {
        m_PlayerController.GameOver();
        GameOverTriggered?.Invoke(reason, m_CurrentLevel);
    }

    public void NewLevel() => StartCoroutine(NewLevelCoroutine());

    public void StartNewGame()
    {
        GameStartTriggered?.Invoke();

        m_CurrentLevel = 1;

        m_BoardManager.Clean();
        m_BoardManager.Init(m_TurnManager, m_PlayerController);

        m_PlayerController.Combatant.ResetState();
        m_PlayerController.PlayerStats.ResetState();

        m_PlayerController.Init();
        m_PlayerController.Spawn(m_BoardManager, m_PlayerSpawnCell);
        m_PlayerController.SetVisible(true);
    }
}
