using System;
using System.Collections;
using Board;
using Level;
using Player;
using Unity.Cinemachine;
using UnityEngine;

namespace Core
{
    public class LevelManager : MonoBehaviour
    {
        // Private references
        private BoardManager m_BoardManager;
        private PlayerController m_PlayerController;

        private LevelBand m_CurrentBand;

        // Serialized references
        [SerializeField]
        private LevelTransitionManager m_LevelTransitionManager;

        [SerializeField]
        private BoardGenerator m_BoardGenerator;

        [SerializeField]
        private BoardCameraConfiner m_CameraConfiner;

        // Events
        public event Action<GameOverReason, int> GameOverTriggered;
        public event Action GameStartTriggered;
        public event Action<int> LevelChanged;

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
            m_PlayerController = GameManager.Instance.PlayerController;

            m_PlayerController.Combatant.Stats.Depleted += OnPlayerDepleted;
            m_PlayerController.Combatant.Defeated += OnPlayerDefeated;

            NewLevel();
        }

        void OnDestroy()
        {
            if (m_PlayerController != null)
            {
                m_PlayerController.Combatant.Stats.Depleted -= OnPlayerDepleted;
                m_PlayerController.Combatant.Defeated -= OnPlayerDefeated;
            }
        }

        void OnPlayerDefeated() => TriggerGameOver(GameOverReason.Defeated);

        void OnPlayerDepleted() => TriggerGameOver(GameOverReason.Depleted);

        void TriggerGameOver(GameOverReason reason)
        {
            m_PlayerController.GameOver();

            AudioManager.Instance.StopMusic();

            GameOverTriggered?.Invoke(reason, m_CurrentLevel);
        }

        public bool IsEliteLevel() =>
            m_CurrentLevel % GameManager.Instance.ProgressionSettings.EliteCadence == 0;

        public void GoToLevel(int level)
        {
            m_CurrentLevel = level;
            LevelChanged?.Invoke(m_CurrentLevel);

            LevelBand previousBand = m_CurrentBand;
            LevelBand newBand = ResolveBand(m_CurrentLevel);

            bool crossedBand = previousBand != newBand;

            if (crossedBand)
            {
                m_PlayerController.Combatant.Stats.RefreshStats();
                StartCoroutine(
                    m_LevelTransitionManager.PlayBandTransitionCoroutine(newBand, RebuildLevel)
                );
            }
            else
                RebuildLevel();
        }

        void RebuildLevel()
        {
            m_BoardManager.ClearCell(m_PlayerController.Combatant.Cell);
            m_PlayerController.gameObject.SetActive(false);

            m_BoardManager.Clean();
            m_BoardManager.Init(m_BoardWidth, m_BoardHeight);
            m_CameraConfiner.FitToBoard(m_BoardWidth, m_BoardHeight);

            m_BoardGenerator.GenerateBoard(m_BoardManager);

            m_PlayerController.gameObject.SetActive(true);
            m_PlayerController.Spawn(m_PlayerSpawnCell);
        }

        IEnumerator BandTransitionCoroutine(LevelBand band)
        {
            m_PlayerController.Combatant.Stats.RefreshStats();

            StartCoroutine(AudioManager.Instance.FadeOutMusicCoroutine(0.5f));

            yield return m_LevelTransitionManager.FadeOutCoroutine(band);

            RebuildLevel();

            yield return new WaitForSeconds(5f);

            StartCoroutine(AudioManager.Instance.FadeInMusicCoroutine(band.Track, 10f));
            yield return m_LevelTransitionManager.FadeInCoroutine();
        }

        public void NewLevel()
        {
            int amount = Mathf.RoundToInt(m_PlayerController.Combatant.Stats.MaxStamina * 0.25f);
            m_PlayerController.Combatant.Stats.ChangeStamina(amount);
            GoToLevel(m_CurrentLevel + 1);
        }

        public void ReloadCurrentLevel() => GoToLevel(m_CurrentLevel);

        public void StartNewGame()
        {
            GameStartTriggered?.Invoke();

            m_CurrentBand = null;
            m_CurrentLevel = 0;

            m_PlayerController.Combatant.Stats.Reset();
            m_PlayerController.Init();
            m_PlayerController.SetVisible(true);

            GoToLevel(1);
        }

        LevelBand ResolveBand(int level)
        {
            foreach (LevelBand band in GameManager.Instance.ProgressionSettings.Bands)
            {
                if (level >= band.MinLevel && level <= band.MaxLevel)
                {
                    m_BoardWidth = band.BoardWidth;
                    m_BoardHeight = band.BoardHeight;
                    m_CurrentBand = band;

                    return band;
                }
            }

            UnityEngine.Debug.LogWarning($"No band matched level {level}, reusing last band");

            return m_CurrentBand;
        }
    }
}
