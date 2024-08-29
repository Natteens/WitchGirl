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
        public float jumpHeight = 5f;
        public float jumpSpeed = 10f;
        public float gravityForce = 20f;
        public float coyoteTime = 0.2f;
        public float jumpBufferTime = 0.1f;
        public float quickFallMultiplier = 2f;

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

        private void Awake()
        {
            GetComponents();
            CalculateJumpVelocity();
        }

        private void GetComponents()
        {
            anim = GetComponentInChildren<Animator>();
            rb = GetComponent<Rigidbody2D>();
            inputController = GetComponent<InputController>();
            groundChecker = GetComponent<GroundChecker>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void CalculateJumpVelocity()
        {
            float timeToApex = Mathf.Sqrt(2 * jumpHeight / gravityForce);
            jumpSpeed = gravityForce * timeToApex;
        }

        private void Update()
        {
            HandleMovement();
            HandleCasting();
        }

        private void FixedUpdate()
        {
            ApplyMovement();
            HandleJumping();
            HandleFalling();
        }

        private void HandleMovement()
        {
            Vector2 input = inputController.move;
            float targetSpeed = input.x * maxSpeed;
            float speedDifference = targetSpeed - rb.velocity.x;
            float accelerationRate = Mathf.Abs(targetSpeed) > 0.01f ? accelerationSpeed : decelerationSpeed;
            float movement = Mathf.Pow(Mathf.Abs(speedDifference) * accelerationRate, 2) * Mathf.Sign(speedDifference);

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

        private void ApplyMovement()
        {
            Vector2 input = inputController.move;
            float targetSpeed = input.x * maxSpeed;
            float speedDifference = targetSpeed - rb.velocity.x;
            float accelerationRate = Mathf.Abs(targetSpeed) > 0.01f ? accelerationSpeed : decelerationSpeed;
            float movement = Mathf.Pow(Mathf.Abs(speedDifference) * accelerationRate, 2) * Mathf.Sign(speedDifference);

            rb.AddForce(movement * Vector2.right);

            if (Mathf.Abs(input.x) < 0.01f && Mathf.Abs(rb.velocity.x) < 0.1f)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }

        private void HandleJumping()
        {
            if (groundChecker.isGrounded)
            {
                coyoteTimeCounter = coyoteTime;
                isFalling = false;
                anim.SetBool("isFalling", false);
                anim.SetBool("isJumping", false);
            }
            else
            {
                coyoteTimeCounter -= Time.fixedDeltaTime;
            }

            if (inputController.jump)
            {
                jumpBufferCounter = jumpBufferTime;
            }
            else
            {
                jumpBufferCounter -= Time.fixedDeltaTime;
            }

            if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f && canJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
                jumpBufferCounter = 0f;
                isJumping = true;
                anim.SetBool("isJumping", true);
            }

            if (isJumping && !inputController.jump && rb.velocity.y > 0)
            {
                rb.velocity += Vector2.down * gravityForce * quickFallMultiplier * Time.fixedDeltaTime;
            }

            if (isJumping && rb.velocity.y < 0)
            {
                isJumping = false;
                isFalling = true;
                anim.SetBool("isJumping", false);
                anim.SetBool("isFalling", true);
            }
        }

        private void HandleFalling()
        {
            if (!groundChecker.isGrounded && rb.velocity.y < 0)
            {
                isFalling = true;
                isJumping = false;
                anim.SetBool("isJumping", false);
                anim.SetBool("isFalling", true);
            }

            if (groundChecker.isGrounded && isFalling)
            {
                isFalling = false;
                anim.SetBool("isFalling", false);
            }

            if (isFalling)
            {
                rb.velocity += Vector2.down * gravityForce * Time.fixedDeltaTime;
            }
        }

        private void HandleCasting()
        {
            if (canCastSpell && inputController.spell)
            {
                isCastingSpell = true;
                anim.SetBool("isCastingSpell", true);

                int castState = groundChecker.isGrounded ? 0 : 1;
                anim.SetFloat("CastState", castState);

                inputController.spell = false;
            }
            else if (isCastingSpell && !inputController.spell)
            {
                isCastingSpell = false;
                anim.SetBool("isCastingSpell", false);
            }
        }
    }
}
