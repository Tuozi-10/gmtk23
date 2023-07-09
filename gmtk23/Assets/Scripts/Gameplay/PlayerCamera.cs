using System;
using DG.Tweening;
using UnityEngine;

namespace Gameplay
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private Vector3 m_positionFromPlayer = new Vector3(0,11.41f, -7.31f);
        [SerializeField] private float m_speedFollow = 0.5f;
        [SerializeField] private float m_speedFollowInDash = 0.5f;
        [Space]
        [SerializeField] private float slowMoFOV = 70;
        [SerializeField] private float zoomFOV = .3f;
        private bool isInDash = false;
        private float baseFOV = 60f;
        private float targetFOV = 60f;

        private void Start() {
            baseFOV = GetComponent<Camera>().fieldOfView;
        }
        
        void Update() {
            if (PlayerController.instance != null) transform.DOMove(PlayerController.instance.transform.position + m_positionFromPlayer, isInDash ? m_speedFollowInDash : m_speedFollow).SetEase(Ease.Linear);
            GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, targetFOV, zoomFOV);
        }

        /// <summary>
        /// Change the current state of the dash
        /// </summary>
        /// <param name="isInDash"></param>
        public void ChangeDashState(bool isInDash) => this.isInDash = isInDash;

        /// <summary>
        /// Change the current value of the field of the view when in slow motion
        /// </summary>
        /// <param name="isSlowMo"></param>
        public void ChangeSlowMo(bool isSlowMo) => targetFOV = isSlowMo ? slowMoFOV : baseFOV;
    }
}
