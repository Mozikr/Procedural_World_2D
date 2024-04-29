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
        // Wywo�ujemy funkcj� MoveNPC co 2 sekundy (mo�esz dostosowa� interwa�)
        InvokeRepeating("MoveNPC", 0f, 2f);
    }

    private void MoveNPC()
    {
        // Generujemy losow� warto�� dla osi x i y
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-1f, 1f);

        // Tworzymy wektor kierunku ruchu na podstawie wygenerowanych warto�ci
        Vector2 moveDirection = new Vector2(randomX, randomY).normalized;

        // Ustawiamy pr�dko�� ruchu NPC
        rb.velocity = moveDirection * moveSpeed;

        // Ustawiamy animacj� w zale�no�ci od kierunku ruchu
        UpdateAnimation(moveDirection);
    }

    private void UpdateAnimation(Vector2 moveDirection)
    {
        // Ustawiamy parametry animacji w animatorze na podstawie kierunku ruchu
        animator.SetFloat("Horizontal", moveDirection.x);
        animator.SetFloat("Vertical", moveDirection.y);
        animator.SetFloat("Speed", moveDirection.sqrMagnitude);
    }
}
