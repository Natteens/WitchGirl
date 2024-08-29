using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Witchgame
{
    [RequireComponent(typeof(Rigidbody2D), typeof(GroundChecker))]
    public class PlayerActions : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float accelerationSpeed = 1.5f;
        public float maxSpeed = 4f;
        public float decelerationSpeed = 2f;

        [Header("Jump Settings")]
        public float jumpHeight = 2f;
        public float jumpTimeToApex = 0.4f;
        public float jumpForceMultiplier = 1.5f;
        public float fallMultiplier = 2.5f;
        public float lowJumpMultiplier = 2f;
        public float coyoteTime = 0.2f;
        public float jumpBufferTime = 0.1f;

        [Header("Other Settings")]
        public bool canJump = true;
        public bool canCastSpell = true;

        private Animator anim;
        private Rigidbody2D rb;
        private InputController inputController;
        private GroundChecker groundChecker;
        private SpriteRenderer spriteRenderer;

        private float coyoteTimeCounter;
        private float jumpBufferCounter;
        private bool isJumping;
        private bool isFalling;
        private bool isCastingSpell;
        private float jumpVelocity;
        private float gravity;

        private void Awake()
        {
            GetComponents();
            CalculateJumpParameters();
        }

        private void GetComponents()
        {
            anim = GetComponentInChildren<Animator>();
            rb = GetComponent<Rigidbody2D>();
            inputController = GetComponent<InputController>();
            groundChecker = GetComponent<GroundChecker>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void CalculateJumpParameters()
        {
            gravity = -(2 * jumpHeight) / Mathf.Pow(jumpTimeToApex, 2);
            jumpVelocity = Mathf.Abs(gravity) * jumpTimeToApex;
            rb.gravityScale = Mathf.Abs(gravity) / Physics2D.gravity.magnitude;
        }

        private void Update()
        {
            HandleMovement();
            HandleJumpInput();
            HandleCasting();
        }

        private void FixedUpdate()
        {
            ApplyJump();
            ApplyGravity();
        }

        private void HandleMovement()
        {
            Vector2 input = inputController.move;
            float targetSpeed = input.x * maxSpeed;
            float speedDifference = targetSpeed - rb.velocity.x;

            float accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accelerationSpeed : decelerationSpeed;
            float movement = Mathf.Pow(Mathf.Abs(speedDifference) * accelerationRate, 0.96f) * Mathf.Sign(speedDifference);

            rb.AddForce(movement * Vector2.right);

            if (Mathf.Abs(input.x) < 0.01f && Mathf.Abs(rb.velocity.x) < 0.1f)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }

            anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

            if (input.x != 0)
            {
                spriteRenderer.flipX = input.x < 0;
            }
        }

        private void HandleJumpInput()
        {
            if (groundChecker.isGrounded)
            {
                coyoteTimeCounter = coyoteTime;
                isJumping = false;
                isFalling = false;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }

            if (inputController.jump)
            {
                jumpBufferCounter = jumpBufferTime;
            }
            else
            {
                jumpBufferCounter -= Time.deltaTime;
            }
        }

        private void ApplyJump()
        {
            if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f && canJump && !isJumping)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpVelocity * jumpForceMultiplier);
                jumpBufferCounter = 0f;
                isJumping = true;
                isFalling = false;
                anim.SetBool("isJumping", true);
                anim.SetBool("isFalling", false);
            }

            if (!inputController.jump && rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }

            if (rb.velocity.y < 0 && !groundChecker.isGrounded)
            {
                isFalling = true;
                isJumping = false;
                anim.SetBool("isJumping", false);
                anim.SetBool("isFalling", true);
            }
        }

        private void ApplyGravity()
        {
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * gravity * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rb.velocity.y > 0 && !inputController.jump)
            {
                rb.velocity += Vector2.up * gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }

        private void HandleCasting()
        {
            if (canCastSpell && inputController.spell)
            {
                isCastingSpell = true;
                anim.SetBool("isCastingSpell", true);
                inputController.spell = false;
            }

            if (isCastingSpell)
            {
                AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
                anim.SetFloat("CastingState", groundChecker.isGrounded ? 0f : 1f);

                if (stateInfo.IsName("CastSpell") && stateInfo.normalizedTime >= 1f)
                {
                    isCastingSpell = false;
                    anim.SetBool("isCastingSpell", false);
                }
            }
        }

    }
}