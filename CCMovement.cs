using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*This is Character Controller Movement 
by NnNeEediIMm with WallRuning*/

public class CCMovement : MonoBehaviour
{
    // movement
    float x, z;
    Vector3 move;
    [Header("Movement")]
    public float moveSpeed = 4f;
    static float defaultSpeed;

    //gravity
    Vector3 velocity;
    [Header("Gravity")]
    public float gravityScale = -9.81f;


    public float checkRadius = 0.011f;
    public LayerMask groundMask;
    private bool isGrounded;

    private GameObject groundCheck;
    float endOfY;

    //component variables 
    protected CharacterController control;
    protected cameraMove cameraComponent;

    //some mechanics
    bool jumping, crouching, sprinting;

    //Jumping 
    [Header("Jump")]
    public KeyCode jumpInput = KeyCode.Space;
    public float jumpForce = 1.5f;

    //crouching 
    [Header("Crouching")]
    public bool canCrouch = true;
    public KeyCode crouchInput = KeyCode.LeftShift;
    bool crouchingInputUp;
    float speedWhileCrouching;
    static float defaultSize;
    float reducedSize;

    //sprinting
    [Header("Sprint")]
    public bool canSprint = true;
    public KeyCode sprintInput = KeyCode.LeftControl;
    float sprintSpeed;
    bool sprintInputUp;

    //Wallrun
    [Header("Character Controller WallRun")]
    public bool canYouWallRun = true;
    public float cameraRotateAngle = 2f;
    public LayerMask wall;
    static float defaultGravity;
    public GameObject playerCamera;
    bool isWallRunning = false;

    private void Start()
    {
        GameObject check = new GameObject("Check");
        groundCheck = GameObject.Find("Check");
        groundCheck.transform.parent = this.transform;

        control = GetComponent<CharacterController>();

        //Some components 
        defaultSpeed = moveSpeed;
        speedWhileCrouching = moveSpeed / 2f;
        defaultSize = transform.localScale.y;
        reducedSize = transform.localScale.y / 2;
        sprintSpeed = moveSpeed * 1.57f;


        /*for wallrun*/
        cameraComponent = playerCamera.GetComponent<cameraMove>();
        defaultGravity = gravityScale;
    }

    private void Update()
    {
        myInput();

        gravity();

        movement();

        jump();

        crouch();

        sprint();

        wallRun();

        //for main movement
        move = transform.forward * z * moveSpeed + transform.right * x * moveSpeed;
        endOfY = transform.lossyScale.y;
    }

    private void myInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        jumping = Input.GetKeyDown(jumpInput);

        crouching = Input.GetKey(crouchInput) && x < 0.5f;
        crouchingInputUp = Input.GetKeyUp(crouchInput);

        sprinting = Input.GetKey(sprintInput) && x < 0.5f;
        sprintInputUp = Input.GetKeyUp(sprintInput);
    }

    private void movement()
    {
        control.Move(move.normalized * moveSpeed * Time.deltaTime);
    }

    private void gravity()
    {
        groundCheck.transform.position = new Vector3(transform.position.x, transform.position.y - endOfY, transform.position.z);
        isGrounded = Physics.CheckSphere(groundCheck.transform.position, checkRadius, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravityScale * Time.deltaTime;

        if (!isWallRunning)
        {
            control.Move(velocity * Time.deltaTime);
        }

    }

    public void jump()
    {
        if (isGrounded && jumping)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravityScale);
        }
    }

    //Crouching!!
    public void crouch()
    {
        if (canCrouch)
        {
            if (crouching)
            {
                transform.localScale = new Vector3(transform.localScale.x, reducedSize, transform.localScale.z);
                moveSpeed = speedWhileCrouching;
            }
            else
            {
                notCrouching();
            }

            if (crouchingInputUp)
            {
                moveSpeed = defaultSpeed;
            }
        }

    }

    void notCrouching()
    {
        transform.localScale = new Vector3(transform.localScale.x, defaultSize, transform.localScale.z);
    }

    //sprinting
    public void sprint()
    {
        if (canSprint)
        {
            if (isGrounded && sprinting)
            {
                moveSpeed = sprintSpeed;
            }
            if (sprintInputUp)
            {
                notSprinting();
            }
        }

    }

    void notSprinting()
    {
        moveSpeed = defaultSpeed;
    }

    /// <summary>
    /// built-in
    /// WallRun 
    /// </summary>
    RaycastHit hit;

    public void wallRun()
    {
        if (canYouWallRun)
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, 1f, wall))
            {
                gravityScale = 0;
                cameraComponent.eulerAngle = cameraRotateAngle;
                isWallRunning = true;
            }
            else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, 1f, wall))
            {
                gravityScale = 0;
                cameraComponent.eulerAngle = -cameraRotateAngle;
                isWallRunning = true;
            }
            else
            {
                gravityScale = defaultGravity;
                cameraComponent.eulerAngle = 0f;
                isWallRunning = false;
            }
        }

    }
}
