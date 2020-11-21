using Jampacked.ProjectInca.Events;

namespace Jampacked.ProjectInca
{
	public sealed class DamagePlayerEvent : Event<DamagePlayerEvent>
	{
        public int   playerObjectId;
        public float damageAmount;
    }

	public sealed class PickUpExpEvent : Event<PickUpExpEvent>
	{
		public int playerObjectId;
		public int expAmount;
	}

	public sealed class PickUpHealthEvent : Event<PickUpHealthEvent>
	{
		public int   playerObjectId;
		public float healthAmount;
	}

	public sealed class PickUpAmmoEvent : Event<PickUpAmmoEvent>
	{
		public int             playerObjectId;
		public int             ammoAmount;
		public Weapon.AmmoType ammoType;
	}
}
