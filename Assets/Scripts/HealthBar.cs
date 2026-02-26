using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HealthBar : MonoBehaviour
{
    [Header("Radial Fill")]
    public Image fill;  // Assign the Image (child of a background circle sprite)

    [Header("Colors")]
    public Gradient gradient;

    private int maxHealth;

    public void SetMaxHealth(int health)
    {
        maxHealth = health;
        SetHealth(health);
    }

    public void SetHealth(int health)
    {
        float normalizedValue = (float)health / maxHealth;
        fill.fillAmount = normalizedValue;  // Drives radial fill (0=empty, 1=full)
        fill.color = gradient.Evaluate(normalizedValue);
    }

    // Billboard stuff
    public Transform cam;
    public bool isWorldSpace;

    void LateUpdate()
    {
        if (isWorldSpace)
        {
            transform.LookAt(transform.position + cam.forward);
        }
    }
}
