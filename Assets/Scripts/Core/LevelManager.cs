using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Private references
    private BoardManager m_BoardManager;
    private TurnManager m_TurnManager;
    private PlayerController m_PlayerController;

    private LevelBand m_CurrentBand;

    // Serialized references
    [SerializeField]
    private LevelTransitionManager m_LevelTransitionManager;

    [SerializeField]
    private BoardGenerator m_BoardGenerator;

    [SerializeField]
    private AudioClip m_LevelTransitionSFX;

    [SerializeField]
    private AudioClip m_GameOverSFX;

    [SerializeField]
    private BoxCollider2D m_ConfinerBounds;

    [SerializeField]
    private CinemachineConfiner2D m_Confiner;

    // Events
    public event Action<GameOverReason, int> GameOverTriggered;
    public event Action GameStartTriggered;

    // State
    private int m_CurrentLevel = 0;

    // Board dimensions
    private int m_BoardWidth = 8;
    private int m_BoardHeight = 8;

    // Readonly
    private static readonly Vector2Int m_PlayerSpawnCell = new Vector2Int(1, 1);

    // Properties
    public int CurrentLevel => m_CurrentLevel;
    public LevelBand CurrentBand => m_CurrentBand;

    void Start()
    {
        m_BoardManager = GameManager.Instance.BoardManager;
        m_TurnManager = GameManager.Instance.TurnManager;
        m_PlayerController = GameManager.Instance.PlayerController;

        m_PlayerController.Combatant.Depleted += OnPlayerDepleted;
        m_PlayerController.Combatant.Defeated += OnPlayerDefeated;

        NewLevel();
    }

    void OnDestroy()
    {
        if (m_PlayerController != null)
        {
            m_PlayerController.Combatant.Depleted -= OnPlayerDepleted;
            m_PlayerController.Combatant.Defeated -= OnPlayerDefeated;
        }
    }

    IEnumerator GoToLevelCoroutine(int level)
    {
        m_PlayerController.gameObject.SetActive(false);

        m_CurrentLevel = level;

        ResolveBand(m_CurrentLevel);

        AudioManager.Instance.PlaySFX(m_LevelTransitionSFX);

        yield return m_LevelTransitionManager.FadeOutCoroutine(m_CurrentLevel);

        m_BoardManager.Clean();
        m_BoardManager.Init(m_BoardWidth, m_BoardHeight);

        UpdateConfiner();

        m_BoardGenerator.GenerateBoard(
            m_BoardManager,
            m_TurnManager,
            m_PlayerController,
            m_CurrentLevel
        );

        m_PlayerController.gameObject.SetActive(true);
        m_PlayerController.Spawn(m_BoardManager, m_PlayerSpawnCell);

        yield return new WaitForSeconds(3f);

        yield return m_LevelTransitionManager.FadeInCoroutine();
    }

    void OnPlayerDefeated() => TriggerGameOver(GameOverReason.Defeated);

    void OnPlayerDepleted() => TriggerGameOver(GameOverReason.Depleted);

    void TriggerGameOver(GameOverReason reason)
    {
        m_PlayerController.GameOver();
        AudioManager.Instance.PlaySFX(m_GameOverSFX);
        GameOverTriggered?.Invoke(reason, m_CurrentLevel);
    }

    public void GoToLevel(int level) => StartCoroutine(GoToLevelCoroutine(level));

    public void NewLevel()
    {
        // Should restore stamina here...
        GoToLevel(m_CurrentLevel + 1);
    }

    public void ReloadCurrentLevel() => GoToLevel(m_CurrentLevel);

    public void StartNewGame()
    {
        GameStartTriggered?.Invoke();

        m_CurrentBand = null;
        m_CurrentLevel = 1;
        ResolveBand(m_CurrentLevel);

        m_BoardManager.Clean();
        m_BoardManager.Init(m_BoardWidth, m_BoardHeight);

        UpdateConfiner();

        m_BoardGenerator.GenerateBoard(
            m_BoardManager,
            m_TurnManager,
            m_PlayerController,
            m_CurrentLevel
        );

        m_PlayerController.Combatant.ResetState();

        m_PlayerController.Init();
        m_PlayerController.Spawn(m_BoardManager, m_PlayerSpawnCell);
        m_PlayerController.SetVisible(true);
    }

    void UpdateConfiner()
    {
        const float k_Padding = 1f;

        m_ConfinerBounds.offset = new Vector2(m_BoardWidth * 0.5f, m_BoardHeight * 0.5f);
        m_ConfinerBounds.size = new Vector2(
            m_BoardWidth + k_Padding * 2f,
            m_BoardHeight + k_Padding * 2f
        );
        m_Confiner.InvalidateBoundingShapeCache();
    }

    LevelBand ResolveBand(int level)
    {
        foreach (LevelBand band in GameManager.Instance.ProgressionSettings.Bands)
        {
            if (level >= band.MinLevel && level <= band.MaxLevel)
            {
                m_BoardWidth = band.BoardWidth;
                m_BoardHeight = band.BoardHeight;

                if (m_CurrentBand != null && m_CurrentBand != band)
                    m_PlayerController.Combatant.RefreshStats();

                m_CurrentBand = band;

                return band;
            }
        }

        Debug.LogWarning($"No band matched level {level}, reusing last band");

        return m_CurrentBand;
    }
}
