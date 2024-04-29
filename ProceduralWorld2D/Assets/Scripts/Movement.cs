using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Movement : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 3f;
    Rigidbody2D _rb;
    public Animator _anim;
    Vector2 _movement;
    AudioManager _audioManager;

    public GameObject replacementPrefab; // Przygotowany prefabrykat do postawienia
    public TextMeshProUGUI scoreText; // Referencja do komponentu TextMeshPro

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



    private int score = 0; // Licznik punktów

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Fox"))
        {
            Destroy(collision.gameObject); // Usuwamy obiekt z tagiem "Fox"
            Vector3 position = collision.transform.position; // Pobieramy pozycjê obiektu
            Quaternion rotation = collision.transform.rotation; // Pobieramy rotacjê obiektu
            Instantiate(replacementPrefab, position, rotation); // Stawiamy prefabrykat w miejscu zniszczonego obiektu

            // Zwiêkszamy licznik punktów i aktualizujemy wyœwietlany tekst
            score++;
            UpdateScoreText();
        }
    }
    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString();
    }
}