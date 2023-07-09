using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using IAs;
using UnityEngine;

public class ShockWave : MonoBehaviour
{
    public AI.Team target;

    public int damages;

    private SphereCollider m_sphereCollider;
    
    private void Awake()
    {
        m_sphereCollider = GetComponent<SphereCollider>();
    }

    public void SetUp(AI.Team newTarget, int dmg )
    {
        damages = dmg;
        target = newTarget;
        StartCoroutine(DisableCollider());
    }

    IEnumerator DisableCollider()
    {
        m_sphereCollider.enabled = true;
        yield return new WaitForSeconds(.2f);
      m_sphereCollider.enabled = false;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            if (target == AI.Team.Hero)
            {
                PlayerController.instance.Hit(damages);
            }

            return;
        }
        
        if (other.GetComponent<AI>())
        {
            var otherAI =other.GetComponent<AI>();
            if (otherAI.team == target)
            {
                other.GetComponent<AI>().Hit(damages, true);
            }
        }
    }
}
