using UnityEngine;

namespace Jampacked.ProjectInca
{
	[RequireComponent(typeof(Collider))]
	public class PlayerLootGatherer : MonoBehaviour
	{
		private int playerId;
		public int PlayerId
		{
			get { return playerId; }
		}
		
		private void Awake()
		{
			var refs = GetComponentInParent<PlayerReferences>();

			playerId = refs.PlayerObjectId;

			GetComponent<Collider>().isTrigger = true;
		}
	}
}
