using UnityEngine;

namespace IAs
{
    public class boss : AI
    {
        protected override void DoAttacking()
        {
            if (Time.time - m_lastAttack > attackSpeedRate)
            {
                m_lastAttack = Time.time;
                m_animator.Play("AttackDistance");
            }

            if (m_targetAI == null || (m_targetAI.FullLife && isHealer))
            {
                m_currentState = States.Wander;
                return;
            }
            
            var distanceTarget = Vector3.Distance(transform.position, m_targetAI.transform.position);

            if (distanceTarget > (m_distanceAttackDistance) + 0.5f)
            {
                m_currentState = States.Chasing;
                return;
            }

            agent.SetDestination(transform.position);
        }
    }
}