using UnityEngine;

namespace src.UnityComponents
{
    public class TurnEverytime : MonoBehaviour
    {
        [SerializeField] private Vector3 m_rotation = Vector3.zero;
        
        private void FixedUpdate()
        {
            transform.Rotate(m_rotation);
        }
    }
}