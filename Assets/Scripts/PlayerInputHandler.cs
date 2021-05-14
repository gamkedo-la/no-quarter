using System.Collections;
using System.Collections.Generic;
// using Unity.MPE;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

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

    public UnityEvent OnFire;

	public List<AudioClip> gunshotSFX = new List<AudioClip>();

    private void Awake()
    {
        controls = new FPSPlayerControls();

        // Fire controls
        playerActions = controls.Player;
        playerActions.Fire.performed += ctx => { isFiring = true; };
        playerActions.Fire.canceled += ctx => { isFiring = false; };

        // Movement controls
        playerActions.Move.performed += ctx =>  isMoving = true;
        playerActions.Move.canceled += ctx =>  isMoving = false;
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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFiring && canFire)
        {
            Fire(playerCamera.forward);
        }
    }

    void FixedUpdate()
    {
        Look(Time.fixedDeltaTime);

        if (isMoving)
        {
            Move(Time.fixedDeltaTime);
        }
    }

    void Look(float deltaTime)
    {
        // Rotate character transform around y (turn left & right).
        var lookDelta = playerActions.Look.ReadValue<Vector2>();
        float yRotation = lookDelta.x * lookSpeed * deltaTime;
        transform.Rotate(Vector3.up, yRotation);

        // Rotate camera round x (look up & down)
        float cameraRotationDirection = invertCameraRotation ? 1 : -1;
        float xRotation = lookDelta.y * lookSpeed * cameraRotationDirection * deltaTime;
        cameraRotation = Mathf.Clamp(cameraRotation + xRotation, cameraMinAngle, cameraMaxAngle);
        playerCamera.localRotation = Quaternion.Euler(cameraRotation, 0, 0);
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
                Vector3 spread = Random.insideUnitCircle * 0.1f;
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

		if (gunshotSFX.Count > 0) {
			AudioManager.Instance.PlaySFX(gunshotSFX[Random.Range(0, gunshotSFX.Count)], gameObject, Random.Range(0.7f, 1f), Random.Range(0.9f, 1.1f), 0f);
		}
    }

    private IEnumerator Cooldown(float duration)
    {
        canFire = false;
        yield return new WaitForSeconds(duration);
        canFire = true;
    }
}
