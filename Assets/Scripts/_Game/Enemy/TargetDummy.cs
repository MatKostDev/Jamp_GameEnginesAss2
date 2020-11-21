using UnityEngine;

namespace Jampacked.ProjectInca
{
	public class TargetDummy : MonoBehaviour
	{
		Health m_health;
		
		ColorFlashBehaviour m_colorFlash;

		void Start()
		{
			m_health     = GetComponent<Health>();
			m_colorFlash = GetComponent<ColorFlashBehaviour>();

			m_health.onDamaged += OnDamaged;
			m_health.onDie     += OnDie;
		}

		void OnDamaged(float a_damageAmount)
		{
			m_colorFlash.StartColorFlash();
		}

		void OnDie()
		{
			m_health.SetToFull();
		}
	}
}