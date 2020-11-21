﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Jampacked.ProjectInca.Events;

namespace Jampacked.ProjectInca
{
	public class WeaponHolder : MonoBehaviour
	{
		[SerializeField]
		private List<Weapon> carriedWeapons = new List<Weapon>();

		[Header("Audio")]
		[SerializeField]
		private AudioSource weaponAudioSource = null;

		[Header("UI")]
		[SerializeField]
		private TMP_Text currentReserveAmmoDisplay = null;

		[SerializeField]
		private TMP_Text currentClipAmmoDisplay = null;

		[SerializeField]
		private TMP_Text maxClipAmmoDisplay = null;

		private PlayerController m_playerController;

		private RecoilController m_recoilController;

		private WeaponSway m_swayController;

		private BobbingWithPlayerMovement m_bobbingController;

		private Transform m_weaponParent;

		private Transform m_fppAnalog;

		private Transform m_transform;

		private Camera m_mainCamera;

		private Camera m_weaponCamera;

		private int m_activeWeaponIndex = 1;

		private Weapon m_activeWeapon;

		private GameObject m_ownerObject;

		private EventDispatcher m_dispatcher;

		public Weapon ActiveWeapon
		{
			get { return m_activeWeapon; }
		}

		public PlayerController PlayerControl
		{
			get { return m_playerController; }
		}

		private void Awake()
		{
			m_transform = transform;

			var refs = GetComponentInParent<PlayerReferences>();

			m_ownerObject = refs.gameObject;

			m_playerController = refs.PlayerController;
			m_recoilController = refs.RecoilController;

			m_swayController    = refs.WeaponSwayController;
			m_bobbingController = refs.WeaponBobbingController;

			m_mainCamera   = refs.MainCamera;
			m_weaponCamera = refs.WeaponCamera;

			m_weaponParent = refs.WeaponParent;

			m_fppAnalog = refs.FppAnalog;
		}

		private void Start()
		{
			m_dispatcher = GameObject.Find("GlobalEventDispatcher").GetComponent<EventDispatcher>();
			
			foreach (Weapon weapon in carriedWeapons)
			{
				SetWeaponProperties(weapon);

				weapon.gameObject.SetActive(false);
			}

			SwapToWeaponSlot(m_activeWeaponIndex);

			m_dispatcher.AddListener<PickUpAmmoEvent>(OnAmmoPickedUp);
			
			UpdateAmmoDisplays();
		}

		private void OnDestroy()
		{
			m_dispatcher.RemoveListener<PickUpAmmoEvent>(OnAmmoPickedUp);
		}

		public void UpdateAmmoDisplays()
		{
			currentClipAmmoDisplay.text = m_activeWeapon.CurrentClipAmmo.ToString();
			maxClipAmmoDisplay.text     = m_activeWeapon.MaxClipAmmo.ToString();

			if (m_activeWeapon.MaxReserveAmmo == int.MaxValue)
			{
				currentReserveAmmoDisplay.text = "∞";
			} else
			{
				currentReserveAmmoDisplay.text = m_activeWeapon.CurrentReserveAmmo.ToString();
			}
		}

		private void OnAmmoPickedUp(in Events.Event a_evt)
		{
			if (!(a_evt is PickUpAmmoEvent pickUpAmmoEvent))
			{
				return;
			}
			if (pickUpAmmoEvent.playerObjectId != m_ownerObject.GetInstanceID())
			{
				return;
			}
			
			foreach (var weapon in carriedWeapons)
			{
				if (weapon.WeaponAmmoType != pickUpAmmoEvent.ammoType)
				{
					continue;
				}
				
				weapon.CurrentReserveAmmo += pickUpAmmoEvent.ammoAmount;
				break;
			}

			UpdateAmmoDisplays();
		}

		private void SetWeaponProperties(Weapon a_weapon)
		{
			a_weapon.WeaponAudioSource = weaponAudioSource;

			a_weapon.transform.parent = transform;
		}

		public void AddWeapon(Weapon a_weapon, bool a_setAsActive)
		{
			SetWeaponProperties(a_weapon);

			carriedWeapons.Add(a_weapon);

			if (a_setAsActive)
			{
				SwapToWeaponSlot(carriedWeapons.IndexOf(a_weapon) + 1);
			}
		}

		public void FireActiveWeapon(Vector3 a_fireStartPosition, Vector3 a_fireDirection, bool a_isFireHeldDown)
		{
			if (!a_isFireHeldDown || (a_isFireHeldDown && m_activeWeapon.IsAutomatic))
			{
				m_activeWeapon.FireWeapon(a_fireStartPosition, a_fireDirection);
			}

			UpdateAmmoDisplays();
		}

		public bool ReloadActiveWeapon()
		{
			return m_activeWeapon.Reload();
		}

		public bool ToggleAimDownSightActiveWeapon()
		{
			return m_activeWeapon.ToggleAimDownSight();
		}

		public bool AimActiveWeapon(bool a_aimIn)
		{
			return a_aimIn
				       ? m_activeWeapon.AimIn()
				       : m_activeWeapon.AimOut();
		}

		public void SwapToWeaponSlot(int a_weaponSlotNumber)
		{
			int weaponIndex = a_weaponSlotNumber - 1;

			if (weaponIndex < 0 || weaponIndex >= carriedWeapons.Count || weaponIndex == m_activeWeaponIndex)
			{
				return;
			}

			var currWeapon = carriedWeapons[m_activeWeaponIndex];

			if (currWeapon != null)
			{
				currWeapon.AimOut();
				
				currWeapon.gameObject.SetActive(false);

				currWeapon.transform.SetParent(transform, false);
			}

			m_activeWeaponIndex = weaponIndex;

			var newWeapon = carriedWeapons[weaponIndex];

			if (newWeapon != null)
			{
				newWeapon.gameObject.SetActive(true);

				m_fppAnalog.localPosition = newWeapon.fppTransform.localPosition;
				//m_fppAnalog.localRotation = newWeapon.fppTransform.localRotation;
				m_fppAnalog.localScale    = newWeapon.fppTransform.localScale;

				var newWepTransform = newWeapon.transform;

				newWepTransform.localScale    = Vector3.one;
				newWepTransform.localPosition = Vector3.zero;
				
				newWepTransform.SetParent(m_weaponParent, true);

				newWepTransform.localRotation = Quaternion.identity;
				
				m_activeWeapon = newWeapon;

				m_activeWeapon.OnWeaponSwappedTo();

				m_swayController.swayProps       = newWeapon.swayProps;
				m_bobbingController.BobbingProps = newWeapon.swayProps.Bobbing;

			}
			
			UpdateAmmoDisplays();
		}
	}
}
