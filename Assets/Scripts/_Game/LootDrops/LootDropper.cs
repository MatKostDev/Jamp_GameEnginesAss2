using UnityEngine;

namespace Jampacked.ProjectInca
{
	public class LootDropper : MonoBehaviour
	{
		[Header("Drop Positions")]
		[SerializeField]
		Vector3 expDropPositionOffset = Vector3.zero;
		
		[SerializeField]
		Vector3 healthDropPositionOffset = Vector3.zero;
		
		[SerializeField]
		Vector3 ammoDropPositionOffset = Vector3.zero;

		[Header("Drop Start Velocities")]
		[SerializeField]
		Vector3 expStartVelocity = Vector3.zero;

		[SerializeField]
		Vector3 healthStartVelocity = Vector3.zero;

		[SerializeField]
		Vector3 ammoStartVelocity = Vector3.zero;

		[Header("Exp")]
		[SerializeField]
		ExpDrop expPrefab = null;
		
        [SerializeField]
        int minExpAmount = 0;

        [SerializeField]
        int maxExpAmount = 0;

        [Header("Health")]
        [SerializeField]
        HealthDrop healthPrefab = null;
        
        [SerializeField]
        [Tooltip("Set to -1 if you don't want it to drop at all")]
		[Range(0, 1)]
        float smallHealthDropRate = 0.4f;

        [SerializeField]
        [Tooltip("Set to -1 if you don't want it to drop at all")]
		[Range(0, 1)]
        float largeHealthDropRate = 0.2f;

		[Header("Ammo")]
        [SerializeField]
        AmmoDrop ammoPrefab = null;

        [SerializeField]
        [Tooltip("Set to -1 if you don't want it to drop at all")]
        [Range(0, 1)]
        float smallAmmoDropRate = 0.5f;

        [SerializeField]
        [Tooltip("Set to -1 if you don't want it to drop at all")]
        [Range(0, 1)]
        float largeAmmoDropRate = 0.25f;

		int m_expAmount;

        void Awake()
        {
	        m_expAmount = Random.Range(minExpAmount, maxExpAmount + 1);
        }

        public void SpawnLoot()
        {
	        SpawnExpDrop();
	        DetermineHealthDrop();
	        DetermineAmmoDrop();

        }

        public void SpawnExpDrop()
        {
	        Vector3 dropPosition = transform.position + expDropPositionOffset;
	        
	        ExpDrop exp = Instantiate(expPrefab); //TODO: object pool this
	        exp.transform.position = dropPosition;

	        exp.RewardAmount = m_expAmount;
	        
	        exp.simplePhysics.Velocity = expStartVelocity;
        }

        void DetermineHealthDrop()
        {
	        HealthDrop.DropSize healthSize;
	        
	        if (Random.value <= largeHealthDropRate)
	        {
		        healthSize = HealthDrop.DropSize.Large;
	        } else if (Random.value <= smallHealthDropRate)
	        {
		        healthSize = HealthDrop.DropSize.Small;
            } else
	        {
		        return;
	        }

	        Vector3 dropPosition = transform.position + healthDropPositionOffset;
	        
	        HealthDrop healthDrop = Instantiate(healthPrefab); //TODO: object pool this
	        healthDrop.transform.position = dropPosition;

			healthDrop.HealthSize = healthSize;

	        healthDrop.simplePhysics.Velocity = healthStartVelocity;
		}

        void DetermineAmmoDrop()
        {
			AmmoDrop.DropSize ammoSize;

			if (Random.value <= largeAmmoDropRate)
			{
				ammoSize = AmmoDrop.DropSize.Large;
			}
			else if (Random.value <= smallAmmoDropRate)
			{
				ammoSize = AmmoDrop.DropSize.Small;
			}
			else
			{
				return;
			}

			Vector3 dropPosition = transform.position + ammoDropPositionOffset;
			
			AmmoDrop ammoDrop = Instantiate(ammoPrefab); //TODO: object pool this
			ammoDrop.transform.position = dropPosition;

			ammoDrop.InitAmmoDrop(ammoSize);

			ammoDrop.simplePhysics.Velocity = ammoStartVelocity;
		}
	}
}
