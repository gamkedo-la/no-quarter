
using UnityEngine;
using System.Collections.Generic; // for list wtf

public class ForceZone : MonoBehaviour
{
 
   public Vector3 ForceVector;
   public List<GameObject> AffectedObjects;
 
    void OnTriggerEnter(Collider collidee)
    {
          AffectedObjects.Add(collidee.gameObject);
    }
 
    void OnTriggerExit(Collider collidee)
    {
       AffectedObjects.Remove(collidee.gameObject);
    }
 
    void FixedUpdate()
    {
       for(int I =0; I < AffectedObjects.Count; I++)
       {
          Rigidbody rb = AffectedObjects[I].GetComponent<Rigidbody>();
          // sadly the player does not have a rigid body
          // it doesn't use physics for movement (which is great!)
          if (rb) {
              Debug.Log("Force Zone is pushing "+AffectedObjects[I].name);
              rb.AddForce(ForceVector);
          } else {
            // danger: this pushes "anything"
            // a lot!!! this value needs to be tiny =)
            Debug.Log("Force Zone is nudging "+AffectedObjects[I].name);
            AffectedObjects[I].transform.position = 
                AffectedObjects[I].transform.position + ForceVector;
          }
       }
    }
}
