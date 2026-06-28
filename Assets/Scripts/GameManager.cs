using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private int m_FoodAmount = 100;
    private int m_CurrentLevel = 1;
    private Label m_FoodLabel;

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
    }

    void OnTurnHappen()
    {
        m_FoodAmount--;
        m_FoodLabel.text = $"Food: {m_FoodAmount}";
    }

    public void ChangeFood(int amount)
    {
        m_FoodAmount += amount;
        m_FoodLabel.text = $"Food: {m_FoodAmount}";
    }

    public void NewLevel()
    {
        BoardManager.Clean();
        BoardManager.Init();

        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));

        m_CurrentLevel++;
    }
}
