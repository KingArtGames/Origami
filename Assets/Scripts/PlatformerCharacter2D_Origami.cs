using System;
using UnityEngine;


public enum CharacterShapes
{
    Shape1,
    Shape2,
    Shape3,
    none
}

public class PlatformerCharacter2D_Origami : MonoBehaviour
{
    [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
    [SerializeField] private float m_AirSpeed = 5f;
    [SerializeField] private float m_SpeedLerpFactor = 5f;
    [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
    [SerializeField] private float m_DoubleJumpForce = 800f;                  // Amount of force added when the player double jumps.
    [SerializeField] private float m_TurnSpeed = 5f;
    [SerializeField] private float m_BirdWingCooldown = 1f;

    [SerializeField] private float blendSpeed = 8f;                      // Speed used for blending
    [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

    private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    private Transform m_CeilingCheck;   // A position marking where to check for ceilings
    const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
    private Animator m_Anim;            // Reference to the player's animator component.
    private Rigidbody2D m_Rigidbody2D;
    private SkinnedMeshRenderer m_SkinnedMeshRenderer;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private CharacterShapes currentCharacterShape = CharacterShapes.Shape1; // Shape1 is our default shape
    private int numBlendShapes = 3;
    private float currSpeed = 0f;
    private float currentBirdWingCooldownTime = 0f;

    private bool doubleJumped = false;

    private void Awake()
    {
        // Setting up references.
        m_GroundCheck = transform.Find("GroundCheck");
        m_CeilingCheck = transform.Find("CeilingCheck");
        m_Anim = GetComponent<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_SkinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }


    private void FixedUpdate()
    {
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject) { 
                m_Grounded = true;
                doubleJumped = false;
            }
        }
    }


    public void Move(float move, bool crouch, bool jump, CharacterShapes shape)
    {
        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {
            if (!m_Grounded)
            {
                currSpeed = Mathf.Lerp(currSpeed, m_AirSpeed, Time.deltaTime * m_SpeedLerpFactor);
            }else{
                currSpeed = Mathf.Lerp(currSpeed, m_MaxSpeed, Time.deltaTime * 5);
            }
            // Move the character
            m_Rigidbody2D.velocity = new Vector2(move*currSpeed, m_Rigidbody2D.velocity.y);
        }

        // If the player should jump...
        if (jump && !doubleJumped)
        {
            if (m_Grounded)
            {
                // Add a vertical force to the player.
                m_Grounded = false;
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            }else{
                doubleJumped = true;
                currentBirdWingCooldownTime = currentCharacterShape == CharacterShapes.Shape3 ? m_BirdWingCooldown : 0;
                m_Rigidbody2D.AddForce(new Vector2(0f, m_DoubleJumpForce));
            }
        }

        // turn player to move direction
        m_FacingRight = move != 0 ? move > 0 : m_FacingRight;

        ProcessCharacterAbilities();
        TurnPlayer();
        ShapeShift(shape);
    }


    private void ProcessCharacterAbilities()
    {
        if (currentCharacterShape == CharacterShapes.Shape3)
        { 
            // Bird can swing it's wings, but has to wait for cooldown
            currentBirdWingCooldownTime = Mathf.Clamp(currentBirdWingCooldownTime -= Time.deltaTime, 0, m_BirdWingCooldown);

            if (currentBirdWingCooldownTime == 0)
                doubleJumped = false;
        }

    }

    private void TurnPlayer()
    {
        float currRotation = m_SkinnedMeshRenderer.transform.rotation.eulerAngles.y;

        if (m_FacingRight)
        {
            m_SkinnedMeshRenderer.transform.Rotate(Vector3.up, (0 - currRotation) * Time.deltaTime * m_TurnSpeed);
        }
        else
        {
            m_SkinnedMeshRenderer.transform.Rotate(Vector3.up, (180 - currRotation) * Time.deltaTime * m_TurnSpeed);
        }
    }

    private void ShapeShift(CharacterShapes theShape)
    {
        currentCharacterShape = theShape != CharacterShapes.none ? theShape : currentCharacterShape;

        for (int i = 0; i < numBlendShapes; i++)
        {
            float currWeight = m_SkinnedMeshRenderer.GetBlendShapeWeight(i);
            if (i == (int)currentCharacterShape)
            {
                m_SkinnedMeshRenderer.SetBlendShapeWeight(i, Mathf.Lerp(0, 100, (currWeight / 100) + Time.deltaTime * blendSpeed));
            }
            else
            {
                m_SkinnedMeshRenderer.SetBlendShapeWeight(i, Mathf.Lerp(0, 100, (currWeight / 100) - Time.deltaTime * blendSpeed));
            }
        }

        // Set values for character shapes
        switch (currentCharacterShape)
        {
            case CharacterShapes.Shape1: // fox
                // main Settings
                m_MaxSpeed = 15f;
                m_AirSpeed = 10f;
                m_SpeedLerpFactor = 3f;
                m_JumpForce = 400f;
                m_DoubleJumpForce = 800f;
                m_TurnSpeed = 8f;

                // rigid body settings
                m_Rigidbody2D.gravityScale = 3;

                break;
            case CharacterShapes.Shape2: // fish
                // main Settings
                m_MaxSpeed = 10f;
                m_AirSpeed = 5f;
                m_SpeedLerpFactor = 5f;
                m_JumpForce = 200f;
                m_DoubleJumpForce = 400f;
                m_TurnSpeed = 8f;

                // rigid body settings
                m_Rigidbody2D.gravityScale = 1;
                break;
            case CharacterShapes.Shape3: // bird
                // main Settings
                m_MaxSpeed = 5f;
                m_AirSpeed = 20f;
                m_SpeedLerpFactor = 3f;
                m_JumpForce = 200f;
                m_DoubleJumpForce = 600f;
                m_TurnSpeed = 8f;

                // rigid body settings
                m_Rigidbody2D.gravityScale = 1f;
                break;
            case CharacterShapes.none:
                break;
            default:
                break;
        }
    }

    public CharacterShapes GetCurrentCharacterShape()
    {
        return currentCharacterShape;
    }
}
