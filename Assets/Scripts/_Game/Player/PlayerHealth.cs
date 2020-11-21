using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jampacked.ProjectInca.Events;
using UnityEngine.UI;

namespace Jampacked.ProjectInca
{
	[RequireComponent(typeof(Health))]
	public class PlayerHealth : MonoBehaviour
	{
		[Header("UI")]
		[SerializeField]
		Image healthBarFill = null;
		
		const float MIN_HEALTH_BAR_FILL_AMOUNT = 0.09f;
		const float MAX_HEALTH_BAR_FILL_AMOUNT = 1f;

		Health        m_health;
		PlayerEffects m_effects;

		EventDispatcher m_dispatcher;
		
		void Start()
		{
			m_dispatcher = GameObject.Find("GlobalEventDispatcher").GetComponent<EventDispatcher>();

			m_health  = GetComponent<Health>();
			m_effects = GetComponent<PlayerEffects>();
			
			m_dispatcher.AddListener<DamagePlayerEvent>(OnDamaged);
			m_dispatcher.AddListener<PickUpHealthEvent>(OnHealthPickUp);
		}
		
		void OnDestroy()
		{
			m_dispatcher.RemoveListener<DamagePlayerEvent>(OnDamaged);
			m_dispatcher.RemoveListener<PickUpHealthEvent>(OnHealthPickUp);
		}

		void OnDamaged(in Events.Event a_evt)
		{
			if (!(a_evt is DamagePlayerEvent damageEvent))
			{
				return;
			}
			if (damageEvent.playerObjectId != gameObject.GetInstanceID())
			{
				return;
			}
			
			m_health.TakeDamage(damageEvent.damageAmount);
			m_effects.OnDamaged();

			UpdateHealthBar();
		}

		void OnHealthPickUp(in Events.Event a_evt)
		{
			if (!(a_evt is PickUpHealthEvent pickUpHealthEvent))
			{
				return;
			}
			if (pickUpHealthEvent.playerObjectId != gameObject.GetInstanceID())
			{
				return;
			}
			
			m_health.AddHealth(pickUpHealthEvent.healthAmount);

			UpdateHealthBar();
		}

		void UpdateHealthBar()
		{
			float healthFraction = m_health.CurrentHealth / m_health.MaxHealth;
			float newFillAmount  = Mathf.Lerp(MIN_HEALTH_BAR_FILL_AMOUNT, MAX_HEALTH_BAR_FILL_AMOUNT, healthFraction);
			
			healthBarFill.fillAmount = newFillAmount;
		}
	}
}
