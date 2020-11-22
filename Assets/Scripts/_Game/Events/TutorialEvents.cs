using UnityEngine;
using Jampacked.ProjectInca.Events;

namespace Jampacked.ProjectInca
{
    public sealed class ObjectiveCompletedEvent : Event<ObjectiveCompletedEvent>
    {

    }

    public sealed class WallRunStartedEvent : Event<WallRunStartedEvent>
    {

    }

    public sealed class WeaponFiredEvent : Event<WeaponFiredEvent>
    {
        public bool didHitEnemy;
    }

    public sealed class EnemyDamagedEvent : Event<EnemyDamagedEvent>
    {
        public float damageDealt;
    }

    public sealed class TutorialCompletedEvent : Event<TutorialCompletedEvent>
    {
        public float timeCompleted;
    }

    public sealed class MetricsPreparedEvent : Event<MetricsPreparedEvent>
    {

    }
}