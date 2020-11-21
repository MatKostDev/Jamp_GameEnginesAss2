using Jampacked.ProjectInca.Events;

namespace Jampacked.ProjectInca
{
	public sealed class RoomEnteredEvent : Event<RoomEnteredEvent>
	{
		public int numEnemiesInRoom;
	}

	public sealed class RoomClearedEvent : Event<RoomClearedEvent>
	{
		
	}
}