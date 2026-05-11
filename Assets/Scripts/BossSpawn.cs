using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.AI;
using System.Collections.Generic;

// countdown timer and then the boss spawns after the timer is done
/// <summary>
/// DEPRECATED!!! MOVED TO BOSSFIGHT
/// </summary>
public class BossSpawn : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject bossPrefab;
    public Vector3 spawnPosition = Vector3.zero;
    public float spawnTimer = 30f; 

    private float timeRemaining;
    private bool bossSpawned = false;
    private bool bossActive = false;
    private GameObject bossInstance;

    public TMP_Text countdownText;

    //MUSIC
    public AudioSource musicSource;
    public AudioClip bossMusicTrack;

    void Start()
    {

        timeRemaining = spawnTimer;
        UpdateCountdownUI(timeRemaining);

        StartCoroutine(SpawnBossSequence());
    }

    void Update()
    {
        if (bossSpawned) return;

        if (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < 0f) timeRemaining = 0f;
            UpdateCountdownUI(timeRemaining);
        }
    }

    IEnumerator SpawnBossSequence()
    {
        yield return new WaitForSeconds(spawnTimer);
        SwitchToBossMusic();

        bossInstance = Instantiate(bossPrefab, spawnPosition, Quaternion.identity);
        bossSpawned = true;
        bossActive = true;

  
        if (countdownText != null)
            countdownText.text = "";

        Debug.Log("Boss has spawned!");
    }

    void UpdateCountdownUI(float timeToDisplay)
    {
        if (countdownText == null) return;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60f);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60f);
        countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void SwitchToBossMusic()
    {
        if (musicSource != null && bossMusicTrack != null)
        {
            musicSource.Stop();
            musicSource.clip = bossMusicTrack;
            musicSource.Play();
            Debug.Log("Boss music started!");
        }
        else
        {
            Debug.LogWarning("Boss music error");
        }
    }

}
