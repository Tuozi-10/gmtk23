using System.Collections;
using System.Collections.Generic;
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
        if (target == null)
        {
            FxManagers.RequestDamageFxAtPos(transform.position);
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

    public void SetUp(AI newTarget, int dmg )
    {
        damages = dmg;
        target = newTarget;
    }
    
    private void Impact()
    {
        target.Hit(damages, true);
        Destroy(gameObject);
    }
}
