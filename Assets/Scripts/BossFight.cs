using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;
using System.Collections;

using System.Collections.Generic;


public class BossController : MonoBehaviour
{
    [Header("Boss Spawn / Timer")]
    public GameObject bossPrefab;
    public GameObject bossInstance;

    public Vector3 spawnPosition = Vector3.zero;
    public float spawnTimer = 30f;

    public TMP_Text countdownText;

    //Music to be implemented later
    //public AudioSource musicSource;
    //public AudioClip bossMusicTrack;

    private float timeRemaining;
    private bool bossSpawned = false;
    private bool bossActive = false;

    [SerializeField] public GameObject BodyAnimator;
    private Animator bodyanimator;

    [SerializeField] public GameObject LeftArmAnimator;
    private Animator leftarmanimator;

    [SerializeField] public GameObject RightArmAnimator;
    private Animator rightarmanimator;



    [Header("Health")]
    public int maxHealth = 1000;
    public HealthBar healthBar;

    [Header("Phase 1: Tentacles")]
    public GameObject tentaclePrefab;
    public Transform[] tentacleSpawnPoints;
    public float tentacleSpawnDelay = 2f;
    public int tentacleCount = 4;
    public List<GameObject> activeTentacles = new List<GameObject>();


    [Header("Phase 2: Beam")]
    public GameObject sludgeProjectile;
    public float beamCooldown = 10f;
    public float weaknessWindow = 20f;

    [Header("Phase 3: Baby")]
    public GameObject babyPrefab;
    public Animator babyanimator;
   
    public GameObject smokebomb;

    [Header("References")]
    public Transform player;

    private int health;
    private bool vulnerable = false;
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

        //assign animators
        bodyanimator = BodyAnimator.GetComponent<Animator>();
        leftarmanimator = LeftArmAnimator.GetComponent<Animator>();
        rightarmanimator = RightArmAnimator.GetComponent<Animator>();
       

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

  

    ///////// BOSS SPAWNING IN ////////////////////////////////////////////////////////

    IEnumerator SpawnBossSequence()
    {
        yield return new WaitForSeconds(spawnTimer);
        //SwitchToBossMusic();

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

    //void SwitchToBossMusic()
    //{
    //    if (musicSource != null && bossMusicTrack != null)
    //    {
    //        musicSource.Stop();
    //        musicSource.clip = bossMusicTrack;
    //        musicSource.Play();
    //        Debug.Log("Boss music started!");
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Boss music error");
    //    }
    //}


    IEnumerator GrowBossFromGround()
    {
        vulnerable = false;
        Debug.Log("Boss State: SPAWNING - Growing from ground");
        Transform bossTransform = bossInstance.transform;

        bossTransform.localScale = Vector3.zero;
        

        float growTime = 7f;
        Vector3 targetScale = Vector3.one;
        for (float t = 0; t < growTime; t += Time.deltaTime)
        {
            bossTransform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t / growTime);
            yield return null;
        }
        bossTransform.localScale = targetScale;
        vulnerable = true;
    }

    void SpawnTentacles()
    {
        StartCoroutine(SpawnTentaclesRoutine());
        vulnerable = true;
       
    }

    IEnumerator SpawnTentaclesRoutine()
    {
        for (int i = 0; i < tentacleCount; i++)
        {
            if (tentacleSpawnPoints.Length > i)
            {
                GameObject tentacle = Instantiate(tentaclePrefab, tentacleSpawnPoints[i].position, Quaternion.identity);
                TentacleHealth tentHealth = tentacle.GetComponent<TentacleHealth>();
                if (tentHealth) tentHealth.boss = this;
                activeTentacles.Add(tentacle);

                Transform tentacleTransform = tentacle.transform;
                tentacleTransform.localScale = Vector3.zero;
                float growTime = 3f;
                Vector3 targetScale = Vector3.one;
                for (float t = 0; t < growTime; t += Time.deltaTime)
                {
                    tentacleTransform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t / growTime);
                    yield return null;
                }
                tentacleTransform.localScale = targetScale;
            }
            yield return new WaitForSeconds(tentacleSpawnDelay);
        }
    }

    IEnumerator ShrinkTentacle(GameObject tentacle)
    {
        Transform tentacleTransform = tentacle.transform;
        float shrinkTime = 2f;
        Vector3 startScale = tentacleTransform.localScale;

        for (float t = 0; t < shrinkTime; t += Time.deltaTime)
        {
            tentacleTransform.localScale = Vector3.Lerp(startScale, Vector3.zero, t / shrinkTime);
            yield return null;
        }

        tentacleTransform.localScale = Vector3.zero;
        activeTentacles.Remove(tentacle);
        Destroy(tentacle);
    }

    ///////// Taking Damage  ////////////////////////////////////////////////////////
    public void BossTakeDamage(int damage)
    {
      
        if(vulnerable)
        {
            Debug.Log("Boss Damaged: " + damage);
            health -= damage;

            //animation
            bodyanimator.SetTrigger("TakeHit");
            leftarmanimator.SetTrigger("TakeHit");
            rightarmanimator.SetTrigger("TakeHit");

        }
       

        if (healthBar != null) healthBar.SetHealth(health);

        if (health <= 0.3f * maxHealth && !babySpawned)
        {
            StartCoroutine(EnterBabyPhase());
        }
        else if (health <= 0.6f * maxHealth && tentaclesActive)
        {
            StartCoroutine(EnterBeamPhase());
            StartCoroutine(ShrinkAllTentacles());

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
        if (health <= maxHealth * 0.6f) StartCoroutine(EnterBeamPhase());

        StartCoroutine(ShrinkAllTentacles());

        bodyanimator.SetTrigger("TakeHit");
        leftarmanimator.SetTrigger("TakeHit");
        rightarmanimator.SetTrigger("TakeHit");
    }

    IEnumerator ShrinkAllTentacles()
    {
        foreach (GameObject tentacle in activeTentacles.ToArray())
        {
            StartCoroutine(ShrinkTentacle(tentacle));
            yield return new WaitForSeconds(0.2f);
        }
    }

    ///////// PHASE 2 - SLUDGE BEAM ////////////////////////////////////////////////////////

    IEnumerator EnterBeamPhase()
    {
        vulnerable = false;

        bodyanimator.SetTrigger("Endphase1");
        leftarmanimator.SetTrigger("Endphase1");
        rightarmanimator.SetTrigger("Endphase1");

        bodyanimator.ResetTrigger("TakeHit");
        leftarmanimator.ResetTrigger("TakeHit");
        rightarmanimator.ResetTrigger("TakeHit");

        yield return new WaitForSeconds(3f);
        tentaclesActive = false;
        Debug.Log("Boss State: 60% Health - Starting beam attacks");
        StartCoroutine(BeamAttackLoop());

        bodyanimator.ResetTrigger("Endphase1");
        leftarmanimator.ResetTrigger("Endphase1");
        rightarmanimator.ResetTrigger("Endphase1");

        vulnerable = true;
    }

    IEnumerator BeamAttackLoop()
    {
        while (health > maxHealth * 0.2f)
        {
            
            yield return new WaitForSeconds(beamCooldown);

            vulnerable = false;
            bodyanimator.SetTrigger("Beam");
            leftarmanimator.SetTrigger("Beam");
            rightarmanimator.SetTrigger("Beam");

            bodyanimator.ResetTrigger("TakeHit");
            leftarmanimator.ResetTrigger("TakeHit");
            rightarmanimator.ResetTrigger("TakeHit");

            yield return new WaitForSeconds(5f);
            vulnerable = true;

        }
    }

    ///////// PHASE 3 - BABY MODE ////////////////////////////////////////////////////////

    IEnumerator EnterBabyPhase()
    {
        bodyanimator.SetTrigger("Endphase2");
        leftarmanimator.SetTrigger("Endphase2");
        rightarmanimator.SetTrigger("Endphase2");

        yield return new WaitForSeconds(6f);

        vulnerable = false;

        //moveboss away - hacky but works
        Transform bossTransform = bossInstance.transform;
        Vector3 currentPos = bossTransform.position;
        bossTransform.position = new Vector3(currentPos.x, -200f, currentPos.z);

        smokebomb.SetActive(true);
        yield return new WaitForSeconds(4f);
        vulnerable = true;

        babySpawned = true;
        Debug.Log("Boss State: 20% Health - Baby flee mode");
        
        babyPrefab.SetActive(true);
        //babyPrefab.transform.position = spawnPosition;

        TarBabyNav babyAI = babyPrefab.GetComponent<TarBabyNav>();
        if (babyAI) babyAI.Setup(player);

        smokebomb.SetActive(false);
        babyanimator = babyPrefab.GetComponent<Animator>();
        babyanimator.SetTrigger("Flip");

    }

    // Not using yet will be used eventually

    void BossDie()
    {
        Debug.Log("Boss State: 0% - FINAL DEATH");
        if (bossInstance)
        {
           babyanimator = babyPrefab.GetComponent<Animator>();
            babyanimator.SetTrigger("Die");

            TarBabyNav babyAI = babyPrefab.GetComponent<TarBabyNav>();
            if (babyAI) babyAI.Die();
            //healthBar.SetActive(false);

            Destroy(bossInstance, 0.5f);
        }

    }
}