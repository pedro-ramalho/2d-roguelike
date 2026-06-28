using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private int m_FoodAmount = 5;
    private int m_CurrentLevel = 0;
    private Label m_FoodLabel;
    private Label m_GameOverMessage;
    private VisualElement m_GameOverPanel;

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
    }

    void Start()
    {
        TurnManager = new TurnManager();
        TurnManager.OnTick += OnTurnHappen;

        NewLevel();

        m_FoodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");
        m_FoodLabel.text = $"Food: {m_FoodAmount}";

        m_GameOverPanel = UIDoc.rootVisualElement.Q<VisualElement>("GameOverPanel");
        m_GameOverMessage = m_GameOverPanel.Q<Label>("GameOverMessage");

        m_GameOverPanel.style.visibility = Visibility.Hidden;
    }

    void OnTurnHappen()
    {
        ChangeFood(-1);
    }

    public void ChangeFood(int amount)
    {
        m_FoodAmount += amount;
        m_FoodLabel.text = $"Food: {m_FoodAmount}";

        if (m_FoodAmount <= 0)
        {
            PlayerController.GameOver();

            string levelString = m_CurrentLevel > 1 ? "levels" : "level";
            m_GameOverPanel.style.visibility = Visibility.Visible;
            m_GameOverMessage.text = $"Game Over!\n\nYou traveled through {m_CurrentLevel} {levelString}.\n\nPress Enter to restart.";
        }
    }

    public void NewLevel()
    {
        BoardManager.Clean();
        BoardManager.Init();

        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));

        m_CurrentLevel++;
    }

    public void StartNewGame()
    {
        m_GameOverPanel.style.visibility = Visibility.Hidden;

        m_CurrentLevel = 1;
        m_FoodAmount = 20;
        m_FoodLabel.text = "Food: " + m_FoodAmount;

        BoardManager.Clean();
        BoardManager.Init();

        PlayerController.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));
    }
}
