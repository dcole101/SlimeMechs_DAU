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
    public CapsuleCollider bossHitCollider;
    public float spawnTimer = 30f;

    public TMP_Text countdownText; 
    private GameObject countdownTextObj;


    //public AudioSource musicSource;
    //public AudioClip bossMusicTrack;
    public MusicManager music;

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
    public float tentacleSpawnDelay = 0.5f;
    public int tentacleCount = 4;
    public List<GameObject> activeTentacles = new List<GameObject>();

    //SWIPE
    public SimpleAttackPlayer swipeAttack;
    public float swipeCooldown = 4f;
    public float swipeRadius = 3f;
    public LayerMask whatIsPlayer;
    private bool isPhase1 = true;

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

    [SerializeField] private Material baseHeadMaterial;
    [SerializeField] private Material bodyBaseMaterial;

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
        countdownTextObj = GameObject.Find("Bosscountdown");

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

        if(vulnerable)
        {
            if (baseHeadMaterial != null) baseHeadMaterial.SetColor("_RimColor", new Color(0.416f, 0.247f, 0.886f, 1f));  // Purple
            if (bodyBaseMaterial != null) bodyBaseMaterial.SetColor("_RimColor", new Color(0.416f, 0.247f, 0.886f, 1f));  // Purple
        }
        else
        {

            if (baseHeadMaterial != null) baseHeadMaterial.SetColor("_RimColor", new Color(0.784f, 0.063f, 0.765f, 1f));  // Red
            if (bodyBaseMaterial != null) bodyBaseMaterial.SetColor("_RimColor", new Color(0.784f, 0.063f, 0.765f, 1f));  // Red
        }
        //SWIPE
        if (isPhase1 && tentaclesActive && vulnerable && swipeAttack != null)
        {
            CheckSwipeAttack();
        }

    }

  

    ///////// BOSS SPAWNING IN ////////////////////////////////////////////////////////

    IEnumerator SpawnBossSequence()
    {
        yield return new WaitForSeconds(spawnTimer);
        // SwitchToBossMusic();
        music.SwitchToBossMusic();

        if (bossInstance != null)
        {
            bossInstance.transform.position = spawnPosition;
            bossInstance.SetActive(true);

            //NavMeshAgent agent = bossInstance.GetComponent<NavMeshAgent>();
            //if (agent) agent.enabled = true;
        }

        bossSpawned = true;
        bossActive = true;

        if (countdownText != null)
        {
            countdownText.text = "";
            countdownTextObj.SetActive(false);
        }

        Debug.Log("Boss has spawned!");
        SpawnTentacles();
        // Start boss behavior (grow + tentacles)
        yield return StartCoroutine(GrowBossFromGround());

        
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
        //if (musicSource != null && bossMusicTrack != null)
        //{
        //    musicSource.Stop();
        //    musicSource.clip = bossMusicTrack;
        //    musicSource.Play();
        //    Debug.Log("Boss music started!");
        //}
        //else
        //{
        //    Debug.LogWarning("Boss music error");
        //}
        music.SwitchToBossMusic();

    }


    IEnumerator GrowBossFromGround()
    {
       
        vulnerable = false;
        Debug.Log("Boss State: SPAWNING - Growing from ground");
        Transform bossTransform = bossInstance.transform;

        bossTransform.localScale = Vector3.zero;


        float growTime = 7f;
        Vector3 targetScale = new Vector3(1.5f, 1.5f, 1.5f);
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
                float growTime = 2f;
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

    ///SWIPE ATTACKS
     // Phase 1 Swipe Attack Logic
    private float lastSwipeTime = -10f;
    private void CheckSwipeAttack()
    {
        if (Time.time - lastSwipeTime >= swipeCooldown)
        {
            // Check if player is within swipe radius
            Collider[] hits = Physics.OverlapSphere(bossInstance.transform.position, swipeRadius, whatIsPlayer);
            bool playerInRange = false;

            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player")) 
                {
                    playerInRange = true;
                    break;
                }
            }

            if (playerInRange)
            {
                PerformSwipeAttack();
            }
        }
    }

    private void PerformSwipeAttack()
    {
        lastSwipeTime = Time.time;
        vulnerable = false; 

        bodyanimator.SetTrigger("Swipe"); 
        leftarmanimator.SetTrigger("Swipe");
        rightarmanimator.SetTrigger("Swipe");

        // Damage delivered through animation event
        StartCoroutine(SwipeAttackRecovery());

        Debug.Log("Boss performs swipe attack!");
    }


    IEnumerator SwipeAttackRecovery()
    {
        yield return new WaitForSeconds(1.5f); 

        // Reset triggers
        bodyanimator.ResetTrigger("Swipe");
        leftarmanimator.ResetTrigger("Swipe");
        rightarmanimator.ResetTrigger("Swipe");

        vulnerable = true;
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

        if (health <= 0.2f * maxHealth && !babySpawned)
        {
            StartCoroutine(EnterBabyPhase());
            isPhase1 = false;
        }
        else if (health <= 0.6f * maxHealth && tentaclesActive)
        {
            StartCoroutine(EnterBeamPhase());
            StartCoroutine(ShrinkAllTentacles());
            isPhase1 = false;
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

        yield return new WaitForSeconds(3f);
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

            yield return new WaitForSeconds(7f);
            vulnerable = true;

        }
    }

    ///////// PHASE 3 - BABY MODE ////////////////////////////////////////////////////////

    IEnumerator EnterBabyPhase()
    {
        vulnerable = false;
        bodyanimator.SetTrigger("Endphase2");
        leftarmanimator.SetTrigger("Endphase2");
        rightarmanimator.SetTrigger("Endphase2");

        yield return new WaitForSeconds(6f);

        music.SwitchToBabyPhaseMusic();

        //moveboss away - hacky but works
        Transform bossTransform = bossInstance.transform;
        Vector3 currentPos = bossTransform.position;
        bossTransform.position = new Vector3(currentPos.x, -200f, currentPos.z);
        bossHitCollider.enabled = false;

        smokebomb.SetActive(true);
        yield return new WaitForSeconds(1f);
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


    void BossDie()
    {
        Debug.Log("Boss State: 0% - FINAL DEATH");
        if (bossInstance)
        {
            babyanimator = babyPrefab.GetComponent<Animator>();
            babyanimator.SetTrigger("Die");

            TarBabyNav babyAI = babyPrefab.GetComponent<TarBabyNav>();
            if (babyAI) babyAI.Die();

            Destroy(bossInstance, 0.5f);
        }

        StartCoroutine(ShowVictoryAfterDelay(5f));
    }

    IEnumerator ShowVictoryAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
        if (pauseMenu) pauseMenu.ShowVictoryScreen();
        music.SwitchToVictoryMusic();
    }
}