using UnityEngine;

public class GameManager : MonoBehaviour
{
    private TurnManager m_TurnManager;

    public BoardManager BoardManager;
    public PlayerController PlayerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_TurnManager = new TurnManager();
        BoardManager.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
