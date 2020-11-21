using System.Collections;
using UnityEngine;

namespace Jampacked.ProjectInca
{
	public class MeleeAttack : EnemyAttack
	{
		[SerializeField]
		float damage = 5f;

		[SerializeField]
		float windUpDuration = 0.1f;

		[SerializeField]
		float attackActiveDuration = 0.5f;
		
		[SerializeField]
		MeleeHitbox[] hitboxes = new MeleeHitbox[1];

		bool m_didHit;
		
		protected override void Start()
		{
			base.Start();

			SetHitboxesActive(false);
		}

		public void OnHit(int a_playerObjectId)
		{
			if (m_didHit)
			{
				return;
			}
			
			m_didHit = true;

			//send damage event for player
			var damageEvent = new DamagePlayerEvent
			{
                playerObjectId = a_playerObjectId,
                damageAmount   = damage,
            };
			m_dispatcher.PostEvent(damageEvent);

			SetHitboxesActive(false);
		}

		protected override void StartAttack()
		{
			m_didHit = false;

			StartCoroutine(AttackRoutine());
		}

		IEnumerator AttackRoutine()
		{
			yield return new WaitForSeconds(windUpDuration);
			
			SetHitboxesActive(true);
			
			yield return new WaitForSeconds(attackActiveDuration);
			
			SetHitboxesActive(false);
			m_lastAttackTime = Time.time;
			m_enemyController.OnAttackFinished();
		}

		void SetHitboxesActive(bool a_setToActive)
		{
			foreach (var hitbox in hitboxes)
			{
				hitbox.gameObject.SetActive(a_setToActive);
			}
		}
	}
}
