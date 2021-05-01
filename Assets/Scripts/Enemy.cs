using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float health = 2.0f;

    private GameObject target;

    private void Update()
    {
        if (target)
        {
            transform.LookAt(target.transform);
            // TODO: try shooting at the target
            // TODO: try moving toward the target (need pathfinding of some sort)
        }
        else
        {
            // TODO: if no target, resume previous activity
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Set aggro target when they come within radius.
        if (other.CompareTag("Player"))
        {
            target = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Unset target if they move outside aggro radius.
        if (other.gameObject == target) target = null;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
