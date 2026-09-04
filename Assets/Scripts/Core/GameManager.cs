using Board;
using Level;
using Player;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core
{
    public enum GameOverReason
    {
        Depleted,
        Defeated,
    }

    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private LevelProgressionSettings m_ProgressionSettings;

        public static GameManager Instance { get; private set; }

        public TurnManager TurnManager { get; private set; }
        public BoardManager BoardManager;
        public LevelManager LevelManager;
        public PlayerController PlayerController;
        public UIDocument HUDLayersDoc;
        public LevelProgressionSettings ProgressionSettings => m_ProgressionSettings;

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
}
