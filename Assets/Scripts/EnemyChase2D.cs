using UnityEngine;

public class EnemyChase2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3f;

    private Transform playerTransform;
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void FixedUpdate()
    {
        if (playerTransform == null || rb == null) return;
        
        if (GameOverController.isAnswering || GameOverController.isGameOver)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            // Stop animation when frozen
            if (animator != null)
            {
                animator.SetFloat("speed", 0f);
                animator.SetBool("isMoving", false);
            }

            return;
        }

        // 1. Calculate direction to player
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        // 2. Move toward player
        rb.linearVelocity = direction * speed;


    

        // 4. Update Animator parameters
        if (animator != null)
        {
            animator.SetFloat("speed", speed);
            animator.SetBool("isMoving", true);

            // Set direction based on dominant axis
            // 0 = down, 1 = up, 2 = right, 3 = left
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                animator.SetInteger("direction", direction.x > 0 ? 2 : 3);
            }
            else
            {
                animator.SetInteger("direction", direction.y > 0 ? 1 : 0);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHP = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHP != null)
            {
                playerHP.TakeDamage(10);
            }
        }
    }
}