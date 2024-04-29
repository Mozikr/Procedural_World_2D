using TMPro;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 3f;
    private int score = 0;
    Rigidbody2D _rb;
    public Animator _anim;
    Vector2 _movement;
    AudioManager _audioManager;

    public GameObject replacementPrefab;
    public GameObject particleEffect;
    public TextMeshProUGUI scoreText;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _audioManager = FindObjectOfType<AudioManager>();
    }

    void Update()
    {
        _movement.x = Input.GetAxisRaw("Horizontal");
        _movement.y = Input.GetAxisRaw("Vertical");
        _anim.SetFloat("Horizontal", _movement.x);
        _anim.SetFloat("Vertical", _movement.y);
        _anim.SetFloat("Speed", _movement.sqrMagnitude);
    }

    private void FixedUpdate()
    {
        PlayFootSteps();
        Move();
    }

    private void Move()
    {
        _rb.MovePosition(_rb.position + _movement * _moveSpeed * Time.fixedDeltaTime);
    }

    private void PlayFootSteps()
    {
        if (_movement.sqrMagnitude > 0)
        {
            _audioManager.Play("StepGrass");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Fox"))
        {
            Destroy(collision.gameObject);
            _audioManager.Play("Poof");
            Vector3 position = collision.transform.position;
            Quaternion rotation = collision.transform.rotation;
            Instantiate(replacementPrefab, position, rotation);
            Instantiate(particleEffect, position, rotation);
            score++;
            UpdateScoreText();
        }
    }
    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString();
    }
}