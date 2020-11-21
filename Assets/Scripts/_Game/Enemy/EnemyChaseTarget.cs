using UnityEngine;
using UnityEngine.AI;

namespace Jampacked.ProjectInca
{
	[RequireComponent(typeof(EnemyController), typeof(NavMeshAgent))]
	public class EnemyChaseTarget : MonoBehaviour
	{
		[SerializeField]
		bool canMoveWhileAttacking = false;
		
		EnemyController m_enemyController;

		NavMeshAgent m_navMeshAgent;
		NavMeshPath  m_navPath;

		void Awake()
		{
			m_enemyController = GetComponent<EnemyController>();
			m_navMeshAgent    = GetComponent<NavMeshAgent>();

			m_navPath = new NavMeshPath();
		}

        void Update()
        {
            if (m_enemyController.CurrentState == EnemyController.State.Attack)
            {
                if (!canMoveWhileAttacking)
                {
                    m_navMeshAgent.ResetPath();
                    return;
                }
            } else if (m_navMeshAgent.remainingDistance <= m_navMeshAgent.stoppingDistance)
            {
                m_enemyController.CurrentState = EnemyController.State.Idle;
            } else
            {
                m_enemyController.CurrentState = EnemyController.State.Chase;
            }

            SetNavPathToTarget();
        }

        void SetNavPathToTarget()
		{
			Vector3 targetPosition = m_enemyController.CurrentTarget.position;

			////find nearest point on nav mesh from target's current position
			//if (NavMesh.SamplePosition(targetPosition, out var navHit, 10f, m_navMeshAgent.areaMask))
			//{
			//	targetPosition = navHit.position;
			//}

			//only use new destination if it's reachable
			if (IsPathValid(targetPosition))
			{
				m_navMeshAgent.SetPath(m_navPath);
			}
		}
		bool IsPathValid(Vector3 a_targetPosition)
		{
			m_navMeshAgent.CalculatePath(a_targetPosition, m_navPath);

			if (m_navPath.status == NavMeshPathStatus.PathComplete)
			{
				return true;
			}

			return false;
		}
	}
}
