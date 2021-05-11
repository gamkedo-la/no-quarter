using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBob : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private float bobFrequency = 1f;
    [SerializeField] private float bobAmplitude = 2f;
    [SerializeField] private Transform bobTarget;

    private Vector3 startingPosition;
    private float verticalOffset = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if (bobTarget == null)
        {
            bobTarget = transform;
        }

        startingPosition = bobTarget.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        // Spin around vertical axis.
        bobTarget.RotateAround(bobTarget.position, transform.up, rotationSpeed * Time.deltaTime);

        // Bob up and down along vertical axis.
        verticalOffset = (float) Math.Sin(bobFrequency * Time.time) * bobAmplitude;
        bobTarget.localPosition = startingPosition + (transform.up * verticalOffset);
    }
}
