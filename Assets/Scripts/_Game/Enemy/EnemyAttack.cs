using UnityEngine;
using Jampacked.ProjectInca.Events;

namespace Jampacked.ProjectInca
{
	[RequireComponent(typeof(EnemyController))]
	public abstract class EnemyAttack : MonoBehaviour
	{
		[Tooltip("The minimum range from the target where the enemy is allowed to start an attack")]
		[SerializeField]
		protected float attackRange = 2f;
		
		[SerializeField]
		protected float cooldownDuration = 0.3f;

		protected EnemyController m_enemyController = null;

		protected float m_lastAttackTime = Mathf.NegativeInfinity;

		protected EventDispatcher m_dispatcher;

		public float AttackRange
		{
			get { return attackRange; }
		}

		protected virtual void Start()
		{
			m_dispatcher = GameObject.Find("GlobalEventDispatcher").GetComponent<EventDispatcher>();

			m_enemyController = GetComponent<EnemyController>();

			m_enemyController.onAttack += StartAttack;
		}

		public bool CanAttack()
		{
			return m_lastAttackTime + cooldownDuration < Time.time;
		}

		protected abstract void StartAttack();
	}
}
