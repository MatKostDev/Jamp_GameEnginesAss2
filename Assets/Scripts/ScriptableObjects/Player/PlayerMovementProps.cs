using System;

using UnityEngine;

namespace Jampacked.ProjectInca
{
	[CreateAssetMenu(fileName = "New Player Movement Props", menuName = "Player Movement Props")]
	public class PlayerMovementProps : ScriptableObject
	{
		// PlayerMotor properties
		public PlayerMotorProps Motor
		{
			get { return motor; }
			set { motor = value; }
		}

		// WallRunning properties
		public WallRunProps WallRun
		{
			get { return wallRun; }
			set { wallRun = value; }
		}

		[Serializable]
		public class PlayerMotorProps
		{
			// PlayerMotor properties that govern general movement
			public GeneralProps General
			{
				get { return general; }
				set { general = value; }
			}
			
			// PlayerMotor properties that govern grounded movement
			public MovementProps Grounded
			{
				get { return grounded; }
				set { grounded = value; }
			}

			// PlayerMotor properties that govern airborne movement
			public MovementProps Airborne
			{
				get { return airborne; }
				set { airborne = value; }
			}

			// PlayerMotor properties that govern vertical forces
			public VerticalForceProps VerticalForces
			{
				get { return verticalForceProps; }
				set { verticalForceProps = value; }
			}


			[Serializable]
			public class GeneralProps
			{
				public float ceilingRayCheckDistance = 0.0f;
			}

			[Serializable]
			public class MovementProps
			{
				public float accelerationRate = 0.0f;
				public float decelerationRate = 0.0f;
				public float maxMovementSpeed = 0.0f;
			}

			[Serializable]
			public class VerticalForceProps
			{
				public float jumpHeight       = 0.0f;
				public float gravityStrength  = 0.0f;
				public float terminalVelocity = 0.0f;
			}
			
			[SerializeField]
			private GeneralProps general = new GeneralProps();

			[SerializeField]
			private MovementProps grounded = new MovementProps();

			[SerializeField]
			private MovementProps airborne = new MovementProps();

			[SerializeField]
			private VerticalForceProps verticalForceProps = new VerticalForceProps();
		}

		[Serializable]
		public class WallRunProps
		{
			// WallRunning properties that govern start of wallrun
			public StartupProps Startup
			{
				get { return startupProps; }
				set { startupProps = value; }
			}

			// WallRunning properties that govern subsequent wall runs without touching the ground
			public FollowupProps Followup
			{
				get { return followupProps; }
				set { followupProps = value; }
			}

			// WallRunning properties that govern during wallrun
			public DuringProps During
			{
				get { return duringProps; }
				set { duringProps = value; }
			}

			// WallRunning properties that govern jumping out of a wallrun
			public JumpProps Jump
			{
				get { return jumpProps; }
				set { jumpProps = value; }
			}

			// WallRunning properties that govern miscellaneous aspects of wallrunning
			public OtherProps Other
			{
				get { return otherProps; }
				set { otherProps = value; }
			}

			[Serializable]
			public class StartupProps
			{
				public float startSpeedMultiplier       = 0.0f;
				public float minInitialHorizontalSpeed  = 0.0f;
				public float minInitialVerticalSpeed    = 0.0f;
				public float maxInitialHorizontalSpeed  = 0.0f;
				public float maxInitialVerticalSpeed    = 0.0f;
				public float maxInitialDistanceFromWall = 0.0f;
			}

			[Serializable]
			public class FollowupProps
			{
				[Range(0f, 1f)]
				public float maxInitialVelocityFactor = 0.0f;

				public float sameWallCooldown      = 0.0f;
				
				[Tooltip("The angle in degrees the followup wall normal has to be different from the initial wall")]
				public float normalTolerance       = 0.0f;
				public float interuptCheckDistance = 0.0f;
			}

			[Serializable]
			public class DuringProps
			{
				public float gravityMultiplier                = 0.0f;
				public float slowDownRate                     = 0.0f;
				public float stickToWallStrength              = 0.0f;
				public float maxContinualDistanceFromWall     = 0.0f;
				public float distanceForwardToCheckForBlocker = 0.0f;
			}

			[Serializable]
			public class JumpProps
			{
				public float wallJumpHeight             = 0.0f;
				public float wallJumpHorizontalStrength = 0.0f;

				[Range(0f, 1f)]
				public float velocityKeptAfterWallJumpFactor = 0.0f;
			}

			[Serializable]
			public class OtherProps
			{
				public float     cameraTiltAmount = 0.0f;
				public float     cameraTiltSpeed  = 0.0f;
				public LayerMask wallLayers       = 0;
			}

			[SerializeField]
			private StartupProps startupProps = new StartupProps();

			[SerializeField]
			private FollowupProps followupProps = new FollowupProps();

			[SerializeField]
			private DuringProps duringProps = new DuringProps();

			[SerializeField]
			private JumpProps jumpProps = new JumpProps();

			[SerializeField]
			private OtherProps otherProps = new OtherProps();
		}

		[SerializeField]
		private PlayerMotorProps motor = new PlayerMotorProps()
		{
			General = new PlayerMotorProps.GeneralProps()
			{
				ceilingRayCheckDistance = 0.1f,
			},
			
			Grounded = new PlayerMotorProps.MovementProps()
			{
				accelerationRate = 80f,
				decelerationRate = 60f,
				maxMovementSpeed = 11f,
			},
			Airborne = new PlayerMotorProps.MovementProps()
			{
				accelerationRate = 80f,
				decelerationRate = 60f,
				maxMovementSpeed = 11f,
			},
			VerticalForces = new PlayerMotorProps.VerticalForceProps()
			{
				jumpHeight       = 2.3f,
				gravityStrength  = 50f,
				terminalVelocity = 55f,
			},
		};

		[SerializeField]
		private WallRunProps wallRun = new WallRunProps()
		{
			Startup = new WallRunProps.StartupProps()
			{
				startSpeedMultiplier       = 1.2f,
				minInitialHorizontalSpeed  = 13,
				minInitialVerticalSpeed    = 3,
				maxInitialHorizontalSpeed  = 20,
				maxInitialVerticalSpeed    = 7,
				maxInitialDistanceFromWall = 0.7f,
			},
			Followup = new WallRunProps.FollowupProps()
			{
				maxInitialVelocityFactor = 0.5f,
				normalTolerance           = 20f,
				sameWallCooldown          = 1.2f,
				interuptCheckDistance     = 4.0f,
			},
			During = new WallRunProps.DuringProps()
			{
				gravityMultiplier                = 0.27f,
				slowDownRate                     = 0.15f,
				stickToWallStrength              = 0.5f,
				maxContinualDistanceFromWall     = 0.8f,
				distanceForwardToCheckForBlocker = 0.6f,
			},
			Jump = new WallRunProps.JumpProps()
			{
				wallJumpHeight                  = 1.7f,
				wallJumpHorizontalStrength      = 12f,
				velocityKeptAfterWallJumpFactor = 0.5f,
			},
			Other = new WallRunProps.OtherProps()
			{
				cameraTiltAmount = 14f,
				cameraTiltSpeed  = 18f,
				wallLayers       = 0,
			},
		};
	}
}
