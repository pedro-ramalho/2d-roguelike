using System.Collections;
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

    public IEnumerator FadeOutMusicCoroutine(float duration)
    {
        float startVolume = m_MusicSource.volume;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            m_MusicSource.volume = Mathf.Lerp(startVolume, 0, elapsed / duration);
            yield return null;
        }

        m_MusicSource.Stop();
        m_MusicSource.volume = startVolume; // restore for next Play
    }

    public IEnumerator FadeInMusicCoroutine(AudioClip track, float duration)
    {
        if (track == null)
            yield break;

        m_MusicSource.clip = track;
        m_MusicSource.loop = true;
        m_MusicSource.volume = 0f;
        m_MusicSource.Play();

        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            m_MusicSource.volume = Mathf.Lerp(0, 1, elapsed / duration);
            yield return null;
        }

        m_MusicSource.volume = 1f;
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
