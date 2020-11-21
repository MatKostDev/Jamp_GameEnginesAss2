using UnityEngine;
using UnityEngine.Events;

namespace Jampacked.ProjectInca
{
	public class Health : MonoBehaviour
	{
		[SerializeField]
		float maxHealth = 100f;

		public UnityAction<float> onHealthAdded;
		public UnityAction<float> onDamaged;
		public UnityAction        onDie;
		
		bool m_isAlive; //ensures onDie isnt invoked more than once

		float m_currentHealth;
		public float CurrentHealth
		{
			get { return m_currentHealth; }
		}

		public float MaxHealth
		{
			get { return maxHealth; }
		}

		void Start()
		{
			m_currentHealth = maxHealth;
			m_isAlive     = true;
		}

		public void SetToFull()
		{
			m_currentHealth = maxHealth;
		}

		public void AddHealth(float a_healthToAdd)
		{
			m_currentHealth += a_healthToAdd;
			m_currentHealth =  Mathf.Min(m_currentHealth, maxHealth);
			
			onHealthAdded?.Invoke(a_healthToAdd);
		}

		public void TakeDamage(float a_damageAmount)
		{
			if (!m_isAlive)
			{
				return;
			}
			
			m_currentHealth -= a_damageAmount;
			m_currentHealth =  Mathf.Max(m_currentHealth, 0f);

			onDamaged?.Invoke(a_damageAmount);

			if (m_currentHealth <= 0f)
			{
				onDie?.Invoke();
				m_isAlive = false;
			}
		}
	}
}