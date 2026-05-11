using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;
    public UnityEvent onTakeDamage, onDeath;

    [Header("UI")]
    public HealthBar healthBar;

    [Header("Respawn")]
    public Transform respawnPoint;

    [Header("Animation")]
    public Animator animator;
    private int _takeHitTriggerID;
    private int _dieTriggerID;

    [Header("Self Heal")]
    public Image healSprite;  
    public float healCooldown = 10f;  

    private float lastHealTime;
    private Color fullColor;
    private bool isHealingOnCooldown;


    private int _hitTriggerID;
    private bool _isDead;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        AssignAnimationIDs();

        if (healSprite != null)
        {
            fullColor = healSprite.color;
        }
    }

    private void AssignAnimationIDs()
    {
        if (animator == null) return;

        _takeHitTriggerID = Animator.StringToHash("TakeHit");
        _dieTriggerID = Animator.StringToHash("Die");
    }

    void Update()
    {
     //DEV HEALING GOD MODE!!!!
        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    SelfHeal();
        //}

        if (Input.GetKeyDown(KeyCode.H))
        {
            SelfHeal();
        }


        if (isHealingOnCooldown && Time.time >= lastHealTime + healCooldown && healSprite != null)
        {
            isHealingOnCooldown = false;
            healSprite.color = fullColor;  
            Debug.Log("Healing ready");
        }
    }

    public void TakePlayerDamage(int damage)
    {
        if (_isDead) return;

        Debug.Log("TakeDamage Player");
        currentHealth -= damage;
        onTakeDamage?.Invoke();

        // Play damage animation

        if (animator != null)
        {
            animator.ResetTrigger(_takeHitTriggerID);
            animator.SetTrigger(_takeHitTriggerID);

            Debug.Log("Playhit animation called");
        }
       

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            healthBar.SetHealth(currentHealth);
            Die();
            return;
        }

        healthBar.SetHealth(currentHealth);
    }

    void Die()
    {
        if (_isDead) return;
        _isDead = true;

        Debug.Log("Player died");
        onDeath?.Invoke();

        // Play death animation
        if (animator != null)
        {
            animator.ResetTrigger(_hitTriggerID);
            animator.ResetTrigger(_dieTriggerID);
            animator.SetTrigger(_dieTriggerID);
        }

    }

    public void OnDeathAnimationFinished()
    {
        if (!_isDead) return;

       
        if (respawnPoint != null)
            transform.position = respawnPoint.position;
        else
            transform.position = Vector3.zero;

        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);

        //if (animator != null)
        //{
        //    animator.ResetTrigger(_dieTriggerID);
        //    animator.Play("Idle Walk Run Blend", 0, 0f); 
        //}

        PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
        if (pauseMenu) pauseMenu.ShowDieScreen();

        //_isDead = false;

    }

    public void SelfHeal()
    {
        if (_isDead || Time.time < lastHealTime + healCooldown) return;

        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
        Debug.Log("Player healed to full health");

        lastHealTime = Time.time;
        isHealingOnCooldown = true;
        Color fadedColor = fullColor;
        fadedColor.a = 0.1f;
        healSprite.color = fadedColor;
    } 
}
