using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [SerializeField]
    private int maxHealth;
    [SerializeField]
    private int currentHealth;
    public int CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }

    
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void takeDamage(int damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage;
        }

        if(currentHealth <= 0)
        {
            currentHealth = 0;
        }
    }

    public void recover(int HP)
    {
        if (currentHealth < maxHealth && currentHealth > 0)
        {
            currentHealth += HP;
        }

        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public bool isHealthZero()
    {
        if(currentHealth <= 0)
        {
            return true;
        }
        return false;
    }
}
