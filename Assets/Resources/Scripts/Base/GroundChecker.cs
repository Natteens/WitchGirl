using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Witchgame
{
    public class GroundChecker : MonoBehaviour
    {
        [Header("Ground Check Settings")]
        public Transform groundCheck;
        public float checkRadius = 0.2f;
        public LayerMask groundLayer;

        public bool isGrounded { get; private set; }

        private void Update()
        {
            CheckGround();
        }

        private void CheckGround()
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}
