using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Boss Spawn / Timer")]
    public GameObject bossPrefab;
    public GameObject bossInstance;
    public Vector3 spawnPosition = Vector3.zero;
    public float spawnTimer = 30f;

    public TMP_Text countdownText;

    public AudioSource musicSource;
    public AudioClip bossMusicTrack;

    private float timeRemaining;
    private bool bossSpawned = false;
    private bool bossActive = false;


    [Header("Health")]
    public int maxHealth = 1000;
    public HealthBar healthBar;

    [Header("Phase 1: Tentacles")]
    public GameObject tentaclePrefab;
    public Transform[] tentacleSpawnPoints;
    public float tentacleSpawnDelay = 2f;
    public int tentacleCount = 4;

    [Header("Phase 2: Beam")]
    public GameObject sludgeProjectile;
    public float beamCooldown = 10f;
    public float weaknessWindow = 20f;

    [Header("Phase 3: Baby")]
    public GameObject babyPrefab;
    public float babyFleeSpeed = 8f;

    [Header("References")]
    public Transform player;

    private int health;
    private bool isWeakness = false;
    private bool tentaclesActive = true;
    private bool babySpawned = false;


    void Awake()
    {
        if (bossInstance != null)
        {
            bossInstance.SetActive(false);
        }
    }

    void Start()
    {
        // Reset health
        health = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(health);
        }

        // Start timer countdown
        timeRemaining = spawnTimer;
        UpdateCountdownUI(timeRemaining);

        StartCoroutine(SpawnBossSequence());
    }

    void Update()
    {
        //if (bossSpawned) return;

        if (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < 0f) timeRemaining = 0f;
            UpdateCountdownUI(timeRemaining);
        }

        if (healthBar != null) healthBar.SetHealth(health);

    }


    IEnumerator SpawnBossSequence()
    {
        yield return new WaitForSeconds(spawnTimer);
        SwitchToBossMusic();

        if (bossInstance != null)
        {
            bossInstance.transform.position = spawnPosition;
            bossInstance.SetActive(true);

            NavMeshAgent agent = bossInstance.GetComponent<NavMeshAgent>();
            if (agent) agent.enabled = true;
        }

        bossSpawned = true;
        bossActive = true;

        if (countdownText != null)
            countdownText.text = "";

        Debug.Log("Boss has spawned!");

        // Start boss behavior (grow + tentacles)
        yield return StartCoroutine(GrowBossFromGround());
        SpawnTentacles();
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


    IEnumerator GrowBossFromGround()
    {
        Debug.Log("Boss State: SPAWNING - Growing from ground");
        Transform bossTransform = bossInstance.transform;

        bossTransform.localScale = Vector3.zero;
        NavMeshAgent agent = bossInstance.GetComponent<NavMeshAgent>();
        if (agent) agent.enabled = false;

        float growTime = 7f;
        Vector3 targetScale = Vector3.one;
        for (float t = 0; t < growTime; t += Time.deltaTime)
        {
            bossTransform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t / growTime);
            yield return null;
        }
        bossTransform.localScale = targetScale;
        if (agent) agent.enabled = true;
    }

    void SpawnTentacles()
    {
        StartCoroutine(SpawnTentaclesRoutine());
        isWeakness = true;
    }

    IEnumerator SpawnTentaclesRoutine()
    {
        for (int i = 0; i < tentacleCount; i++)
        {
            if (tentacleSpawnPoints.Length > i)
            {
                GameObject tentacle = Instantiate(tentaclePrefab, tentacleSpawnPoints[i].position, Quaternion.identity);
                TentacleHealth tentHealth = tentacle.GetComponent<TentacleHealth>();
                if (tentHealth) tentHealth.boss = this; // Link to this boss
            }
            yield return new WaitForSeconds(tentacleSpawnDelay);
        }
    }


    public void BossTakeDamage(int damage)
    {
        // Only damage during weak / low‑health windows
        if (!isWeakness && health > maxHealth * 0.6f) return;

        Debug.Log("Boss Damaged: " + damage);
        health -= damage;
        if (healthBar != null) healthBar.SetHealth(health);

        if (health <= 0.2f * maxHealth && !babySpawned)
        {
            EnterBabyPhase();
        }
        else if (health <= 0.6f * maxHealth && tentaclesActive)
        {
            EnterBeamPhase();
        }
        else if (health <= 0)
        {
            BossDie();
        }
    }



    public void OnTentacleDeath()
    {
        health -= 10;
        if (healthBar) healthBar.SetHealth(health);
        Debug.Log("Boss State: Tentacle killed - Health reduced to " + health);
        if (health <= maxHealth * 0.6f) EnterBeamPhase();
    }

    void EnterBeamPhase()
    {
        tentaclesActive = false;
        Debug.Log("Boss State: 60% Health - Starting beam attacks");
        StartCoroutine(BeamAttackLoop());
    }

    IEnumerator BeamAttackLoop()
    {
        Transform bossTransform = bossInstance.transform;
        NavMeshAgent agent = bossInstance.GetComponent<NavMeshAgent>();
        Transform firePoint = bossInstance.transform.Find("FirePoint");
        if (firePoint == null)
        {
            Debug.LogWarning("BossController: FirePoint not found; using boss position as fallback.");
            firePoint = bossInstance.transform;
        }

        while (health > maxHealth * 0.2f)
        {
            yield return StartCoroutine(FireSludgeBeam(bossTransform, agent, firePoint));
            isWeakness = true;
            Debug.Log("Boss State: Beam cooldown - VULNERABLE (" + weaknessWindow + "s)");
            yield return new WaitForSeconds(weaknessWindow);
            isWeakness = false;
            Debug.Log("Boss State: INVINCIBLE");
            yield return new WaitForSeconds(beamCooldown);
        }
    }

    IEnumerator FireSludgeBeam(Transform bossTransform, NavMeshAgent agent, Transform firePoint)
    {
        if (agent != null) agent.SetDestination(bossTransform.position);
        bossTransform.LookAt(player);

        int beamCount = 7;
        float beamAngleSpread = 45f;
        for (int i = 0; i < beamCount; i++)
        {
            float angle = ((float)i / (beamCount - 1) - 0.5f) * beamAngleSpread;
            Quaternion rot = Quaternion.Euler(0, angle, 0) * bossTransform.rotation;
            Vector3 spawnPos = firePoint ? firePoint.position : bossTransform.position + bossTransform.up * 1.5f;
            GameObject proj = Instantiate(sludgeProjectile, spawnPos, rot);
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.AddForce(bossTransform.forward * 32f, ForceMode.Impulse);
                rb.AddForce(bossTransform.up * 8f, ForceMode.Impulse);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    void EnterBabyPhase()
    {
        babySpawned = true;
        Debug.Log("Boss State: 20% Health - Baby flee mode");
        GameObject baby = Instantiate(babyPrefab, bossInstance.transform.position, Quaternion.identity);
        TarBabyNav babyAI = baby.GetComponent<TarBabyNav>();
        if (babyAI) babyAI.Setup(player);
        if (bossInstance) Destroy(bossInstance);
    }

    void BossDie()
    {
        Debug.Log("Boss State: 0% - FINAL DEATH");
        if (bossInstance)
        {
            Animator anim = bossInstance.GetComponent<Animator>();
            if (anim) anim.SetTrigger("Die");
            Destroy(bossInstance, 0.5f);
        }

        // Or load scene, etc.
        // SceneManager.LoadScene("EndScreen");
    }
}