using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private float damageAmount = 3.0f;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health hittedEnemyHealth = collision.GetComponentInParent<Health>();

            if (hittedEnemyHealth != null)
            {
                hittedEnemyHealth.OnTakeDamage_event(damageAmount);
                SoundManager.Instance.PlayEnemyHit();
            }
        }
    }
}
