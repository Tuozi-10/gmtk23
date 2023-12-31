﻿using System;
using Managers;
using UnityEngine;

namespace IAs
{
    public class HitTrigger : MonoBehaviour
    {
        private AI ai;
        
        private void Awake()
        {
            ai = transform.parent.GetComponent<AI>();
        }

        public void Hit()
        {
            ai.HitTarget();
        }

        public void Step()
        {
            AudioManager.PlaySoundMobStep();
        }
    }
}