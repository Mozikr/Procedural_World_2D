using UnityEngine;

public class NPC_Movement : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Rigidbody2D rb;
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        InvokeRepeating("MoveNPC", 0f, 2f);
    }

    private void MoveNPC()
    {
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-1f, 1f);
        Vector2 moveDirection = new Vector2(randomX, randomY).normalized;
        rb.velocity = moveDirection * moveSpeed;
        UpdateAnimation(moveDirection);
    }

    private void UpdateAnimation(Vector2 moveDirection)
    {
        animator.SetFloat("Horizontal", moveDirection.x);
        animator.SetFloat("Vertical", moveDirection.y);
        animator.SetFloat("Speed", moveDirection.sqrMagnitude);
    }
}
