using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField]
    private AudioMixer m_AudioMixer;

    [SerializeField]
    private AudioSource m_MusicSource;

    [SerializeField]
    private AudioSource m_SFXSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);

            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void PlayMusic(AudioClip clip) { }

    public void StopMusic() { }

    public void PlaySFX(AudioClip clip, float volume = 1f) { }

    public void SetVolume(string parameter, float linear) { }
}
