using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private int m_FoodAmount = 100;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_FoodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");
        m_FoodLabel.text = $"Food: {m_FoodAmount}";

        TurnManager = new TurnManager();
        TurnManager.OnTick += OnTurnHappen;
        
        BoardManager.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));

    }

    // Update is called once per frame
    void Update()
    {
        
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
}
