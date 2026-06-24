using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TurnManager TurnManager { get; private set; }
    public BoardManager BoardManager;
    public PlayerController PlayerController;

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
        TurnManager = new TurnManager();
        
        BoardManager.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
