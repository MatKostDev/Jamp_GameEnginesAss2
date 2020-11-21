using UnityEngine;

namespace Jampacked.ProjectInca
{
	[RequireComponent(typeof(Collider))]
	public class MeleeHitbox : MonoBehaviour
	{
		MeleeAttack m_attackGroup;

		void Start()
		{
			m_attackGroup = MyHelper.FindFirstParentWithComponent(gameObject, typeof(MeleeAttack)).GetComponent<MeleeAttack>();
		}

		void OnTriggerEnter(Collider a_other)
        {
            if (a_other.TryGetComponent<PlayerHealth>(out var playerHealth))
			{
				m_attackGroup.OnHit(a_other.gameObject.GetInstanceID());
            }
        }
	}
}
