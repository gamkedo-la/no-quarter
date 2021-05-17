using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(CharacterController))]
public class PlayerInputHandler : TeleportAgent
{
    public float moveSpeed = 10f;
    [SerializeField]
    private float lookSpeed = 3f;

    private CharacterController characterController;
    private Transform playerCamera;

    [SerializeField]
    private float cameraMaxAngle = 60f;
    [SerializeField]
    private float cameraMinAngle = -60f;
    private float cameraRotation = 0f;
    [SerializeField]
    private bool invertCameraRotation = false;

    [SerializeField]
    private Projectile projectile;
    [SerializeField]
    private Transform firePosition;
    [SerializeField]
    private float fireDelay = 0.3f;
    private bool canFire = true;

    [SerializeField]
    private List<WeaponMod> equippedMods;

    private FPSPlayerControls controls;
    private FPSPlayerControls.PlayerActions playerActions;

    // Toggled by PlayerInput events
    private bool isFiring = false;
    public bool isMoving = false;
    private bool isJumping = false;

    [SerializeField]
    private CapsuleCollider capsuleCollider;
    private Vector3 playerJumpVelocity = Vector3.zero;
    private float gravityValue = -9.81f;

    public UnityEvent OnFire;

	public List<AudioClip> gunshotSFX = new List<AudioClip>();
    public float footstepPace = 0.333f;
    public List<AudioClip> footstepSFX = new List<AudioClip>();

    private SfxHelper sfx;

    private void Awake()
    {
        controls = new FPSPlayerControls();

        // Fire controls
        playerActions = controls.Player;
        playerActions.Fire.performed += ctx => { isFiring = true; };
        playerActions.Fire.canceled += ctx => { isFiring = false; };

        // Movement controls
        playerActions.Move.performed += ctx =>
        {
            isMoving = true;
        };
        playerActions.Move.canceled += ctx =>
        {
            isMoving = false;
        };
        playerActions.Jump.performed += ctx =>
        {
            isJumping = true;
        };
        playerActions.Jump.canceled += ctx =>
        {
            isJumping = false;
        };
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDestroy()
    {
        controls.Disable();
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>().transform;
        sfx = GetComponent<SfxHelper>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(PlayFootsteps());
    }

    // Update is called once per frame
    void Update()
    {
        Look(Time.deltaTime);
        
        if (isFiring && canFire)
        {
            Fire(playerCamera.forward);
        }
        
        if (isMoving)
        {
            Move(Time.deltaTime);
        }
        Jump();
    }

    void Look(float deltaTime)
    {
        // Rotate character transform around y (turn left & right).
        var lookDelta = playerActions.Look.ReadValue<Vector2>();
        float yRotation = lookDelta.x * lookSpeed;
        transform.Rotate(Vector3.up, yRotation);

        // Rotate camera round x (look up & down)
        float cameraRotationDirection = invertCameraRotation ? 1 : -1;
        float xRotation = lookDelta.y * lookSpeed * cameraRotationDirection;
        cameraRotation = Mathf.Clamp(cameraRotation + xRotation, cameraMinAngle, cameraMaxAngle);
        playerCamera.localRotation = Quaternion.Euler(cameraRotation, 0, 0);
    }

    IEnumerator PlayFootsteps()
    {
        while (true)
        {
            if (isMoving)
            {
                sfx.PlayRandomAudioOneshot(footstepSFX, 0.7f, 1.0f, 0.9f, 1.1f);
            }
            yield return new WaitForSeconds(footstepPace);
        }
    }

    void Move(float deltaTime)
    {
        var movement = playerActions.Move.ReadValue<Vector2>();
        var moveX = movement.x * transform.right;
        var moveZ = movement.y * transform.forward;
        characterController.Move(moveSpeed * deltaTime * (moveX + moveZ).normalized);
    }

    int GetProjectileCount()
    {
        var projectileCount = projectile.baseProjectileCount;

        foreach (var mod in equippedMods)
        {
            projectileCount += mod.additionalProjectiles;
        }

        return projectileCount;
    }

    float GetFireDelay()
    {
        var delay = fireDelay;
        
        foreach (var mod in equippedMods)
        {
            delay *= mod.fireDelayMultiplier;
        }

        return delay;
    }

    List<Vector3> GetProjectileDirections(Vector3 originalDirection)
    {
        var numProjectiles = GetProjectileCount();
        List<Vector3> directions = new List<Vector3>();

        if (numProjectiles > 1)
        {
            for (var i = 0; i < numProjectiles; i++)
            {
                Vector3 spread = UnityEngine.Random.insideUnitCircle * 0.1f;
                var skewedDirection = originalDirection + (spread.x * playerCamera.right) + (spread.y * playerCamera.up);
                directions.Add(skewedDirection);
            }
        }
        else
        {
            directions.Add(originalDirection);
        }

        return directions;
    }

    void Fire(Vector3 fireDirection)
    {
        StartCoroutine(Cooldown(GetFireDelay()));

        var fireDirections = GetProjectileDirections(fireDirection);

        foreach (var dir in fireDirections)
        {
            var projectileInstance = Instantiate(projectile, firePosition.position, playerCamera.rotation);
            projectileInstance.GetComponent<Projectile>().projectileDirection = dir;
        }
        
        OnFire.Invoke();

        sfx.PlayRandomAudioOneshot(gunshotSFX, 0.7f, 1f, 0.9f, 1.1f);
    }

    private void Jump()
    {
        if (characterController.isGrounded && playerJumpVelocity.y < 0)
        {
            playerJumpVelocity.y = 0f;
        }
        if (characterController.isGrounded && isJumping)
        {
            float jumpHeight = capsuleCollider.bounds.size.y;
            Debug.Log(capsuleCollider.bounds.size.y);
            playerJumpVelocity.y += Mathf.Sqrt(jumpHeight *  jumpHeight * -gravityValue);
            isJumping = false;
        }
        playerJumpVelocity.y += gravityValue * Time.deltaTime;
        characterController.Move(playerJumpVelocity * Time.deltaTime);
    }

    private IEnumerator Cooldown(float duration)
    {
        canFire = false;
        yield return new WaitForSeconds(duration);
        canFire = true;
    }
}
