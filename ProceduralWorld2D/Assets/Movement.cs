using System.Collections;
using System.Diagnostics;
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
}