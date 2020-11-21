using System.Linq;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace Jampacked.ProjectInca
{
	public enum InputType
	{
		Keyboard = 0,
		Gamepad  = 1,
	}

	[RequireComponent(typeof(PlayerInput))]
	public class PlayerInputCommandHandler : CharacterCommandHandler
	{
		public override event OnReloadDelegate OnReload;

		public override event OnSwapWeaponDelegate OnSwapWeapon;

		public override event OnShootDelegate OnShoot;

		public override event OnZoomDelegate OnZoom;
		
		public override event OnCrouchDelegate OnCrouch;

		private InputType m_inputType = InputType.Keyboard;

		[SerializeField]
		private Vector2 mouseSensitivity = Vector2.one;

		public Vector2 MouseSensitivity
		{
			get { return mouseSensitivity; }
			set
			{
				if (value.x >= 0)
				{
					mouseSensitivity.x = value.x;
				}
				if (value.y >= 0)
				{
					mouseSensitivity.y = value.y;
				}
			}
		}

		[SerializeField]
		private Vector2 gamepadSensitivity = Vector2.one;

		public Vector2 GamepadSensitivity
		{
			get { return gamepadSensitivity; }
			set
			{
				if (value.x >= 0)
				{
					gamepadSensitivity.x = value.x;
				}
				if (value.y >= 0)
				{
					gamepadSensitivity.y = value.y;
				}
			}
		}

		private PlayerInput m_playerInput = null;

		private InputAction m_lookAction = null;
		private InputAction m_moveAction = null;

		private InputAction m_jumpAction   = null;
		private InputAction m_crouchAction = null;

		private InputAction m_reloadAction      = null;
		private InputAction m_shootAction       = null;
		private InputAction m_weaponSwapAction1 = null;
		private InputAction m_weaponSwapAction2 = null;
		private InputAction m_weaponSwapAction3 = null;
		private InputAction m_weaponSwapAction4 = null;
		private InputAction m_zoomAction        = null;

		private void Start()
		{
			m_playerInput = GetComponent<PlayerInput>();

			m_playerInput.defaultControlScheme = "Keyboard";
			m_playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;

			const int weaponSlot1 = 1;
			const int weaponSlot2 = 2;
			const int weaponSlot3 = 3;
			const int weaponSlot4 = 4;
			
			m_playerInput.onControlsChanged += OnControlsChanged;
			OnControlsChanged(m_playerInput);

			m_lookAction = m_playerInput.actions[nameof(InputMaster.PlayerActions.Look)];
			m_moveAction = m_playerInput.actions[nameof(InputMaster.PlayerActions.Move)];

			m_jumpAction = m_playerInput.actions[nameof(InputMaster.PlayerActions.Jump)];
			m_jumpAction.started += OnJumpAction;
			
			m_crouchAction = m_playerInput.actions[nameof(InputMaster.PlayerActions.Crouch)];
			m_crouchAction.performed += OnCrouchAction;

			m_reloadAction = m_playerInput.actions[nameof(InputMaster.PlayerActions.Reload)];
			m_reloadAction.started += OnReloadAction;

			m_shootAction = m_playerInput.actions[nameof(InputMaster.PlayerActions.Shoot)];
			m_shootAction.performed += OnShootAction;
			
			m_weaponSwapAction1  = m_playerInput.actions[nameof(InputMaster.PlayerActions.WeaponSwap1)];
			m_weaponSwapAction1.started += a_ctx => OnSwapWeaponAction(weaponSlot1);

			m_weaponSwapAction2  = m_playerInput.actions[nameof(InputMaster.PlayerActions.WeaponSwap2)];
			m_weaponSwapAction2.started += a_ctx => OnSwapWeaponAction(weaponSlot2);

			m_weaponSwapAction3  = m_playerInput.actions[nameof(InputMaster.PlayerActions.WeaponSwap3)];
			m_weaponSwapAction3.started += a_ctx => OnSwapWeaponAction(weaponSlot3);

			m_weaponSwapAction4  = m_playerInput.actions[nameof(InputMaster.PlayerActions.WeaponSwap4)];
			m_weaponSwapAction4.started += a_ctx => OnSwapWeaponAction(weaponSlot4);

			m_zoomAction = m_playerInput.actions[nameof(InputMaster.PlayerActions.Zoom)];
			m_zoomAction.performed += OnZoomAction;
		}

		private void OnControlsChanged(PlayerInput a_playerInput)
		{
			m_inputType = a_playerInput.currentControlScheme == "Gamepad" ? InputType.Gamepad : InputType.Keyboard;
		}

		private void Update()
		{
			MoveInput = m_moveAction.ReadValue<Vector2>();

			if (MoveInput.sqrMagnitude > 1)
			{
				MoveInput.Normalize();
			}

			var lookInputRaw = m_lookAction.ReadValue<Vector2>();

			switch (m_inputType)
			{
				case InputType.Keyboard:
				{
					lookInputRaw *= mouseSensitivity;
					break;
				}
				case InputType.Gamepad:
				{
					lookInputRaw *= Time.deltaTime * gamepadSensitivity;
					break;
				}
			}
			
			LookInput = lookInputRaw;

			if (m_shoot)
			{
				OnShoot?.Invoke(m_shotLastFrame);
			}

			m_shotLastFrame = m_shoot;
		}

		protected override void OnJumpAction(InputAction.CallbackContext a_context)
		{
			Jump = true;
		}
		
		protected override void OnCrouchAction(InputAction.CallbackContext a_context)
		{
			bool pressed = a_context.ReadValueAsButton();
			OnCrouch?.Invoke(pressed);
		}

		protected override void OnShootAction(InputAction.CallbackContext a_context)
		{
			m_shoot = a_context.ReadValueAsButton();
		}

		protected override void OnReloadAction(InputAction.CallbackContext a_context)
		{
			OnReload?.Invoke();
		}

		protected override void OnSwapWeaponAction(int a_input)
		{
			OnSwapWeapon?.Invoke(a_input);
		}

		protected override void OnZoomAction(InputAction.CallbackContext a_context)
		{
			OnZoom?.Invoke(a_context.ReadValueAsButton());
		}
	}
}
