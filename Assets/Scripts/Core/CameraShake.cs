using Combat;
using Unity.Cinemachine;
using UnityEngine;

namespace Core
{
    public class CameraShake : MonoBehaviour
    {
        public static CameraShake Instance { get; private set; }

        [SerializeField]
        private float m_ShakeForcePerHp = 0.5f;

        private CinemachineImpulseSource m_ImpulseSource;

        void Awake()
        {
            if (Instance == null)
                Instance = this;

            m_ImpulseSource = GetComponent<CinemachineImpulseSource>();
        }

        void Start() =>
            GameManager.Instance.PlayerController.Combatant.Stats.Damaged += OnPlayerDamaged;

        void OnDestroy()
        {
            if (GameManager.Instance != null && GameManager.Instance.PlayerController != null)
                GameManager.Instance.PlayerController.Combatant.Stats.Damaged -= OnPlayerDamaged;
        }

        void OnPlayerDamaged(DamageResult result)
        {
            if (result.HPLost > 0)
                m_ImpulseSource.GenerateImpulseWithForce(result.HPLost * m_ShakeForcePerHp);
        }
    }
}
