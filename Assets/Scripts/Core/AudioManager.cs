using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Mixer and Sources")]
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

    public void PlayMusic(AudioClip track)
    {
        if (track == null)
            return;

        if (m_MusicSource.clip == track && m_MusicSource.isPlaying)
            return;

        m_MusicSource.clip = track;
        m_MusicSource.loop = true;

        m_MusicSource.Play();
    }

    public void StopMusic() => m_MusicSource.Stop();

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null)
            return;

        m_SFXSource.PlayOneShot(clip, volume);
    }

    public void PlayRandomSFXFromList(AudioClip[] clips, float volume = 1f)
    {
        if (clips == null || clips.Length == 0)
            return;

        int randomIndex = Random.Range(0, clips.Length);
        PlaySFX(clips[randomIndex], volume);
    }

    public void PlaySFXWithPitch(AudioClip clip, float pitch, float volume = 1f)
    {
        if (clip == null)
            return;

        m_SFXSource.pitch = pitch;
        m_SFXSource.PlayOneShot(clip, volume);
        m_SFXSource.pitch = 1f;
    }

    public void SetVolume(string parameter, float linear)
    {
        float dB = linear > 0.0001f ? Mathf.Log10(linear) * 20f : -80f;

        m_AudioMixer.SetFloat(parameter, dB);
    }
}
