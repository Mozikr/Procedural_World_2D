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
        // Wywo³ujemy funkcjê MoveNPC co 2 sekundy (mo¿esz dostosowaæ interwa³)
        InvokeRepeating("MoveNPC", 0f, 2f);
    }

    private void MoveNPC()
    {
        // Generujemy losow¹ wartoœæ dla osi x i y
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-1f, 1f);

        // Tworzymy wektor kierunku ruchu na podstawie wygenerowanych wartoœci
        Vector2 moveDirection = new Vector2(randomX, randomY).normalized;

        // Ustawiamy prêdkoœæ ruchu NPC
        rb.velocity = moveDirection * moveSpeed;

        // Ustawiamy animacjê w zale¿noœci od kierunku ruchu
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
