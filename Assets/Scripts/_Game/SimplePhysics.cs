using UnityEngine;

namespace Jampacked.ProjectInca
{
	public class SimplePhysics : MonoBehaviour
	{
		[SerializeField]
		Vector3 initialVelocity = Vector3.zero;

		[SerializeField]
		[Tooltip("This number should be positive if you want gravity to pull the object downwards")]
		float gravityStrength = 10f;

		Transform m_transform;

		Vector3 m_velocity;

		public Vector3 Velocity
		{
			get { return m_velocity; }
			set { m_velocity = value; }
		}

		void Awake()
		{
			m_transform = transform;

			m_velocity = initialVelocity;
		}

		void Update()
		{
			m_velocity.y -= gravityStrength * Time.deltaTime;

			m_transform.position += m_velocity * Time.deltaTime;
		}
	}
}
