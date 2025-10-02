using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Pointers")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private Health healthComponent;
    [SerializeField] private RespawnSystem respSys;
    [Header("Movement")]
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float jumpForce = 6.0f;
    [Header("Attack")]
    [SerializeField] private float damageAmount = 10.0f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float attackDistance = 0.3f;
    private Vector2 direction;
    private Vector2 screenBounds;
    private float playerHalfWidth = 0.0f;
    private float nextAttackTime = 0.0f;


    void OnEnable()
    {
        if (TryGetComponent<PlayerInput>(out var playerInput))
        {
            playerInput.actions["Jump"].performed += Jump;
            playerInput.actions["Attack"].performed += Attack;
        }
    }

    void OnDisable()
    {
        if (TryGetComponent<PlayerInput>(out var playerInput))
        {
            playerInput.actions["Jump"].performed -= Jump;
            playerInput.actions["Attack"].performed -= Attack;
        }
    }

    void Start()
    {
        // Bind events
        if (healthComponent != null)
        {
            healthComponent.OnDie += HandleDeath;
            healthComponent.OnTakeDamage += PlayTakingDamage;
        }

        // Calculate screen (viewport) bounds
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        // Get player sprite bounds
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            playerHalfWidth = spriteRenderer.bounds.extents.x;
        }
    }

    void Update()
    {
        // Call movement functions
        MovePlayer();
        CalculateScreenBounds();
        SetAnimatorValues_Running();
        RotatePlayerToDirection();

        SetAnimatorValues_Jumping(GroundRaycast());
    }

    private void MovePlayer()
    {
        // Get input direction (x or y) 
        float input = Input.GetAxisRaw("Horizontal");
        direction.x = input * speed * Time.deltaTime;

        // Apply direction to player
        transform.Translate(direction);
    }

    private void CalculateScreenBounds()
    {
        // Get current position and clamp it with screen bound X scalar
        Vector2 pos = transform.position;
        float clampedX = Mathf.Clamp(transform.position.x, -screenBounds.x + playerHalfWidth, screenBounds.x - playerHalfWidth);

        // Set pos with clamped vector
        pos.x = clampedX;
        transform.position = pos;
    }

    private void RotatePlayerToDirection()
    {
        // Rotate player to direction
        if (direction.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (direction.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    private bool GroundRaycast()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, 0.7f, LayerMask.GetMask("Ground"));
    }

    private void Jump(InputAction.CallbackContext ctx)
    {
        if (GroundRaycast() == true)
        {
            rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void SetAnimatorValues_Running()
    {
        // Set var for Animator
        if (direction.x != 0.0f)
        {
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
    }

    private void SetAnimatorValues_Jumping(bool isGrounded)
    {
        // Set var for Animator
        if (isGrounded == false)
        {
            animator.SetBool("IsGrounded", false);
        }
        else
        {
            animator.SetBool("IsGrounded", true);
        }
    }

    private void Attack(InputAction.CallbackContext ctx)
    {
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            animator.SetTrigger("Attack");
            DealDamage();
        }
    }

    private void PlayTakingDamage()
    {
        animator.SetTrigger("TakeDamage");
        SoundManager.Instance.PlayPlayerHit();
    }

    private void DealDamage()
    {
        int currentDirection = spriteRenderer.flipX == true ? -1 : 1;
        float halfWidth = spriteRenderer.bounds.extents.x;

        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, Vector2.right * currentDirection, halfWidth + attackDistance, LayerMask.GetMask("Enemy"));

        if (hit2D.collider != null)
        {
            Health hittedEnemyHealth = hit2D.collider.GetComponentInParent<Health>();

            if (hittedEnemyHealth != null)
            {
                hittedEnemyHealth.OnTakeDamage_event(damageAmount);
                SoundManager.Instance.PlayEnemyHit();
            }
        }
        else
        {
            SoundManager.Instance.PlayAirHit();
        }

    }

    private void HandleDeath()
    {
        respSys.Respawn(gameObject);
        if (healthComponent != null) healthComponent.OnDie -= HandleDeath;
        StopAllCoroutines();
    }
}

