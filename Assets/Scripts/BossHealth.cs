using UnityEngine;
using UnityEngine.SceneManagement;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 1000;  // Set here once
    [HideInInspector] public HealthBar healthBar;  // Controller sets this
    [HideInInspector] public int health;

    void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Boss Damaged: " + damage);
        health -= damage;
        if (healthBar != null) healthBar.SetHealth(health);

        if (health <= 0) Die();
    }

    public virtual void Die()  // Virtual for override
    {
        Debug.Log("Boss defeated!");
        Destroy(gameObject, 0.5f);
        SceneManager.LoadScene("EndScreen");
    }
}