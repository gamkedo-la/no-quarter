﻿using System;
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

    [Header("Dash")]
    [SerializeField] private float dashDistance = 1f;
    [SerializeField] private float dashTime = 0.333f;
    [SerializeField] private AnimationCurve dashSmoothingCurve;
    [SerializeField] private AnimationCurve dashFovCurve;
    [SerializeField] private AudioClip dashSFX;
    [Range(0f, 1f)]
    [SerializeField] private float dashSFXVolume = 1f;

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
    private FPSWeapon activeWeapon;
    private WeaponSwitch weaponSwitcher;
    [SerializeField]
    private Transform firePosition;
    [SerializeField]
    private float fireDelay = 0.3f;
    private bool canFire = true;

    private bool canDash = true;
    public delegate void DashStarted();
    public static event DashStarted dashStarted;

    public List<WeaponMod> equippedMods;

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

    public float footstepPace = 0.333f;
    public List<AudioClip> footstepSFX = new List<AudioClip>();

    private SfxHelper sfx;

    private bool isSwitchingWeapon = false;
    [SerializeField] private float weaponSwitchDuration = 0.3f;
    public delegate void WeaponScroll(int scrollDirection);
    public static event WeaponScroll OnWeaponScroll;

    public delegate void Pause();
    public static event Pause OnPause;

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
        playerActions.Dash.performed += ctx =>
        {
            if (canDash){
                 StartCoroutine(Dash());
            }
        };
        controls.UI.Pause.performed += ctx =>
        {
            TogglePause();
        };

        // Swap Weapon Controls
        playerActions.ScrollWeapon.performed += ctx => {
            if (!isSwitchingWeapon){
                StartCoroutine(SwitchingWeaponDelay(weaponSwitchDuration));
                OnWeaponScroll?.Invoke(Mathf.RoundToInt(ctx.ReadValue<Vector2>().y));
                activeWeapon = weaponSwitcher.GetActiveWeapon()?.GetComponent<FPSWeapon>();
            }
        };
    }

    private void OnEnable()
    {
        controls.Enable();
        PlayerStatsManager.OnStaminaChange += updateCanDash;
    }

    private void OnDisable() {
        PlayerStatsManager.OnStaminaChange += updateCanDash;
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
        weaponSwitcher = GetComponentInChildren<WeaponSwitch>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(PlayFootsteps());
    }

    // Update is called once per frame
    void Update()
    {
        if (canTeleport)
            Look(Time.deltaTime);
        
        if (isFiring && canFire)
        {
            Fire(playerCamera.forward);
        }
        
        if (isMoving && canTeleport)
        {
            Move(Time.deltaTime);
        }
        if(canTeleport)
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

    void Fire(Vector3 fireDirection)
    {
        StartCoroutine(Cooldown(activeWeapon.GetFireDelay()));
        activeWeapon.Fire(fireDirection, playerCamera);
        OnFire.Invoke();
        sfx.PlayRandomAudioOneshot(activeWeapon.gunshotSFX, 0.7f, 1f, 0.9f, 1.1f);
    }

    private void Jump()
    {
        if (characterController.isGrounded && playerJumpVelocity.y < 0)
        {
            playerJumpVelocity.y = 0f;
        }
        if (characterController.isGrounded && isJumping)
        {
            // characterController.height
            float jumpHeight = characterController.height;
            // Debug.Log(capsuleCollider.bounds.size.y);
            playerJumpVelocity.y += Mathf.Sqrt(jumpHeight *  jumpHeight * -gravityValue);
            isJumping = false;
        }
        playerJumpVelocity.y += gravityValue * Time.deltaTime;
        characterController.Move(playerJumpVelocity * Time.deltaTime);
    }

    private IEnumerator Dash()
    {
        playerActions.Disable();
        dashStarted?.Invoke();

        var mainCam = Camera.main;
        var baseFOV = mainCam.fieldOfView;

        var time = 0f;
        var dashDirection = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        var dashMovement = dashDirection * dashDistance;
        var origin = transform.position;
        var destination = transform.position + dashMovement;
        
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, dashDistance))
        {
            destination = hitInfo.point;
        }

        AudioManager.Instance.PlaySFX(dashSFX, gameObject, dashSFXVolume, 1f, 0f);

        while (time < dashTime)
        {
            var curTime = time / dashTime;
            var t = dashSmoothingCurve.Evaluate(curTime);
            var between = Vector3.Lerp(origin, destination, t);
            var diff = between - transform.position;
            characterController.Move(diff);
            
            var fovMulti = dashFovCurve.Evaluate(curTime);
            mainCam.fieldOfView = baseFOV * fovMulti;
            
            time += Time.deltaTime;
            yield return null;
        }

        mainCam.fieldOfView = baseFOV;
        playerActions.Enable();
    }

    private IEnumerator Cooldown(float duration)
    {
        canFire = false;
        yield return new WaitForSeconds(duration);
        canFire = true;
    }

    private IEnumerator SwitchingWeaponDelay(float duration) {
        //TODO: disable Player's ability to shoot while weapons are switching
        isSwitchingWeapon = true;
        yield return new WaitForSeconds(duration);
        isSwitchingWeapon = false;
    }

    private void updateCanDash(int currentCharges){
        if (currentCharges == 0)
            canDash = false;
        else if (!canDash) 
            canDash = true;
    }

    public void TogglePause() {
        if(Time.timeScale == 0){
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            OnPause?.Invoke();
            playerActions.Enable();
        } else {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            OnPause?.Invoke();
            playerActions.Disable();
        }
    }

    public void SetFireDelay(float delay){
        fireDelay = delay;
    }
}
