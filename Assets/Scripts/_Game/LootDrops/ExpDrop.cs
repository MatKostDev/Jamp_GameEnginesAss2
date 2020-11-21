using Jampacked.ProjectInca.Events;
using UnityEngine;

namespace Jampacked.ProjectInca
{
	[RequireComponent(typeof(SimplePhysicsCollidable))]
	public class ExpDrop : MonoBehaviour
	{
		[HideInInspector]
		public SimplePhysicsCollidable simplePhysics;
		
		const int AMOUNT_FOR_MAX_SIZE = 50;
		
		const float MIN_SCALE = 0.35f;
		const float MAX_SCALE = 1.0f;

		const string LOOT_GATHERER_LAYER_NAME = "Loot Gatherer";

		EventDispatcher m_dispatcher;

		int m_lootGathererLayer;

		int m_rewardAmount;
		public int RewardAmount
		{
			get { return m_rewardAmount; }

			set
			{
				m_rewardAmount = value;

				float amountFractionOfMax = Mathf.Clamp01((float)m_rewardAmount / AMOUNT_FOR_MAX_SIZE);
				float newScaleValue       = Mathf.Lerp(MIN_SCALE, MAX_SCALE, amountFractionOfMax);
				
				transform.localScale = new Vector3(newScaleValue, newScaleValue, newScaleValue);
			}
		}

		void Awake()
		{
			m_dispatcher = GameObject.Find("GlobalEventDispatcher").GetComponent<EventDispatcher>();

			simplePhysics = GetComponent<SimplePhysicsCollidable>();

			m_lootGathererLayer = LayerMask.NameToLayer(LOOT_GATHERER_LAYER_NAME);
		}

		void OnTriggerEnter(Collider a_other)
		{
			if (a_other.gameObject.layer == m_lootGathererLayer)
			{
				var expEvent = new PickUpExpEvent()
				{
					playerObjectId = a_other.GetComponent<PlayerLootGatherer>().PlayerId,
					expAmount      = m_rewardAmount,
				};
				m_dispatcher.PostEvent(expEvent);
				
				Destroy(gameObject);
			}
		}
	}
}
