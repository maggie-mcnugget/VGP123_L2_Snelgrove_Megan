using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    //public GameObject groundCheckTransform;
    [Header("Ground Check Settings")]
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.02f;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;


    private Rigidbody2D _rb;
    private Collider2D _collider;
    private SpriteRenderer _sr;
    private Animator _anim;
    private GroundCheck _groundCheck;

    private bool _isAttack = false;
    private bool _isGrounded = false;
    private bool _isJumpAttack = false;
    private bool _isFiring = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _sr = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();

        _groundCheck = new GroundCheck(_collider, _rb, groundCheckRadius, groundLayer);

        ////initalize the ground check object here rather than in the inpsector for safety - only if we use a gameobject to get our foot position
        //if (groundCheckTransform == null)
        //{
        //    groundCheckTransform = new GameObject("GroundCheck");
        //    groundCheckTransform.transform.SetParent(transform);
        //    groundCheckTransform.transform.localPosition = Vector3.zero;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        _isGrounded = _groundCheck.IsGrounded();

        //input handling
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        bool jumpInput = Input.GetButtonDown("Jump");
        bool attackInput = Input.GetButton("Attack");
        bool fireInput = Input.GetButtonDown("Fire");

        if (!_isFiring)
        {
            Vector2 velocity = _rb.linearVelocity;
            velocity.x = horizontalInput * moveSpeed
;           _rb.linearVelocity = velocity;
        }
       


        if (horizontalInput != 0) SpriteFlip(horizontalInput);

        //jumping
        if (jumpInput && _isGrounded)
        {
            _rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }

        if (fireInput && !_isFiring)
        {
            _isFiring = true;
        }

        if (!_isGrounded && attackInput)
        {
            _isJumpAttack = true;
        }
        else if (attackInput && _isGrounded)
        {
            _isAttack = true;
        }

        if (!attackInput)
        {
            _isAttack = false;
            _isJumpAttack = false;
        }
        //animation
        _anim.SetFloat("moveInput", Mathf.Abs(horizontalInput));
        _anim.SetFloat("yVel", _rb.linearVelocity.y);
        _anim.SetBool("isGrounded", _isGrounded);
        _anim.SetBool("isAttack", _isAttack);
        _anim.SetBool("isJumpAttack", _isJumpAttack);
        _anim.SetBool("Fire", _isFiring);
    }

    /// <summary>
    /// Sprite flipping based on horizontal input - this function should only be called when horizontal input is non-zero
    /// </summary>
    /// <param name="horizontalInput">The input received from Unity's input system</param>
    private void SpriteFlip(float horizontalInput) => _sr.flipX = (horizontalInput < 0);

    public void ResetFireAnimation()
    {
        _isFiring = false;
    }
}