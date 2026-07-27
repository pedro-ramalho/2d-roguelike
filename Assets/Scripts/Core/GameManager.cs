using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public enum GameOverReason { Depleted, Defeated }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public TurnManager TurnManager { get; private set; }
    public BoardManager BoardManager;
    public LevelManager LevelManager;
    public PlayerController PlayerController;
    public UIDocument UIDoc;

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
}
