using UnityEngine;
using Jampacked.ProjectInca.Events;

namespace Jampacked.ProjectInca
{
	[RequireComponent(typeof(SimplePhysicsCollidable))]
	public class HealthDrop : MonoBehaviour
	{
		public enum DropSize
		{
			Small,
			Large,
		}
		
		[Header("General")]
		[SerializeField]
		float rotationSpeed = 110f;

		[HideInInspector]
		public SimplePhysicsCollidable simplePhysics;

		const float SMALL_DROP_SCALE = 0.75f;
		const float LARGE_DROP_SCALE = 1.15f;
		
		const float SMALL_DROP_AMOUNT = 10f;
		const float LARGE_DROP_AMOUNT = 25f;
		
		const string LOOT_GATHERER_LAYER_NAME = "Loot Gatherer";
		
		EventDispatcher m_dispatcher;

		Transform m_transform;

		float m_healthAmount;
		
		int m_lootGathererLayer;
		
		DropSize m_healthSize;
		public DropSize HealthSize
		{
			get { return m_healthSize; }
			
			set
			{
				float newScale;
				if (value == DropSize.Small)
				{
					newScale       = SMALL_DROP_SCALE;
					m_healthAmount = SMALL_DROP_AMOUNT;
				} else
				{
					newScale       = LARGE_DROP_SCALE;
					m_healthAmount = LARGE_DROP_AMOUNT;
				}

				transform.localScale *= newScale;

				m_healthSize = value;
			}
		}

		void Awake()
		{
			m_dispatcher = GameObject.Find("GlobalEventDispatcher").GetComponent<EventDispatcher>();

			m_transform   = transform;
			simplePhysics = GetComponent<SimplePhysicsCollidable>();

			m_lootGathererLayer = LayerMask.NameToLayer(LOOT_GATHERER_LAYER_NAME);
		}

		void Update()
		{
			m_transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.up);
		}

		void OnTriggerEnter(Collider a_other)
		{
			if (a_other.gameObject.layer == m_lootGathererLayer)
			{
				var healthEvent = new PickUpHealthEvent()
				{
					playerObjectId = a_other.GetComponent<PlayerLootGatherer>().PlayerId,
					healthAmount   = m_healthAmount,
				};
				m_dispatcher.PostEvent(healthEvent);

				Destroy(gameObject);
			}
		}
	}
}
