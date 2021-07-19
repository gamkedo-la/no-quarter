using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowIfClose : MonoBehaviour
{
    public GameObject showGO;
    public Transform obj1;
    public Transform obj2;
    public float tooFarToShow = 5.0f;
    public float tooCloseToShow = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        showGO.SetActive(false);            
    }

    // Update is called once per frame
    void Update()
    {
        float distNow = Vector3.Distance(obj1.position, obj2.position);
        bool showIt = distNow < tooFarToShow && distNow > tooCloseToShow;
        if (showIt != showGO.activeSelf) {
            showGO.SetActive(showIt);
        }
    }
}
