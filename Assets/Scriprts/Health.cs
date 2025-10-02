using UnityEngine;

public class Health : MonoBehaviour
{
    public event System.Action OnTakeDamage;
    public event System.Action OnDie;
    [SerializeField] private float maxHealth;
    private GameObject owner;
    private float currentHealth;
    private bool isDead = false;

    private void Start()
    {
        // Set owner
        owner = gameObject;

        // Set currentHealth equal to maxHealth
        currentHealth = maxHealth;
    }

    private void OnDestroy()
    {
        OnTakeDamage = null;
        OnDie = null;
    }


    public void OnTakeDamage_event(float damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        OnTakeDamage?.Invoke();

        if (currentHealth <= 0.0f)
        {
            OnDie?.Invoke();
        }
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}