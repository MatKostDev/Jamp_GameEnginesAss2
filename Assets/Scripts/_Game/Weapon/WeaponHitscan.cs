﻿using System.Collections;
using UnityEngine;
using Jampacked.ProjectInca.Events;

namespace Jampacked.ProjectInca
{
	public class WeaponHitscan : Weapon
	{
		[Header("Visual Effects")]
		[SerializeField] 
		ParticleSystem impactEffect = null;
		
		[SerializeField]
		LineRenderer bulletTrailFPP = null;

		[SerializeField]
		LayerMask bulletTrailLayerMaskFPP;

		const float BULLET_TRAIL_LIFETIME         = 2f;
		const float MIN_DISTANCE_FOR_BULLET_TRAIL = 0.8f;

		int m_bulletTrailLayerNumFPP;

        EventDispatcher m_dispatcher;

		protected override void Start()
		{
            m_dispatcher = GameObject.Find("GlobalEventDispatcher").GetComponent<EventDispatcher>();

			base.Start();
			
			//determine layer number based on layermask
			m_bulletTrailLayerNumFPP = (int) Mathf.Log(bulletTrailLayerMaskFPP.value, 2);
		}

		public override bool FireWeapon(
			Vector3 a_fireStartPosition,
			Vector3 a_fireDirection,
			bool    a_isSingleShot = true
		)
		{
			if (!IsAbleToFire())
			{
				return false;
			}

            bool wasEnemyHit = false;
			
			Vector3 bulletTrailEndPos;
			if (Physics.Raycast(
				a_fireStartPosition,
				a_fireDirection,
				out var bulletRayHit,
				range,
				layersToTarget,
				QueryTriggerInteraction.Ignore
			))
			{
				bulletTrailEndPos = bulletRayHit.point;

				CreateImpactEffect(bulletRayHit.point, bulletRayHit.normal);

                if (ProcessBulletHit(bulletRayHit.transform.gameObject, bulletRayHit.point))
                {
                    wasEnemyHit = true;
                }
			} else //no hit
			{
				bulletTrailEndPos = a_fireStartPosition + (a_fireDirection * range);
			}

            var firedEvent = new WeaponFiredEvent()
            {
                didHitEnemy = wasEnemyHit,
            };
            m_dispatcher.PostEvent(firedEvent);

			Vector3 muzzleScreenPosFPP = m_weaponCamera.WorldToScreenPoint(muzzleFPP.position);
			Vector3 muzzleWorldPosFPP  = m_mainCamera.ScreenToWorldPoint(muzzleScreenPosFPP);

			//spawn first person bullet trail
			//if the hit was super close, the trail would look weird so don't draw it
			if (bulletTrailFPP
			    && Vector3.Distance(muzzleFPP.position, bulletTrailEndPos) > MIN_DISTANCE_FOR_BULLET_TRAIL)
			{
                DrawBulletTrail(
                    muzzleWorldPosFPP,
                    bulletTrailEndPos,
                    bulletTrailFPP.gameObject,
                    m_bulletTrailLayerNumFPP
                );
            }

			if (a_isSingleShot)
			{
				animatorFPP.Play("Fire");

				if (fireAudioClip)
				{
					m_audioSource.PlayOneShot(fireAudioClip);
				}

				m_recoilController.ApplyRecoil(recoilDuration, recoilSpeed, minRecoilAmount, maxRecoilAmount);

				m_lastTimeFired = Time.time;

				//reduce ammo and auto-start reload if we're out
				m_currentClipAmmo--;
				if (m_currentClipAmmo <= 0)
				{
					Reload(m_firingDuration);
				}
			}

			return true;
		}

		bool ProcessBulletHit(GameObject a_objectHit, Vector3 a_hitPosition)
		{
			Health     objectHitHealth;
			GameObject objectWithHealth = a_objectHit;

			//try to find a health component
			if (!a_objectHit.TryGetComponent<Health>(out objectHitHealth))
			{
				//go up the hierarchy and see if any parent has a health component
				objectWithHealth =
					MyHelper.FindFirstParentWithComponent(
						a_objectHit,
						typeof(Health)
					); 
				
				if (objectWithHealth)
				{
					objectHitHealth = objectWithHealth.GetComponent<Health>();
				}
			}

			//if the object hit has health, deal damage
			if (objectHitHealth)
			{
				float damageToInflict = damagePerHit;

				bool didHitWeakSpot = false;
				//check for weak spot
				if (a_objectHit.CompareTag("WeakSpot"))
				{
					damageToInflict *= weakSpotMultiplier;
					didHitWeakSpot  =  true;
				}

				objectHitHealth.TakeDamage(damageToInflict);
				
				CreateDamageNumberPopup(a_hitPosition, damageToInflict, didHitWeakSpot);

                return true;
            }

            return false;
        }

		void CreateImpactEffect(Vector3 a_hitPosition, Vector3 a_hitNormal)
		{
			var newImpactEffect = Instantiate(impactEffect, a_hitPosition, Quaternion.LookRotation(a_hitNormal));
			
			Destroy(newImpactEffect, 1f);
		}

		void DrawBulletTrail(
			Vector3    a_startPos,
			Vector3    a_endPos,
			GameObject a_bulletTrailGO,
			int        a_bulletTrailLayer = 0
		)
		{
			GameObject   newBulletTrailGO = Instantiate(a_bulletTrailGO, a_startPos, Quaternion.identity);
			LineRenderer newBulletTrailLR = newBulletTrailGO.GetComponent<LineRenderer>();

			newBulletTrailLR.positionCount = 3;

			newBulletTrailLR.SetPosition(0, a_startPos);
			newBulletTrailLR.SetPosition(1, a_startPos + ((a_endPos - a_startPos) / 2f)); //halfway point
			newBulletTrailLR.SetPosition(2, a_endPos);

			newBulletTrailGO.layer = a_bulletTrailLayer;

			GameObject.Destroy(newBulletTrailGO, BULLET_TRAIL_LIFETIME);
		}

		public override bool Reload(float a_delay = 0f)
		{
			if (!IsAbleToReload())
			{
				return false;
			}

			StartCoroutine(ReloadRoutine(a_delay));

			return true;
		}

		IEnumerator ReloadRoutine(float a_delay)
		{
			AimOut();
			
			m_isReloading = true;

			yield return new WaitForSeconds(a_delay);
			
			animatorFPP.Play("Reload");
			
			yield return new WaitForSeconds(reloadDuration);

			m_isReloading = false;

			if (maxReserveAmmo == int.MaxValue)
			{
				m_currentClipAmmo = maxClipAmmo;
				
				m_holder.UpdateAmmoDisplays();
				
				yield break; //exit coroutine
			}

			int clipAmmoBeforeReload = m_currentClipAmmo;

			if (m_currentClipAmmo + currentReserveAmmo < maxClipAmmo)
			{
				m_currentClipAmmo += currentReserveAmmo;
			} else
			{
				int ammoToRefill = maxClipAmmo - m_currentClipAmmo;

				if (currentReserveAmmo < ammoToRefill)
				{
					m_currentClipAmmo += currentReserveAmmo;
				} else
				{
					m_currentClipAmmo += ammoToRefill;
				}
			}

			currentReserveAmmo -= m_currentClipAmmo - clipAmmoBeforeReload;
			
			m_holder.UpdateAmmoDisplays();
		}
	}
}