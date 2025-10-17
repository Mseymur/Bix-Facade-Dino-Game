using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class IdleJump : MonoBehaviour
{
    [Tooltip("Upward force applied each idle jump.")]
    public float jumpForce = 7f;
    [Tooltip("Seconds between idle jumps.")]
    public float interval = 2f;

    Rigidbody2D rb;
    Coroutine idleRoutine;

    void Awake()
    {
        // Try to grab the Rigidbody immediately
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Call this to start the idle‐jump loop.
    /// </summary>
    public void StartIdle()
    {
        if (idleRoutine == null && enabled)
        {
            idleRoutine = StartCoroutine(JumpLoop());
        }
    }

    /// <summary>
    /// Call this to stop the idle‐jump loop.
    /// </summary>
    public void StopIdle()
    {
        if (idleRoutine != null)
        {
            StopCoroutine(idleRoutine);
            idleRoutine = null;
        }
    }

    IEnumerator JumpLoop()
    {
        // Initial delay
        yield return new WaitForSeconds(1f);

        while (enabled)
        {
            // Ensure rb is valid
            if (rb == null)
                rb = GetComponent<Rigidbody2D>();

            // If we have a Rigidbody and we're on (or very near) the ground, jump
            if (rb != null && Mathf.Abs(rb.velocity.y) < 0.01f)
            {
                rb.velocity = Vector2.up * jumpForce;
            }

            yield return new WaitForSeconds(interval);
        }

        // Clean up reference when we exit
        idleRoutine = null;
    }

    void OnDisable()
    {
        // If the component is disabled, stop the coroutine too
        StopIdle();
    }
}