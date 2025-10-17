using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce    = 15f;
    public float gravityScale = 7f;

    Rigidbody2D rb;
    Animator    animator;
    AudioSource audioSource;
    bool        isGrounded = false;

    void Awake()
    {
        rb          = GetComponent<Rigidbody2D>();
        animator    = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // Apply custom gravity
        rb.gravityScale = gravityScale;

        // Freeze X & rotation so the Dino only moves up/down
        rb.constraints = RigidbodyConstraints2D.FreezePositionX 
                       | RigidbodyConstraints2D.FreezeRotation;
    }

    /// <summary>
    /// Called by GameManager (or via socket). 
    /// Will only fire if we're actually grounded.
    /// </summary>
    public void TryJump()
    {
        if (!isGrounded) return;   // ignore mid-air commands

        // Zero out any Y-velocity then jump
        rb.velocity  = new Vector2(rb.velocity.x, jumpForce);
        isGrounded   = false;      // prevent immediate re-jump

        // Trigger your jump animation
        if (animator != null)
            animator.SetTrigger("Jump");

        // Play SFX if one is attached
        if (audioSource != null)
            audioSource.Play();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // If we land on anything tagged "Ground", we can jump again
        if (col.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        // Obstacle hit → game over
        else if (col.collider.CompareTag("Obstacle") 
              && GameManager.Instance != null 
              && GameManager.Instance.IsPlaying())
        {
            GameManager.Instance.GameOver();
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        // Leaving the ground flag
        if (col.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}