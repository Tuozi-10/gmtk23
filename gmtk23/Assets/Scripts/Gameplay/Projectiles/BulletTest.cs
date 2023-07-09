using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using IAs;
using Managers;
using UnityEngine;

public class BulletTest : MonoBehaviour
{
    [SerializeField] private float speed,alignmentspeed;
    public AI target;

    public int damages;
    
    void Update()
    {
        if (target == null || damages<0 && target.FullLife)
        {
            if(damages> 0) FxManagers.RequestDamageFxAtPos(transform.position);
           else FxManagers.RequestHealFxAtPos(transform.position);
            Destroy(transform.gameObject);
            return;
        }
        
        transform.position += transform.forward * speed * Time.deltaTime;
        Vector3 rot =  target.transform.position - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(new Vector3(rot.x,0,rot.z)), Time.deltaTime * alignmentspeed);

        if (Vector3.Distance(transform.position, target.transform.position) < 1.25f)
        {
            Impact();
        }
    }

    public void SetUp(AI newTarget, int dmg, bool zone = true)
    {
        damages = dmg;
        target = newTarget;
        m_zone = zone;
    }

    private bool m_zone;
    
    private void Impact()
    {

        if (damages > 0)
        {
            FxManagers.RequestHitShockWaveFxAtPos(transform.position, 1f, target.team, damages);
        }
        else if(m_zone)
        {
            FxManagers.RequestHealShockWaveFxAtPos(transform.position, 1f, target.team, damages);
        }
        
        AudioManager.PlaySoundFireBallExplosion();
        target.Hit(damages, true);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.GetComponent<PlayerController>() != null && target.team == AI.Team.Hero)
        {
            PlayerController.instance.Hit(damages);
            return;
        }
        
        if (other.GetComponent<AI>())
        {
            var otherAI =other.GetComponent<AI>();
            if (otherAI.team == target.team && target != otherAI)
            {
                other.GetComponent<AI>().Hit(damages, true);
            }
        }
    }
}
