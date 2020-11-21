using UnityEngine;

namespace Jampacked.ProjectInca
{
	[RequireComponent(typeof(Collider))]
	public class RoomEntryTrigger : MonoBehaviour
	{
		Room m_roomParent;
		
		void Start()
		{
			m_roomParent = MyHelper.FindFirstParentWithComponent(gameObject, typeof(Room)).GetComponent<Room>();

			GetComponent<Collider>().isTrigger = true;
		}
		
		void OnTriggerEnter(Collider a_other)
		{
			if (a_other.TryGetComponent<PlayerReferences>(out var playerRefs))
			{
				m_roomParent.OnRoomEntered();
			}
		}
	}
}
