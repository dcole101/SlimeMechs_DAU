using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public UnityEvent onTakeDamage, onDeath;

    public HealthBar healthBar;
    public Transform respawnPoint;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        // Press H to heal to full health
        if (Input.GetKeyDown(KeyCode.H))
        {
            SelfHeal();
        }
    }

    public void TakePlayerDamage(int damage)
    {
        Debug.Log("TakeDamage Player");
        currentHealth -= damage;
        onTakeDamage?.Invoke();

        if (currentHealth <= 0)
            Die();

        healthBar.SetHealth(currentHealth);
    }

    void Die()
    {
        Debug.Log("Player died");
        //onDeath?.Invoke();

        // Respawn at origin or respawnPoint
        if (respawnPoint != null)
            transform.position = respawnPoint.position;
        else
            transform.position = Vector3.zero;

        // Reset health to full
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
    }

    public void SelfHeal()
    {
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
        Debug.Log("Player healed to full health with H key");
    }
}
