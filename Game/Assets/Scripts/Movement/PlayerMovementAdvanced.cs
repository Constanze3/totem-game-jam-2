using System;
using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerMovementAdvanced : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;
    public float wallrunSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Footsteps")]
    [SerializeField]
    private AudioClip footstepClip;

    [SerializeField]
    private float baseFootstepVolume = 0.5f;

    [SerializeField]
    private float walkPitch = 1f;

    [SerializeField]
    private float sprintPitch = 1.2f;

    [SerializeField]
    private float crouchPitch = 0.85f;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    private AudioSource audioSource;

    public MovementState state;

    public enum MovementState
    {
        walking,
        sprinting,
        wallrunning,
        crouching,
        sliding,
        air,
    }

    public bool sliding;
    public bool crouching;
    public bool wallrunning;

    public TextMeshProUGUI text_speed;
    public TextMeshProUGUI text_mode;

    private void OnEnable()
    {
        rb.isKinematic = false;
    }

    private void OnDisable()
    {
        audioSource.Stop();
        rb.isKinematic = true;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rb.freezeRotation = true;

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.clip = footstepClip;

        readyToJump = true;
        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        grounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            playerHeight * 0.5f + 0.2f,
            whatIsGround
        );

        MyInput();
        SpeedControl();
        StateHandler();
        HandleFootsteps();

        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(
                transform.localScale.x,
                crouchYScale,
                transform.localScale.z
            );
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

            crouching = true;
        }

        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(
                transform.localScale.x,
                startYScale,
                transform.localScale.z
            );

            crouching = false;
        }
    }

    private void StateHandler()
    {
        if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallrunSpeed;
        }
        else if (sliding)
        {
            state = MovementState.sliding;

            if (OnSlope() && rb.linearVelocity.y < 0.1f)
                desiredMoveSpeed = slideSpeed;
            else
                desiredMoveSpeed = sprintSpeed;
        }
        else if (crouching)
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
        else if (grounded && Input.GetKey(sprintKey) && !Input.GetKey(crouchKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }

        if (state == MovementState.crouching || state == MovementState.sprinting)
        {
            StopAllCoroutines();
            moveSpeed = desiredMoveSpeed;
        }
        else if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time +=
                    Time.deltaTime
                    * speedIncreaseMultiplier
                    * slopeIncreaseMultiplier
                    * slopeAngleIncrease;
            }
            else
            {
                time += Time.deltaTime * speedIncreaseMultiplier;
            }

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.linearVelocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            rb.AddForce(
                moveDirection.normalized * moveSpeed * 10f * airMultiplier,
                ForceMode.Force
            );
        }

        if (!wallrunning)
            rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (rb.linearVelocity.magnitude > moveSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
    }

    private void HandleFootsteps()
    {
        bool hasMovementInput = horizontalInput != 0 || verticalInput != 0;
        bool shouldPlay =
            grounded && hasMovementInput && !wallrunning && state != MovementState.air;

        if (footstepClip == null)
            return;

        if (shouldPlay)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();

            switch (state)
            {
                case MovementState.sprinting:
                    audioSource.pitch = sprintPitch;
                    audioSource.volume = baseFootstepVolume;
                    break;

                case MovementState.crouching:
                    audioSource.pitch = crouchPitch;
                    audioSource.volume = baseFootstepVolume * 0.7f;
                    break;

                default:
                    audioSource.pitch = walkPitch;
                    audioSource.volume = baseFootstepVolume;
                    break;
            }
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if (
            Physics.Raycast(
                transform.position,
                Vector3.down,
                out slopeHit,
                playerHeight * 0.5f + 0.3f
            )
        )
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, digits);
        return Mathf.Round(value * mult) / mult;
    }
}
