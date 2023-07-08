using DG.Tweening;
using UnityEngine;

namespace Gameplay
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private Vector3 m_positionFromPlayer = new Vector3(0,11.41f, -7.31f);
        [SerializeField] private float m_speedFollow = 0.5f;
        [SerializeField] private float m_speedFollowInDash = 0.5f;
        [SerializeField] private Ease EaseFollow = Ease.OutQuad;
        private Vector3 distanceFromPlayer = new();
        private bool isInDash = false;
        
        // Update is called once per frame
        void Update() {
            if (isInDash) {
                transform.DOMove(PlayerController.instance.transform.position + m_positionFromPlayer, m_speedFollowInDash).SetEase(EaseFollow);
            }
            else {
                distanceFromPlayer = transform.position - PlayerController.instance.transform.position;
                transform.DOMove(PlayerController.instance.transform.position + m_positionFromPlayer, m_speedFollow).SetEase(EaseFollow);
            }
        }

        public void ChangeDashState(bool isInDash) => this.isInDash = isInDash;
    }
}
