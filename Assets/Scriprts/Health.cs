using UnityEngine;

public class Health : MonoBehaviour
{
    public event System.Action<float> OnTakeDamage;
    public event System.Action OnDie;
    
    [SerializeField] private float maxHealth = 100.0f;
    
    private float currentHealth;
    private bool isDead = false;

    private void Awake()
    {
        // Initialize health
        maxHealth = Mathf.Max(1.0f, maxHealth);
        currentHealth = maxHealth;
    }
    
    public float GetCurrentHealth => currentHealth;
    public float GetMaxHealth => maxHealth;
    public bool IsDead => isDead;

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);
        OnTakeDamage?.Invoke(damageAmount);

        if (currentHealth <= 0.0f && !isDead)
        {
            isDead = true;
            OnDie?.Invoke();
        }
    }
    
    public void Heal(float healAmount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
    }
}