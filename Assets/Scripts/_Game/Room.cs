using UnityEngine;
using Jampacked.ProjectInca.Events;

namespace Jampacked.ProjectInca
{
	public class Room : MonoBehaviour
	{
		[Header("Enemies")]
		[SerializeField]
		GameObject allEnemiesParent = null;
		
		[Header("Exit Blockers")]
		[SerializeField]
		GameObject[] exitBlockers = new GameObject[1];

		[Header("Entry Triggers")]
		[SerializeField]
		RoomEntryTrigger[] entryTriggers = new RoomEntryTrigger[1];

		EventDispatcher m_dispatcher;
		
		bool m_isRoomLocked = false;
		
		int m_numEnemiesAlive;

		EnemyController[] m_enemiesInRoom;

		void Start()
		{
			m_dispatcher = GameObject.Find("GlobalEventDispatcher").GetComponent<EventDispatcher>();

			m_enemiesInRoom = allEnemiesParent.GetComponentsInChildren<EnemyController>();
			
			foreach (var enemy in m_enemiesInRoom)
			{
				enemy.gameObject.SetActive(false);
			}

			foreach (var blocker in exitBlockers)
			{
				blocker.SetActive(false);
			}
			
			m_dispatcher.AddListener<EnemyKilledEvent>(OnEnemyKilled);
		}

		void OnEnemyKilled(in Events.Event a_evt)
		{
			if (!m_isRoomLocked)
			{
				return;
			}
			if (!(a_evt is EnemyKilledEvent enemyKilledEvent))
			{
				return;
			}

			m_numEnemiesAlive--;

			if (m_numEnemiesAlive <= 0)
			{
				OnRoomCleared();
			}
		}

		public void OnRoomEntered()
		{
			foreach (var entryTrigger in entryTriggers)
			{
				entryTrigger.gameObject.SetActive(false);
			}
			
			if (m_enemiesInRoom.Length > 0)
			{
				m_isRoomLocked    = true;
				m_numEnemiesAlive = m_enemiesInRoom.Length;

				foreach (var enemy in m_enemiesInRoom)
				{
					enemy.gameObject.SetActive(true);
				}

				foreach (var blocker in exitBlockers)
				{
					blocker.SetActive(true);
				}
			}
		}

		void OnRoomCleared()
		{
			m_isRoomLocked = false;
			
			foreach (var blocker in exitBlockers)
			{
				blocker.SetActive(false);
			}
			
			Destroy(this); //destroy this component
		}
	}
}
