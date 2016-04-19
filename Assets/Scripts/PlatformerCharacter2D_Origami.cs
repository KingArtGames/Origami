using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum CharacterShapes
{
    Fox,
    Fish,
    Bird,
    none
}

public class PlatformerCharacter2D_Origami : MonoBehaviour
{
    [SerializeField] private float m_MaxSpeed = 10f;         // The fastest the player can travel in the x axis.
    [SerializeField] private float m_AirSpeed = 5f;
    [SerializeField] private float m_GroundToAirSpeedFactor = 5f;
    [SerializeField] private float m_AirDragFactor = 5f;
    [SerializeField] private float m_JumpForce = 400f;       // Amount of force added when the player jumps.
    [SerializeField] private float m_DoubleJumpForce = 800f; // Amount of force added when the player double jumps.
    [SerializeField] private float m_TurnSpeed = 5f;
    [SerializeField] private float m_BirdWingCooldown = 1f;

    [SerializeField] private float blendSpeed = 8f;          // Speed used for blending
    [SerializeField] private float blendDelay = 8f;
    [SerializeField] private bool m_AirControl = false;      // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;       // A mask determining what is ground to the character
    [SerializeField] private GameObject fadeToWhitePlane;

    private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    private Transform m_CeilingCheck;   // A position marking where to check for ceilings
    const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
    private Animator m_Anim;            // Reference to the player's animator component.
    private Rigidbody2D m_Rigidbody2D;
    private CharachterStatus_Origami m_CharachterStatus_Origami;
    private SkinnedMeshRenderer m_SkinnedMeshRenderer;
    private Transform m_charMeshPivot;
    private SpriteRenderer m_fadePlaneRenderer;
    private GameObject char_mesh;
    private Renderer rend;

    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private CharacterShapes currentCharacterShape = CharacterShapes.Fox; // Shape1 is our default shape
    private int numBlendShapes = 3;
    private float currGroundToAirSpeed = 0f;
    private float currentBirdWingCooldownTime = 0f;
    private float currMoveVal = 0f;
    private bool doubleJumped = false;
    private int blossomCounter = 0;
    private bool fadeStartet = false;
    public Material FoxMat;
    public Material FishMat;
    public Material BirdMat;

    private void Awake()
    {
        // Setting up references.
        m_GroundCheck = transform.Find("GroundCheck");
        m_CeilingCheck = transform.Find("CeilingCheck");
        m_Anim = GetComponent<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_CharachterStatus_Origami = GetComponent<CharachterStatus_Origami>();
        m_charMeshPivot = transform.Find("char_mesh_pivot");
        m_SkinnedMeshRenderer = m_charMeshPivot.GetComponentInChildren<SkinnedMeshRenderer>();
        m_fadePlaneRenderer = fadeToWhitePlane.GetComponent<SpriteRenderer>();

        ShapeShift(CharacterShapes.Fox);
        AudioSource audio = GetComponent<AudioSource>();
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


        // game ends if at least 5 blossoms have been collected
        if (blossomCounter >= 5)
        {
            if (!fadeStartet)
            {
                fadeStartet = true;
                StartCoroutine("FadeToWhite");
            }
            ShapeShift(CharacterShapes.Fox);
            if (m_fadePlaneRenderer.color.a >= 1f)
            {
                SceneManager.LoadScene("story_end");
            }
        }
    }


    IEnumerator FadeToWhite()
    {
        float ElapsedTime = 0.0f;
        float TotalTime = 4.0f;
        Color c = m_fadePlaneRenderer.color;
        while (ElapsedTime < TotalTime)
        {
            ElapsedTime += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, (ElapsedTime / TotalTime));
            m_fadePlaneRenderer.color = c;
            yield return null;
        }
    }


    public void Move(float move, bool crouch, bool jump, CharacterShapes shape)
    {
        if (!m_Grounded)
        {
            currMoveVal = Mathf.Lerp(currMoveVal, move, Time.deltaTime * m_AirDragFactor);
        }else
        {
            currMoveVal = Mathf.Lerp(currMoveVal, move, Time.deltaTime * 3);
        }
        

        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {
            if (!m_Grounded)
            {
                currGroundToAirSpeed = Mathf.Lerp(currGroundToAirSpeed, m_AirSpeed, Time.deltaTime * m_GroundToAirSpeedFactor);
            }else{
                currGroundToAirSpeed = Mathf.Lerp(currGroundToAirSpeed, m_MaxSpeed, Time.deltaTime * 5);
            }
            // Move the character
            m_Rigidbody2D.velocity = new Vector2(currMoveVal * currGroundToAirSpeed, m_Rigidbody2D.velocity.y);
        }

        // If the player should jump...
        if (jump && !doubleJumped)
        {
            if (m_Grounded)
            {
                // Add a vertical force to the player.
                m_Grounded = false;
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            }
            else
            {
                doubleJumped = true;
                currentBirdWingCooldownTime = currentCharacterShape == CharacterShapes.Bird ? m_BirdWingCooldown : 0;
                m_Rigidbody2D.AddForce(new Vector2(0f, m_DoubleJumpForce));
            }
        }

        ProcessCharacterAbilities(move);
        ShapeShift(shape);
    }

    
    private void ProcessCharacterAbilities(float playerInputDir)
    {

        // Bird can swing it's wings, but has to wait for cooldown
        currentBirdWingCooldownTime = Mathf.Clamp(currentBirdWingCooldownTime -= Time.deltaTime, 0, m_BirdWingCooldown);

        if (currentCharacterShape == CharacterShapes.Bird)
        {
            if (currentBirdWingCooldownTime == 0)
                doubleJumped = false;
        }
        if (currentCharacterShape == CharacterShapes.Fish)
        {
            doubleJumped = false;
        }

        Vector2 moveDirection = m_Rigidbody2D.velocity;
        if (moveDirection != Vector2.zero)
        {
            Quaternion upDownRot = Quaternion.LookRotation(moveDirection, Vector3.up);
            Quaternion finalRot = Quaternion.Lerp(m_charMeshPivot.transform.rotation, upDownRot, Time.deltaTime * m_TurnSpeed);

            m_charMeshPivot.transform.rotation = finalRot;
        }
    }
    
    private void ShapeShift(CharacterShapes theShape)
    {
        CharacterShapes newChar = theShape != CharacterShapes.none ? theShape : currentCharacterShape;

        // check if characters have been unlocked
        if (newChar == CharacterShapes.Fish && !m_CharachterStatus_Origami.o_Fish)
            newChar = currentCharacterShape;

        if (newChar == CharacterShapes.Bird && !m_CharachterStatus_Origami.o_Bird)
            newChar = currentCharacterShape;

        // assign requested shape to current character shape
        if (currentCharacterShape != newChar)
        {
            GetComponent<AudioSource>().Play();
            if (newChar.ToString() == "Fox")
                GetComponentInChildren<Renderer>().material = FoxMat;
            if (newChar.ToString() == "Bird")
                GetComponentInChildren<Renderer>().material = BirdMat;
            if (newChar.ToString() == "Fish")
                GetComponentInChildren<Renderer>().material = FishMat;
            //Debug.Log("Why");
        }
        currentCharacterShape = newChar;
        // process blend weights
        for (int i = 0; i < numBlendShapes; i++)
        {
            float currWeight = m_SkinnedMeshRenderer.GetBlendShapeWeight(i);
            if (i == (int)currentCharacterShape)
            {
                m_SkinnedMeshRenderer.SetBlendShapeWeight(i, Mathf.Lerp(0, 100, (currWeight / 100) + Time.deltaTime * blendSpeed));
            }
            else
            {
                m_SkinnedMeshRenderer.SetBlendShapeWeight(i, Mathf.Lerp(0, 100, (currWeight / 100) - Time.deltaTime * blendSpeed * blendDelay));
            }
        }

        // Set values for character shapes
        switch (currentCharacterShape)
        {
            case CharacterShapes.Fox: // fox
                // main Settings
                m_MaxSpeed = 15f;
                m_AirSpeed = 10f;
                m_GroundToAirSpeedFactor = 3f;
                m_JumpForce = 400f;
                m_DoubleJumpForce = 800f;
                m_TurnSpeed = 8f;

                // rigid body settings
                m_Rigidbody2D.gravityScale = 3;

                break;
            case CharacterShapes.Fish: // fish
                // main Settings
                m_MaxSpeed = 3f;
                m_AirSpeed = 5f;
                m_GroundToAirSpeedFactor = 5f;
                m_JumpForce = 200f;
                m_DoubleJumpForce = 400f;
                m_TurnSpeed = 8f;

                // rigid body settings
                m_Rigidbody2D.gravityScale = 1f;
                break;
            case CharacterShapes.Bird: // bird
                // main Settings
                m_MaxSpeed = 5f;
                m_AirSpeed = 20f;
                m_GroundToAirSpeedFactor = 3f;
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

    public float GetCurrentWingCooldownTime()
    {
        return currentBirdWingCooldownTime;
    }

    public float GetWingCooldownTime()
    {
        return m_BirdWingCooldown;
    }

    public void RaiseBlossemCounter(int count)
    {
        blossomCounter += count;
    }

    public void SetBlossemCounter(int num)
    {
        blossomCounter = num;
    }

    public int GetBlossemCount()
    {
        return blossomCounter;
    }
}
