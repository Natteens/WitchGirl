using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Witchgame
{
    public class InputController : MonoBehaviour
    {
        [field: SerializeField] public Vector2 move { get;  set; }
        [field: SerializeField] public Vector2 mousePos { get;  set; }
        [field: SerializeField] public bool jump { get;  set; }
        [field: SerializeField] public bool interact { get;  set; }
        [field: SerializeField] public bool spell { get;  set; }

        public void OnMove(InputValue value)
        {
            movement(value.Get<Vector2>());
        }

        public void OnJump(InputValue value)
        {
            jumping(value.isPressed);
        }

        public void OnInteract(InputValue value)
        {
            interaction(value.isPressed);
        }

        public void OnSpell(InputValue value)
        {
            castingSpell(value.isPressed);
        }

        public void OnMouse(InputValue value)
        {
            mousePosition(value.Get<Vector2>());
        }

        public void movement(Vector2 vector2)
        {
            move = vector2;
        }

        public void mousePosition(Vector2 vector2)
        {
            mousePos = vector2;
        }

        public void jumping(bool state)
        {
            jump = state;
        }

        public void castingSpell(bool state)
        {
            spell = state;
        }
        public void interaction(bool state)
        {
            interact = state;
        }
    }
}
