using DG.Tweening;
using UnityEngine;

namespace Gameplay
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private Vector3 m_positionFromPlayer = new Vector3(0,11.41f, -7.31f);

        [SerializeField] private float m_speedFollow = 0.5f;

        [SerializeField] private Ease EaseFollow = Ease.OutQuad;
        
 
        // Update is called once per frame
        void Update()
        {
            transform.DOMove(PlayerController.instance.transform.position + m_positionFromPlayer,m_speedFollow).SetEase(Ease.OutQuad);
        }
    }
}
