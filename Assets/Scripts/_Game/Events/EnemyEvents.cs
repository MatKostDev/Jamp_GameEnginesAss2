using Jampacked.ProjectInca.Events;

namespace Jampacked.ProjectInca
{
	public sealed class EnemyKilledEvent : Event<EnemyKilledEvent>
	{
		public int enemyObjectId;
	}

    public sealed class TutorialEnemyKilledEvent : Event<TutorialEnemyKilledEvent>
    {
        public int enemyObjectId;
    }
}