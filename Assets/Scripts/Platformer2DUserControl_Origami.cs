using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof (PlatformerCharacter2D_Origami))]
    public class Platformer2DUserControl_Origami : MonoBehaviour
    {
        private PlatformerCharacter2D_Origami m_Character;
        private bool m_Jump;
        CharacterShapes m_requestedShape = CharacterShapes.none;

        private void Awake()
        {
            m_Character = GetComponent<PlatformerCharacter2D_Origami>();
        }


        private void Update()
        {
            if (!m_Jump)
            {
                // Read the jump input in Update so button presses aren't missed.
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (CrossPlatformInputManager.GetButtonDown("Shape1"))
            {
                m_requestedShape = CharacterShapes.Shape1;
            }
            else if (CrossPlatformInputManager.GetButtonDown("Shape2"))
            {
                m_requestedShape = CharacterShapes.Shape2;
            }
            else if (CrossPlatformInputManager.GetButtonDown("Shape3"))
            {
                m_requestedShape = CharacterShapes.Shape3;
            }

        }


        private void FixedUpdate()
        {
            // Read the inputs.
            //bool crouch = Input.GetKey(KeyCode.LeftControl);
            bool crouch = false; // no crouching for now
            float h = CrossPlatformInputManager.GetAxis("Horizontal");

            // Pass all parameters to the character control script.
            m_Character.Move(h, crouch, m_Jump, m_requestedShape);
            m_Jump = false;
            m_requestedShape = CharacterShapes.none;
        }
    }
}
