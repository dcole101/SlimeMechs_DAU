using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
  
    [SerializeField] private AudioSource musicSourceA;
    [SerializeField] private AudioSource musicSourceB;

    [SerializeField] private AudioClip levelMusicTrack;
    [SerializeField] private AudioClip bossMusicTrack;
    [SerializeField] private AudioClip babyPhaseMusicTrack;
    [SerializeField] private AudioClip victoryMusicTrack;

    [SerializeField] private float fadeDuration = 1.5f;

    private AudioSource _activeSource;
    private Coroutine _fadeCoroutine;

    private void Awake()
    {
        _activeSource = musicSourceA;
    }

    public void SwitchToLevelMusic() => CrossfadeTo(levelMusicTrack);
    public void SwitchToBossMusic() => CrossfadeTo(bossMusicTrack);
    public void SwitchToBabyPhaseMusic() => CrossfadeTo(babyPhaseMusicTrack);
    public void SwitchToVictoryMusic() => CrossfadeTo(victoryMusicTrack);

    public void CrossfadeTo(AudioClip newClip)
    {
        if (newClip == null)
        {
            Debug.LogWarning("Attempted to crossfade to null clip");
            return;
        }

        if (_activeSource.clip == newClip && _activeSource.isPlaying)
            return;

        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        _fadeCoroutine = StartCoroutine(CrossfadeCoroutine(newClip));
    }

    private IEnumerator CrossfadeCoroutine(AudioClip newClip)
    {
        AudioSource fadeOutSource = _activeSource;
        AudioSource fadeInSource = (_activeSource == musicSourceA) ? musicSourceB : musicSourceA;

        fadeInSource.clip = newClip;
        fadeInSource.volume = 0f;
        fadeInSource.Play();

        float startVolume = fadeOutSource.volume;
        float elapsed = 0f;

        // Crossfade
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            fadeOutSource.volume = Mathf.Lerp(startVolume, 0f, t);
            fadeInSource.volume = Mathf.Lerp(0f, 0.5f, t);

            yield return null;
        }

        fadeOutSource.Stop();
        fadeOutSource.volume = 0.5f;
        fadeInSource.volume = 0.5f;

        _activeSource = fadeInSource;
        _fadeCoroutine = null;
    }
}
