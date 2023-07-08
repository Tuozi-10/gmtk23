using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTest : MonoBehaviour
{
    [SerializeField] private float speed,alignmentspeed;
    public Transform target;
    
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        Vector3 rot =  target.transform.position - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(new Vector3(rot.x,0,rot.z)), Time.deltaTime * alignmentspeed);
    }
}
