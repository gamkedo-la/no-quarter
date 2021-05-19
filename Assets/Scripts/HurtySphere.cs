using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtySphere : MonoBehaviour
{
    [SerializeField] [Min(0f)] float damageAmount = 50.0f;
    [SerializeField] [Min(0f)] float rechargeTime = 5.0f;
    PlayerStatsManager playerStats;

    Collider m_Collider;

    void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatsManager>();
        m_Collider = GetComponent<Collider>();    
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerStats.TakeDamage(damageAmount);
            m_Collider.enabled = false;
            gameObject.transform.localScale = Vector3.zero;
            StartCoroutine(WaitForRecharge());
        }    
    }

    public IEnumerator WaitForRecharge() {
        yield return new WaitForSeconds (rechargeTime);
        m_Collider.enabled = true;
        gameObject.transform.localScale = Vector3.one;
    }
}
