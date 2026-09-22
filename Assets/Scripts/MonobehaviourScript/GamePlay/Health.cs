using UnityEngine;

public class Health : MonoBehaviour
{
    
    [SerializeField]private float maxHealth = 1;
    [SerializeField]private float currentHealth;
    private bool isDead = false;
    public bool TakeDamage(float damage)
    {
        if(isDead)
        {
            Debug.LogError("Health:current health less than 0",this);
            return false;
        }
        else
        {
            currentHealth-=damage;
            if (currentHealth <= 0)
                Die();
            Debug.Log($"Health:current health is {currentHealth}.",this);
            return true;
        }

    }
    public void OnDie()
    {
        Debug.Log($"Health:{this.name} is dead",this);
    }
    public void Die()
    {
        isDead = true;
        OnDie();
    }


    void Awake()
    {
        currentHealth = maxHealth;
    }

}
