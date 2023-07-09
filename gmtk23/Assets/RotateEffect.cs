using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateEffect : MonoBehaviour
{

    public float speed;

    private void Update()
    {
        transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x,transform.localEulerAngles.y+ Time.deltaTime* speed,transform.localEulerAngles.z);
    }
}
