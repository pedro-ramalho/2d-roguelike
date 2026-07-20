using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public enum GameOverReason { Depleted, Defeated }

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelTransitionManager m_LevelTransitionManager;

    private int m_CurrentLevel = 0;
    private Label m_FoodLabel;
    private Label m_GameOverMessage;
    private VisualElement m_GameOverPanel;
    private GameOverReason m_GameOverReason;    

    public TurnManager TurnManager { get; private set; }
    public BoardManager BoardManager;
    public PlayerController PlayerController;
    public UIDocument UIDoc;

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);

            return;
        }

        Instance = this;
        TurnManager = GetComponent<TurnManager>();
    }

    void Start()
    {
        NewLevel();

        m_FoodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");

        m_GameOverPanel = UIDoc.rootVisualElement.Q<VisualElement>("GameOverPanel");
        m_GameOverMessage = m_GameOverPanel.Q<Label>("GameOverMessage");

        m_GameOverPanel.style.visibility = Visibility.Hidden;

        PlayerController.PlayerStats.Depleted += OnPlayerDepleted;
        PlayerController.Combatant.Defeated += OnPlayerDefeated;
    }

    void OnDestroy()
    {
        if (PlayerController != null && PlayerController.Combatant != null)
        {
            PlayerController.PlayerStats.Depleted -= OnPlayerDepleted;
            PlayerController.Combatant.Defeated -= OnPlayerDefeated;
        }
    }

    void OnPlayerDefeated() => TriggerGameOver(GameOverReason.Defeated);
    void OnPlayerDepleted() => TriggerGameOver(GameOverReason.Depleted);
    void TriggerGameOver(GameOverReason reason)
    {
        PlayerController.GameOver();

        string levelString = m_CurrentLevel > 1 ? "levels" : "level";
        string reasonString = reason == GameOverReason.Depleted ? "You ran out of stamina!" : "You were defeated!";
        m_GameOverPanel.style.visibility = Visibility.Visible;
        m_GameOverMessage.text = $"Game Over! {reasonString}\n\nYou traveled through {m_CurrentLevel} {levelString}.\n\nPress Enter to restart.";
        m_GameOverReason = reason;
    }

    IEnumerator NewLevelCoroutine()
    {
        PlayerController.gameObject.SetActive(false);

        yield return m_LevelTransitionManager.FadeOutCoroutine(m_CurrentLevel + 1);

        BoardManager.Clean();
        BoardManager.Init();

        PlayerController.gameObject.SetActive(true);
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));

        m_CurrentLevel++;
        yield return new WaitForSeconds(3f);

        yield return m_LevelTransitionManager.FadeInCoroutine();
    }

    public void NewLevel() => StartCoroutine(NewLevelCoroutine());

    public void StartNewGame()
    {
        m_GameOverPanel.style.visibility = Visibility.Hidden;
    
        m_CurrentLevel = 1;
        m_FoodLabel.text = $"Stamina: {PlayerController.PlayerStats.Stamina}/{PlayerController.PlayerStats.MaxStamina}";

        BoardManager.Clean();
        BoardManager.Init();

        PlayerController.Combatant.ResetState();
        PlayerController.PlayerStats.ResetState();

        PlayerController.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));
        PlayerController.SetVisible(true);
    }
}
