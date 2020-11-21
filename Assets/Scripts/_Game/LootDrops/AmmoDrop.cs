using UnityEngine;
using Jampacked.ProjectInca.Events;

namespace Jampacked.ProjectInca
{
	[RequireComponent(typeof(SimplePhysicsCollidable))]
	public class AmmoDrop : MonoBehaviour
	{
		public enum DropSize
		{
			Small,
			Large,
		}
		
		[Header("General")]
		[SerializeField]
		float rotationSpeed = 110f;

		[Header("Ammo Drop Models")]
		[SerializeField]
		Mesh assaultRifleAmmoMesh = null;
		
		[SerializeField]
		Mesh shotgunAmmoMesh = null;
		
		[SerializeField]
		Mesh marksmanRifleAmmoMesh = null;

		[HideInInspector]
		public SimplePhysicsCollidable simplePhysics;

		const float SMALL_DROP_SCALE_FACTOR = 0.5f;
		const float LARGE_DROP_SCALE_FACTOR = 0.75f;
		
		const int SMALL_ASSAULT_RIFLE_AMOUNT = 30;
		const int LARGE_ASSAULT_RIFLE_AMOUNT = 90;

		const int SMALL_SHOTGUN_AMOUNT = 8;
		const int LARGE_SHOTGUN_AMOUNT = 24;

		const int SMALL_MARKSMAN_RIFLE_AMOUNT = 8;
		const int LARGE_MARKSMAN_RIFLE_AMOUNT = 24;

		const string LOOT_GATHERER_LAYER_NAME = "Loot Gatherer";

		EventDispatcher m_dispatcher;

		Transform  m_transform;
		MeshFilter m_meshFilter;

		int m_ammoAmount;

		Weapon.AmmoType m_ammoType;

		DropSize m_ammoSize;

		int m_lootGathererLayer;

		void Awake()
		{
			m_dispatcher = GameObject.Find("GlobalEventDispatcher").GetComponent<EventDispatcher>();

			m_transform   = transform;
			m_meshFilter  = GetComponent<MeshFilter>();
			simplePhysics = GetComponent<SimplePhysicsCollidable>();

			int firstAmmoTypeIndex = 1; //0th index is none
			int lastAmmoTypeIndex  = System.Enum.GetValues(typeof(Weapon.AmmoType)).Length; //get the size of the ammo type enum
			m_ammoType = (Weapon.AmmoType)Random.Range(firstAmmoTypeIndex, lastAmmoTypeIndex); //TODO: instead of completely random, use weights based on weapons currently held
			
			m_lootGathererLayer = LayerMask.NameToLayer(LOOT_GATHERER_LAYER_NAME);
		}

		void Update()
		{
			m_transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.forward);
		}

		void OnTriggerEnter(Collider a_other)
		{
			if (a_other.gameObject.layer == m_lootGathererLayer)
			{
				var ammoEvent = new PickUpAmmoEvent()
				{
					playerObjectId = a_other.GetComponent<PlayerLootGatherer>().PlayerId,
					ammoAmount     = m_ammoAmount,
					ammoType       = m_ammoType,
				};
				m_dispatcher.PostEvent(ammoEvent);

				Destroy(gameObject);
			}
		}

		public void InitAmmoDrop(DropSize a_ammoDropSize)
		{
			float scaleFactor;
			if (a_ammoDropSize == DropSize.Small)
			{
				scaleFactor = SMALL_DROP_SCALE_FACTOR;

				if (m_ammoType == Weapon.AmmoType.AssaultRifle)
				{
					m_meshFilter.mesh = assaultRifleAmmoMesh;
					m_ammoAmount      = SMALL_ASSAULT_RIFLE_AMOUNT;
				} else if (m_ammoType == Weapon.AmmoType.Shotgun)
				{
					m_meshFilter.mesh = shotgunAmmoMesh;
					m_ammoAmount      = SMALL_SHOTGUN_AMOUNT;
				} else if (m_ammoType == Weapon.AmmoType.MarksmanRifle)
				{
					m_meshFilter.mesh = marksmanRifleAmmoMesh;
					m_ammoAmount      = SMALL_MARKSMAN_RIFLE_AMOUNT;
				}
			} else
			{
				scaleFactor = LARGE_DROP_SCALE_FACTOR;

				if (m_ammoType == Weapon.AmmoType.AssaultRifle)
				{
					m_meshFilter.mesh = assaultRifleAmmoMesh;
					m_ammoAmount      = LARGE_ASSAULT_RIFLE_AMOUNT;
				}
				else if (m_ammoType == Weapon.AmmoType.Shotgun)
				{
					m_meshFilter.mesh = shotgunAmmoMesh;
					m_ammoAmount      = LARGE_SHOTGUN_AMOUNT;
				}
				else if (m_ammoType == Weapon.AmmoType.MarksmanRifle)
				{
					m_meshFilter.mesh = marksmanRifleAmmoMesh;
					m_ammoAmount      = LARGE_MARKSMAN_RIFLE_AMOUNT;
				}
			}

			m_transform.localScale *= scaleFactor;

			m_ammoSize = a_ammoDropSize;
		}
	}
}
