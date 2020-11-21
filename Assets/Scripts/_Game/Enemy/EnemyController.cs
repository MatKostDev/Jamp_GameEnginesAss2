using UnityEngine;
using UnityEngine.Events;
using Jampacked.ProjectInca.Events;

namespace Jampacked.ProjectInca
{
	[RequireComponent(typeof(Health), typeof(Animator))]
	public class EnemyController : MonoBehaviour
	{
		public enum State
		{
			Idle,
			Chase,
			Attack,
		}
		
		[Header("General")]
		[Tooltip("Delay after death before the GameObject is destroyed to allow for animation")]
		[SerializeField]
		float deathDuration = 0f;
		
		[SerializeField]
		float rotateSpeed = 10f;

		[Header("Visual Effects")]
		[SerializeField]
		GameObject deathVisualEffect = null;

		//[Header("Audio")]
		//[SerializeField]
		//AudioClip moveSound;

		//[SerializeField]
		//AudioClip hitSound;
		
		//[SerializeField]
		//AudioClip dieSound;

		public UnityAction onAttack;
		
		const float DURATION_BEFORE_DESTROYING_VFX = 3f;

		protected EventDispatcher m_dispatcher;

		Transform m_transform;

		ColorFlashBehaviour m_colorFlash;
		EnemyAttack         m_attack;
		Health              m_health;
		Animator            m_animator;
		LootDropper         m_lootDropper;
		
		bool m_tookDamageThisFrame;
		
		Transform m_currentTarget;
		public Transform CurrentTarget
		{
			get { return m_currentTarget; }
		}
		
		State m_currentState;
		public State CurrentState
		{
			get { return m_currentState; }
			set
			{
				if (m_currentState == value)
				{
					return;
				}

				//state transition stuff here
				switch (value)
				{
					case State.Idle:
					{
						m_animator.Play("Armature_Static T-pose");
						break;
					} 
					case State.Chase:
					{
						m_animator.Play("Armature_Running");
						break;
					}
					case State.Attack:
					{
						m_animator.Play("Armature_BitchSlap");
						break;
					}
				}

				m_currentState = value;
			}
		}

		void Start()
		{
			m_dispatcher = GameObject.Find("GlobalEventDispatcher").GetComponent<EventDispatcher>();
			
			m_transform   = transform;
			m_colorFlash  = GetComponent<ColorFlashBehaviour>();
			m_attack      = GetComponent<EnemyAttack>();
			m_health      = GetComponent<Health>();
			m_animator    = GetComponent<Animator>();
			m_lootDropper = GetComponent<LootDropper>();

			m_health.onDamaged += OnDamaged;
			m_health.onDie     += OnDie;
			
			SetTargetToClosest();

			CurrentState = State.Idle;
		}

		void Update()
		{
			m_tookDamageThisFrame = false;

			if (IsTargetInAttackRange() && m_attack.CanAttack() && CurrentState != State.Attack)
			{
                onAttack.Invoke();
                CurrentState = State.Attack;
            }
		}

		public void OnAttackFinished()
		{
			CurrentState = State.Idle;
		}
		
        public void RotateTowardsTarget()
        {
            Vector3 lookDirection = Vector3.ProjectOnPlane(m_currentTarget.transform.position - m_transform.position, Vector3.up).normalized;
            if (lookDirection.sqrMagnitude != 0f)
            {
	            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
	            
                m_transform.rotation = Quaternion.Slerp(
	                m_transform.rotation,
	                targetRotation,
	                Time.deltaTime * rotateSpeed
                );
            }
        }

		bool IsTargetInAttackRange()
		{
			if (!m_attack)
			{
				return false;
			}
			
			return Vector3.Distance(m_transform.position, m_currentTarget.position) <= m_attack.AttackRange;
		}

        void OnDamaged(float a_damageAmount)
		{
			if (m_tookDamageThisFrame)
			{
				return;
			}
			
			m_colorFlash.StartColorFlash();

			m_tookDamageThisFrame = true;
		}

		void OnDie()
		{
			if (deathVisualEffect)
			{
				var deathEffectObject = Instantiate(deathVisualEffect, m_transform.position, Quaternion.identity);
				Destroy(deathEffectObject, DURATION_BEFORE_DESTROYING_VFX);
			}
			
			if (m_lootDropper)
			{
				m_lootDropper.SpawnLoot();
			}

			var killedEvent = new EnemyKilledEvent()
			{
				enemyObjectId = gameObject.GetInstanceID(),
			};
			m_dispatcher.PostEvent(killedEvent);

            var tutorialKilledEvent = new TutorialEnemyKilledEvent()
            {
                enemyObjectId = gameObject.GetInstanceID(),
            };
            m_dispatcher.PostEvent(tutorialKilledEvent);

			Destroy(gameObject, deathDuration);
		}

		void SetTargetToClosest()
		{
			Transform targetPlayerTransform = FindObjectOfType<PlayerController>().transform;
			
			m_currentTarget = targetPlayerTransform;
		}
	}
}
