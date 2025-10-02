using System.Collections;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [Header("Pointers")]
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private Health enemyHealth;
    [Header("Movement")]
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private int startDirection = 1;
    [SerializeField] private float waitTime = 2.0f;
    [Header("Attack")]
    [SerializeField] private float damageAmount = 10.0f;
    [SerializeField] private float attackRange = 0.1f;
    [SerializeField] private float attackCooldown = 0.7f;
    [SerializeField] private float attackDistance = 0.3f;
    private int currentDirection;
    private float halfWidth;
    private float halfHeight;
    private Vector2 movement;
    private bool isChangingDirection = false;
    private bool inGameLoop = true;


    private void Start()
    {
        // Bind event
        if (enemyHealth != null)
        {
            enemyHealth.OnDie += HandleDeath;
            enemyHealth.OnTakeDamage += PlayTakingDamage;
        }

        // Set defaults
        currentDirection = startDirection;
        halfWidth = spriteRenderer.bounds.extents.x;
        halfHeight = spriteRenderer.bounds.extents.y;
        FlipSpriteDirection();
    }

    private void FixedUpdate()
    {
        if (inGameLoop == true)
        {
            // set movement
            if (!isChangingDirection)
            {
                movement.x = speed * currentDirection;
                movement.y = rigidBody.linearVelocity.y;
                rigidBody.linearVelocity = movement;
            }

            if (DetectPlayer() == true)
            {
                inGameLoop = false;

                // Stop movement
                rigidBody.linearVelocity = new Vector2(0, 0);
                animator.SetBool("IsRunning", false);

                StartCoroutine(AttackPlayer());
            }

            // Set animation parameter
            SetAnimatorBool_Running();

            // Set direction of the enemy
            SetDirection();
        }
    }

    private IEnumerator ChangeDirection()
    {
        isChangingDirection = true;

        // Stop movement
        Vector2 originalVelocity = rigidBody.linearVelocity;
        rigidBody.linearVelocity = new Vector2(0, 0);
        animator.SetBool("IsRunning", false);

        // Wait
        yield return new WaitForSeconds(waitTime);

        // Switch direction
        currentDirection *= -1;
        FlipSpriteDirection();

        // Return velocity
        rigidBody.linearVelocity = originalVelocity;
        isChangingDirection = false;
    }

    private IEnumerator AttackPlayer()
    {
        animator.SetTrigger("Attack");
        DealDamage();

        // Wait
        yield return new WaitForSeconds(attackCooldown);

        if (DetectPlayer() == true) StartCoroutine(AttackPlayer());
        else if (DetectPlayer() == false) inGameLoop = true;
    }

    private void PlayTakingDamage()
    {
        animator.SetTrigger("TakeDamage");
    }

    private void SetDirection()
    {
        if (isChangingDirection) return;

        Vector2 rightPos = transform.position;
        Vector2 leftPos = transform.position;
        rightPos.x += halfWidth;
        leftPos.x -= halfWidth;

        if (currentDirection > 0)
        {
            bool hitWall = Physics2D.Raycast(transform.position, Vector2.right, halfWidth + 0.1f, LayerMask.GetMask("Ground"));
            bool noGroundAhead = !Physics2D.Raycast(rightPos, Vector2.down, halfHeight + 0.1f, LayerMask.GetMask("Ground"));

            if (hitWall || noGroundAhead)
            {
                StartCoroutine(ChangeDirection());
            }
        }
        else if (currentDirection < 0)
        {
            bool hitWall = Physics2D.Raycast(transform.position, Vector2.left, halfWidth + 0.1f, LayerMask.GetMask("Ground"));
            bool noGroundAhead = !Physics2D.Raycast(leftPos, Vector2.down, halfHeight + 0.1f, LayerMask.GetMask("Ground"));

            if (hitWall || noGroundAhead)
            {
                StartCoroutine(ChangeDirection());
            }
        }
    }

    private void FlipSpriteDirection()
    {
        if (currentDirection > 0)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }

    private void SetAnimatorBool_Running()
    {
        if (isChangingDirection) return;

        if (rigidBody.linearVelocityX != 0)
        {
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
    }

    private bool DetectPlayer()
    {
        int currentDirectionInst = currentDirection > 0 ? 1 : -1;
        return Physics2D.Raycast(transform.position, Vector2.right * currentDirectionInst,
        halfWidth + attackRange, LayerMask.GetMask("Player"));
    }

    private void DealDamage()
    {
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, Vector2.right * currentDirection, halfWidth + attackDistance, LayerMask.GetMask("Player"));

        if (hit2D.collider != null)
        {
            Health hittedEnemyHealth = hit2D.collider.GetComponentInParent<Health>();

            if (hittedEnemyHealth != null)
            {
                hittedEnemyHealth.OnTakeDamage_event(damageAmount);
            }
        }

    }

    private void HandleDeath()
    {
        StopAllCoroutines();
        if (enemyHealth != null) enemyHealth.OnDie -= HandleDeath;
        Destroy(gameObject);
    }
}
