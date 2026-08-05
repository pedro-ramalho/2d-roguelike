using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip m_Track;

    void Start() => AudioManager.Instance.PlayMusic(m_Track);
}
