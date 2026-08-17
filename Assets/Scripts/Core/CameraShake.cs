using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private CinemachineImpulseSource m_ImpulseSource;

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        m_ImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Start() => GameManager.Instance.PlayerController.Combatant.Damaged += OnPlayerDamaged;

    void OnPlayerDamaged(DamageResult result)
    {
        if (result.HPLost > 0)
            m_ImpulseSource.GenerateImpulseWithForce(result.HPLost * 0.5f);
    }
}
