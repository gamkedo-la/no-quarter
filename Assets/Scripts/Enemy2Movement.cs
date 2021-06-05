using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2Movement : MonoBehaviour
{
    public GameObject Pivot;
    public float zRotate = .5f;

    private void Update()
    {
        float minRotation = -45;
        float maxRotation = 45;
        Vector3 currentRotation = Pivot.transform.localRotation.eulerAngles;
        currentRotation.y = Mathf.Clamp(currentRotation.y, minRotation, maxRotation);
        Pivot.transform.localRotation = Quaternion.Euler(currentRotation);
    }
}
