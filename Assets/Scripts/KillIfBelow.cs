using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillIfBelow : MonoBehaviour
{
    public Transform heightIndicator;


    // Start is called before the first frame update
    void Start()
    {
        if(heightIndicator == null)
        {
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < heightIndicator.position.y)
        {
            gameObject.SendMessage("TakeDamage", 5000);
        }
    }
}
