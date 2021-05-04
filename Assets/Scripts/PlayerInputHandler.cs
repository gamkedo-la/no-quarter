﻿using System.Collections;
using System.Collections.Generic;
// using Unity.MPE;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(CharacterController))]
public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10f;
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
    private bool isFiring = false; // toggled by PlayerInput events

    [SerializeField]
    private List<WeaponMod> equippedMods;

    private FPSPlayerControls controls;
    private FPSPlayerControls.PlayerActions playerActions;

    private float fireInput;
    private Vector2 lookDelta = Vector2.zero;
    private Vector2 moveDelta = Vector2.zero;

    private void Awake()
    {
        controls = new FPSPlayerControls();

        // Fire controls
        playerActions = controls.Player;
        playerActions.Fire.performed += ctx => { isFiring = true; };
        playerActions.Fire.canceled += ctx => { isFiring = false; };

        // Camera controls
        playerActions.Look.performed += ctx => lookDelta += ctx.ReadValue<Vector2>();
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
        // Vector3 moveX = Input.GetAxis("Horizontal") * transform.right;
        // Vector3 moveZ = Input.GetAxis("Vertical") * transform.forward;
        // characterController.Move(moveSpeed * Time.deltaTime * (moveX + moveZ).normalized);

        if (isFiring && canFire)
        {
            Fire(playerCamera.forward);
        }
    }

    void FixedUpdate()
    {
        Look(Time.fixedDeltaTime);
    }

    void Look(float deltaTime)
    {
        // Rotate character transform around y (turn left & right).
        float yRotation = lookDelta.x * lookSpeed * deltaTime;
        transform.Rotate(Vector3.up, yRotation);

        // Rotate camera round x (look up & down)
        float cameraRotationDirection = invertCameraRotation ? 1 : -1;
        float xRotation = lookDelta.y * lookSpeed * cameraRotationDirection * deltaTime;
        cameraRotation = Mathf.Clamp(cameraRotation + xRotation, cameraMinAngle, cameraMaxAngle);
        playerCamera.localRotation = Quaternion.Euler(cameraRotation, 0, 0);

        // Zero out the input after use.
        lookDelta = Vector2.zero;
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
            var projectileInstance = Instantiate(projectile, firePosition.position, Quaternion.identity);
            projectileInstance.GetComponent<Projectile>().projectileDirection = dir;
        }
    }

    private IEnumerator Cooldown(float duration)
    {
        canFire = false;
        yield return new WaitForSeconds(duration);
        canFire = true;
    }
}
