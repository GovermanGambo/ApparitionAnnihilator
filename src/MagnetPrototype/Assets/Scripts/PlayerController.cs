using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float force;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float jumpCancleForce;
    [SerializeField] private float startingSpeed;
    [SerializeField] private InputAction moveAction;
    [SerializeField] private InputAction jumpAction;
    [SerializeField] private InputAction toggleMagnetAction;
    [SerializeField] private InputAction cheatAction;
    [SerializeField] private InputAction resetAction;

    // Variables for rotation state
    private bool isRotating = false;
    private float rotDistance;
    private float rotAngle;
    private float rotPrevAngle;
    private float rotVelocity;
    private Vector3 rotPosition;
    private bool isTouching = false;
    private Collider2D rotCollider;

    private bool isJumping = false;
    private bool jumpKeyReleased = true;
    private bool isDead;

    public float HorizontalSpeed => rigidBody2D.velocity.x;

    public event EventHandler OnPlayerDeath;
    public event EventHandler OnToggleForceField;
    public PlayerData PlayerData => new PlayerData(transform.position, spriteTransform.localScale);
    private Transform spriteTransform;
    public bool IsForceFieldActive => magnetTrigger.enabled;
    public int isInForceField = 0;

    public PlayerState PlayerState
    {
        get => playerState;
        set
        {
            rigidBody2D.isKinematic = value == PlayerState.Blocked;
            playerState = value;
        }
    }

    public void SetRotation(Vector3 forceFieldPosition)
    {
        isRotating = true;
        isTouching = false;
        rotPosition = forceFieldPosition;

        var distanceVector = (rigidBody2D.transform.position - rotPosition);
        var distanceVector90 = new Vector2(distanceVector.y, -distanceVector.x);

        rotVelocity = -Vector2.Dot(rigidBody2D.velocity, distanceVector90.normalized);
        rotDistance = distanceVector.magnitude;
        rotAngle = Mathf.Atan2(distanceVector.y, distanceVector.x);
        rotPrevAngle = rotAngle;

        Debug.Log("Enter");

        rigidBody2D.isKinematic = true;
    }

    public void RemoveRoation()
    {
        isRotating = false;

        var velocityVector = new Vector2(-rotVelocity * Mathf.Sin(rotAngle), rotVelocity * Mathf.Cos(rotAngle));
        rigidBody2D.velocity = velocityVector;

        rigidBody2D.isKinematic = false;
    }

    private Rigidbody2D rigidBody2D;
    private BoxCollider2D hitbox;
    private CircleCollider2D magnetTrigger;
    private float currentDirection;
    private PlayerState playerState = PlayerState.Normal;

    private bool isGrounded
    {
        get
        {
            LayerMask groundLayer = LayerMask.GetMask("Ground");
            bool isGroundedDownLeft = Physics2D.Raycast(transform.position + Vector3.left * 0.5f, Vector2.down, 0.55f, groundLayer);
            bool isGroundedDownRight = Physics2D.Raycast(transform.position + Vector3.right * 0.5f, Vector2.down, 0.55f, groundLayer);
            return isGroundedDownLeft || isGroundedDownRight;
        }
    }
    
    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<BoxCollider2D>();
        magnetTrigger = GetComponent<CircleCollider2D>();
        jumpAction.performed += OnJump;
        jumpAction.canceled += (InputAction.CallbackContext obj) => jumpKeyReleased = true;
        toggleMagnetAction.performed += (InputAction.CallbackContext obj) =>
        {
            if (PlayerState == PlayerState.Blocked) return;
            AudioManager.Instance.PlaySound("forceFieldActivate", 0.6f);
            magnetTrigger.enabled = true;
            OnToggleForceField?.Invoke(this, null);
        };
        toggleMagnetAction.canceled += (InputAction.CallbackContext obj) => 
        {
            if (PlayerState == PlayerState.Blocked) return;
            AudioManager.Instance.PlaySound("forceFieldDeactivate", 0.6f);
            magnetTrigger.enabled = false;
            OnToggleForceField?.Invoke(this, null);
        };
        cheatAction.performed += CheatAction;
        resetAction.performed += ResetAction;
        spriteTransform = transform.Find("sprite");
    }

    private void OnJump(InputAction.CallbackContext obj)
    {
        if (!isGrounded) return;
        
        AudioManager.Instance.PlaySound("whoosh", 0.7f);
        isJumping = true;
        jumpKeyReleased = false;
        rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, jumpSpeed);
    }

/*    private void OnToggleMagnet(InputAction.CallbackContext obj)
    {
        var circleCollider2Ds = GetComponents<CircleCollider2D>();
        foreach (var circleCollider2D in circleCollider2Ds)
        {
            if (circleCollider2D.isTrigger)
            {
                circleCollider2D.enabled = !circleCollider2D.enabled;
            }
        }
    }*/

    private void ResetAction(InputAction.CallbackContext obj)
    {
        Kill();
    }

    private void CheatAction(InputAction.CallbackContext obj)
    {
        rigidBody2D.velocity *= 2;
        rotVelocity *= 2;
        Debug.Log("Cheated");
    }

    private void Update()
    {
        if (playerState == PlayerState.Blocked)
        {
            return;
        }
        
        currentDirection = moveAction.ReadValue<float>();
    }

    private void FixedUpdate()
    {
        if (!isRotating)
        {
            var appliedForce = isGrounded ? force : force / 2.0f;
            rigidBody2D.AddForce(new Vector2(currentDirection * appliedForce, 0.0f));

            // Cancle out jump if user has stopped jumping
            if (isJumping && jumpKeyReleased)
            {
                rigidBody2D.AddForce(Vector2.down * jumpCancleForce);

                if (rigidBody2D.velocity.y < 0.0f) isJumping = false;
            }

            /*// Lower friction
            if (rigidBody2D.velocity.x > startingSpeed * 2)
            {
                rigidBody2D.sharedMaterial.friction = 0.0f;
            }
            else
            {
                rigidBody2D.sharedMaterial.friction = 0.0f;
            }*/
            
            // Give a boos on low speeds
            if (Math.Abs(rigidBody2D.velocity.x) < startingSpeed && isGrounded && isInForceField == 0)
            {
                float normalizedCurrentDirection;
                if (currentDirection > 0.0001f)
                {
                    normalizedCurrentDirection = 1.0f;
                }
                else if (currentDirection < -0.0001f)
                {
                    normalizedCurrentDirection = -1.0f;
                }
                else
                {
                    normalizedCurrentDirection = 0.0f;
                }
                
                rigidBody2D.velocity = new Vector2(normalizedCurrentDirection * startingSpeed, rigidBody2D.velocity.y);
            }
        }
        else
        {
            rotPrevAngle = rotAngle;

            // We don't use speed
            rigidBody2D.velocity = Vector2.zero;

            // Add gravity
            rotVelocity += Physics2D.gravity.y * Mathf.Cos(rotAngle) * Time.fixedDeltaTime;
            // Add drag
            rotVelocity = rotVelocity * ( 1 - Time.fixedDeltaTime * rigidBody2D.drag);

            if (isTouching)
            {
                //rotVelocity = 0.0f;
                isTouching = false;
            }
            else
            {
                // Update rotAngle
                rotAngle += rotVelocity * Time.fixedDeltaTime / rotDistance;
            }

            // Set position
            Vector3 circlePosition = new Vector3(rotDistance * Mathf.Cos(rotAngle), rotDistance * Mathf.Sin(rotAngle), 0.0f);
            rigidBody2D.transform.position = rotPosition + circlePosition;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isRotating && other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("Col Enter");
            rotCollider = other;
            
            UndoRotation();
        }

        if (other.CompareTag("Hazard")) Kill();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isRotating && !isTouching && other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("Col Stay");
            rotCollider = other;

            UndoRotation();
        }
    }

    private void UndoRotation()
    {
        isTouching = true;

        // Update rotAngle, by removing the last move
        rotAngle = rotPrevAngle;//rotAngle -= rotVelocity * Time.fixedDeltaTime / rotDistance;

        // Set position
        Vector3 circlePosition = new Vector3(rotDistance * Mathf.Cos(rotAngle), rotDistance * Mathf.Sin(rotAngle), 0.0f);
        rigidBody2D.transform.position = rotPosition + circlePosition;

        //Remove speed
        rotVelocity = 0.0f;
    }

    private void Kill()
    {
        if (isDead) return;

        isDead = true;
        
        var hordeManager = FindObjectOfType<HordeManager>();
        if (hordeManager != null)
        {
            hordeManager.KillAll();
        }

        AudioManager.Instance.PlaySound("death");
        rigidBody2D.velocity = Vector2.zero;
        rigidBody2D.isKinematic = true;
        enabled = false;
        OnPlayerDeath?.Invoke(this, null);
        Game.RestartLevel(this);
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        toggleMagnetAction.Enable();
        cheatAction.Enable();
        resetAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        toggleMagnetAction.Disable();
        cheatAction.Disable();
        resetAction.Disable();
    }
}
