using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerWeaponManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform weaponParentSocket;

    [Header("Weapon Bob")]
    [SerializeField] private float bobFrequency = 10f;
    [SerializeField] private float bobSharpness = 10f;
    [SerializeField] private float defaultBobAmount = 0.05f;
    [SerializeField] private float aimingBobAmount = 0.05f;

    private bool isAiming;

    private float weaponBobFactor;
    private Vector3 weaponDefaultPosition;
    private Vector3 weaponBobOffset;
    private CharacterController characterController;
    private PlayerInputHandler inputHandler;

    void Start()
    {
        weaponDefaultPosition = weaponParentSocket.localPosition;
        characterController = GetComponent<CharacterController>();
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    void LateUpdate()
    {
        UpdateWeaponBob();

        weaponParentSocket.localPosition = weaponDefaultPosition + weaponBobOffset;
    }

    private void UpdateWeaponBob()
    {
        if (Time.deltaTime > 0f)
        {
            var movementFactor = inputHandler.isMoving ? 1 : 0;

            weaponBobFactor = Mathf.Lerp(weaponBobFactor, movementFactor, bobSharpness * Time.deltaTime);

            var amount = isAiming ? aimingBobAmount : defaultBobAmount;
            var frequency = bobFrequency;
            var hBobValue = Mathf.Sin(Time.time * frequency) * amount * weaponBobFactor;
            var vBobValue = ((Mathf.Sin(Time.time * frequency * 2f) * 0.5f) + 0.5f) * amount * weaponBobFactor;

            weaponBobOffset.x = hBobValue;
            weaponBobOffset.y = Mathf.Abs(vBobValue);
        }
    }
}