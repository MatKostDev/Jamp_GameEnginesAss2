using UnityEngine;

namespace Jampacked.ProjectInca
{
	[RequireComponent(typeof(Rigidbody))]
	public class SimplePhysicsCollidable : MonoBehaviour
	{
		[SerializeField]
		Vector3 initialVelocity = Vector3.zero;
		
		[SerializeField]
		[Tooltip("This number should be positive if you want gravity to pull the object downwards")]
		float gravityStrength = 10f;
		
		[SerializeField]
		[Tooltip("Disables this simple physics behavior when colliding with a object that has a non-trigger collider")]
		bool disableOnCollision = false;

		Transform m_transform;
		Rigidbody m_rigidBody;
		Collider  m_collider;
		
		Vector3 m_velocity;

		public Vector3 Velocity
		{
			get { return m_velocity; }
			set { m_velocity = value; }
		}

		void Awake()
		{
			m_transform = transform;
			m_collider  = GetComponent<Collider>();
			m_rigidBody = GetComponent<Rigidbody>();

			m_collider.isTrigger    = true;
			m_rigidBody.isKinematic = true;

			m_velocity = initialVelocity;
		}

		void Update()
		{
			m_velocity.y -= gravityStrength * Time.deltaTime;

			m_transform.position += m_velocity * Time.deltaTime;
		}

		void OnTriggerEnter(Collider a_other)
		{
			if (disableOnCollision && !a_other.isTrigger)
			{
				this.enabled = false;
			}
		}
	}
}
