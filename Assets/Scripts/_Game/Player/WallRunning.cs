using System.Collections.Generic;

using UnityEngine;

namespace Jampacked.ProjectInca
{
	public class WallRunning : MonoBehaviour
	{
		public enum Side
		{
			Left  = 0,
			Right = 1,
		}

		[SerializeField]
		private PlayerMovementProps playerMovementProps = null;

		private PlayerMovementProps.WallRunProps m_wallrunProps;

		//[Header("Player and Camera")]
		//[SerializeField]
		private Camera m_mainCamera = null;

		private PlayerRotationController m_rotationController = null;

		private const int   NUM_PREVIOUS_WALL_OBJECTS_TO_STORE        = 3;
		private const int   NEW_WALL_NORMAL_OVERWRITE_FRAME_ALLOWANCE = 4;
		private const float MIN_DURATION_TO_COUNT_WALL_RUN            = 0.1f;

		private Transform m_moveTarget;

		private float m_cameraTiltTargetValue;

		private Side    m_wallSide;
		private float   m_wallJumpVelocityY;
		private Vector3 m_wallNormal;
		private Vector3 m_moveDirection;
		private bool    m_touchedGroundSinceLastWallRun;
		private float   m_lastTimeWallRunStarted;

		private LinkedList<(GameObject ObjectRunOn, float TimeEnded)> m_lastWallRuns =
			new LinkedList<(GameObject, float)>();

		private Vector3    m_lastWallNormal;
		private GameObject m_newWallHitObject;
		private Vector3    m_newWallFirstHitNormal;
		private int        m_newWallFirstHitFrame;
		private bool       m_lastWallInterupted;

		private bool m_isWallRunning = false;

		public bool IsWallRunning
		{
			get { return m_isWallRunning; }
		}

		public float WallJumpHeight
		{
			get { return m_wallrunProps.Jump.wallJumpHeight; }
		}

		public GameObject LastObjectWallRunOn
		{
			get { return m_lastWallRuns.Last.Value.ObjectRunOn; }
		}

		public float LastTimeWallRun
		{
			get { return m_lastWallRuns.Last.Value.TimeEnded; }
		}

		private void Awake()
		{
			var refs = GetComponentInParent<PlayerReferences>();

			m_moveTarget = refs.MoveTarget;

			m_mainCamera = refs.MainCamera;

			m_rotationController = refs.RotationController;

			m_wallrunProps = playerMovementProps.WallRun;
			
			m_lastWallRuns.AddLast((null, Mathf.NegativeInfinity));
		}

		private void Update()
		{
			UpdateCameraTilt();
		}

		private void UpdateCameraTilt()
		{
			float currentTiltValue = m_rotationController.Roll;
			if (Mathf.Approximately(currentTiltValue, m_cameraTiltTargetValue))
			{
				return;
			}

			m_rotationController.Roll = Mathf.Lerp(
				currentTiltValue,
				m_cameraTiltTargetValue,
				m_wallrunProps.Other.cameraTiltSpeed * Time.deltaTime
			);
		}

		public void OnPlayerGrounded()
		{
			m_touchedGroundSinceLastWallRun = true;
		}

		public void SetWallJumpHeight(float a_newWallJumpHeight, float a_gravityStrength)
		{
			m_wallrunProps.Jump.wallJumpHeight = a_newWallJumpHeight;
			m_wallJumpVelocityY =
				Mathf.Sqrt(
					m_wallrunProps.Jump.wallJumpHeight * 2f * a_gravityStrength
				); //calculate velocity needed in order to reach desired jump height
		}

		public void CheckForWallRun(ref Vector3 a_velocity, bool a_isGrounded, float a_verticalAxis)
		{
			float      raycastHitDistance = 999f;
			GameObject wallHitGameObject  = null;
			Vector3    wallHitNormal      = Vector3.zero;

			//raycast left and right and see if either hit, if both hit take the one that's closest
			if (Physics.Raycast(
				m_mainCamera.transform.position,
				-m_moveTarget.right,
				out RaycastHit leftHit,
				m_wallrunProps.Startup.maxInitialDistanceFromWall,
				m_wallrunProps.Other.wallLayers,
				QueryTriggerInteraction.Ignore
			))
			{
				m_wallSide         = Side.Left;
				raycastHitDistance = leftHit.distance;
				wallHitGameObject  = leftHit.transform.gameObject;
				wallHitNormal      = leftHit.normal;
			}
			if (Physics.Raycast(
				m_mainCamera.transform.position,
				m_moveTarget.right,
				out RaycastHit rightHit,
				m_wallrunProps.Startup.maxInitialDistanceFromWall,
				m_wallrunProps.Other.wallLayers,
				QueryTriggerInteraction.Ignore
			))
			{
				if (rightHit.distance < raycastHitDistance)
				{
					m_wallSide        = Side.Right;
					wallHitGameObject = rightHit.transform.gameObject;
					wallHitNormal     = rightHit.normal;
				}
			}

			if (Physics.Raycast(
				m_mainCamera.transform.position,
				-m_lastWallNormal,
				out RaycastHit interruptCheckHit,
				m_wallrunProps.Followup.interuptCheckDistance,
				m_wallrunProps.Other.wallLayers,
				QueryTriggerInteraction.Ignore
			))
			{
				GameObject interruptHitObject = interruptCheckHit.transform.gameObject;

				bool isNewWall = false;
				
				//if this is the same wall we just ran on, keep updating the last normal
				if (interruptHitObject == LastObjectWallRunOn)
				{
					//don't update normal if there was a large, sudden change, as it's probably on an edge
					if (Vector3.Dot(m_lastWallNormal, interruptCheckHit.normal) > 0.8f)
					{
						m_lastWallNormal = interruptCheckHit.normal;
					}
				} else if (interruptHitObject != m_newWallHitObject)
				{
					isNewWall               = true;
					m_newWallFirstHitFrame  = Time.frameCount;
					m_newWallHitObject      = interruptHitObject;
					m_newWallFirstHitNormal = interruptCheckHit.normal;
				}

				//check if this wall is new OR we're within the frame allowance to update the wall's normal
				//this allows us to ensure we don't just hit the edge of a wall and count that as the first normal hit
				if (isNewWall || (m_newWallFirstHitFrame + NEW_WALL_NORMAL_OVERWRITE_FRAME_ALLOWANCE > Time.frameCount))
				{
					m_newWallHitObject      = interruptHitObject;
					m_newWallFirstHitNormal = interruptCheckHit.normal;
				}
			} else if (!m_lastWallInterupted)
			{
				m_lastWallInterupted = true;
			}

			if (IsWallRunStartValid(wallHitGameObject, wallHitNormal, a_isGrounded, a_verticalAxis))
			{
				StartWallRun(ref a_velocity, wallHitGameObject, wallHitNormal);
			}
		}

		private bool IsWallRunStartValid(GameObject a_wallObject, Vector3 a_normal, bool a_isGrounded, float a_verticalAxis)
		{
			//check if there is a wall to run on and if we're able to start a wall run
			if (!a_wallObject
			    || a_isGrounded
			    || a_verticalAxis <= 0.2f)
			{
				return false;
			}
			
			// Check if the player is facing away from the wall
			if (Vector3.Dot(m_moveTarget.forward, a_normal) > Mathf.Cos((90 - 10) * Mathf.Deg2Rad))
			{
				return false;
			}

			//if ground was touched since the last wall run, a new wall run can be done without waiting for cooldown
			if (m_touchedGroundSinceLastWallRun)
			{
				return true;
			}

			if (!m_lastWallInterupted)
			{
				//determine new wall normal, either from the parameter
				//or from the normal we hit with a raycast earlier if it was the same wall
				Vector3 newWallNormal = a_normal;
				if (a_wallObject == m_newWallHitObject)
				{
					newWallNormal = m_newWallFirstHitNormal;
				}

				if (Vector3.Dot(m_lastWallNormal, newWallNormal) > Mathf.Cos(m_wallrunProps.Followup.normalTolerance * Mathf.Deg2Rad))
				{
					return false;
				}
			} else if (a_wallObject != LastObjectWallRunOn)
			{
				return true;
			}

			//check through previous wall runs to check if we ran on the wall and if the wall is off cooldown
			foreach (var (objectRunOn, timeEnded) in m_lastWallRuns)
			{
				if (objectRunOn                                             == a_wallObject
				    && timeEnded + m_wallrunProps.Followup.sameWallCooldown > Time.time)
				{
					return false;
				}
			}

			return true;
		}

		private void StartWallRun(ref Vector3 a_velocity, GameObject a_objectBeingWallRunOn, Vector3 a_surfaceNormal)
		{
			m_wallNormal = a_surfaceNormal;

			m_isWallRunning = true;

			if (m_wallSide == Side.Left)
			{
				m_moveDirection.x = -m_wallNormal.z;
				m_moveDirection.z = m_wallNormal.x;

				m_cameraTiltTargetValue = -m_wallrunProps.Other.cameraTiltAmount;
			} else //right side
			{
				m_moveDirection.x = m_wallNormal.z;
				m_moveDirection.z = -m_wallNormal.x;

				m_cameraTiltTargetValue = m_wallrunProps.Other.cameraTiltAmount;
			}

			//calculate new horizontal velocity by accounting for current velocity and new direction (which is based on wall normal)
			Vector3 velocityXZ = new Vector3(a_velocity.x, 0f, a_velocity.z);

			float oldVelocityInfluence = GetInfluenceOnNewVelocity(velocityXZ, m_moveDirection);

			Vector3 newVelocityXZ =
				m_moveDirection
				* velocityXZ.magnitude
				* oldVelocityInfluence
				* m_wallrunProps.Startup.startSpeedMultiplier;
			newVelocityXZ.y = 0f; //just in case

			if (newVelocityXZ.magnitude < m_wallrunProps.Startup.minInitialHorizontalSpeed)
			{
				a_velocity.x = m_moveDirection.x * m_wallrunProps.Startup.minInitialHorizontalSpeed;
				a_velocity.z = m_moveDirection.z * m_wallrunProps.Startup.minInitialHorizontalSpeed;
			} else
			{
				if (newVelocityXZ.magnitude > m_wallrunProps.Startup.maxInitialHorizontalSpeed)
				{
					newVelocityXZ = Vector3.Normalize(newVelocityXZ) * m_wallrunProps.Startup.maxInitialHorizontalSpeed;
				}

				a_velocity.x = newVelocityXZ.x;
				a_velocity.z = newVelocityXZ.z;
			}

			//check if we recently finished a wall run
			if (LastTimeWallRun + m_wallrunProps.Followup.sameWallCooldown > Time.time
			    && !m_touchedGroundSinceLastWallRun)
			{
				float min    = m_wallrunProps.Startup.minInitialVerticalSpeed;
				float max    = m_wallrunProps.Startup.maxInitialVerticalSpeed;
				float factor = m_wallrunProps.Followup.maxInitialVelocityFactor;

				float maxAdjusted = Mathf.Lerp(min, max, factor);

				a_velocity.y = Mathf.Clamp(a_velocity.y, min, maxAdjusted);
			} else
			{
				a_velocity.y *= m_wallrunProps.Startup.startSpeedMultiplier;
				a_velocity.y = Mathf.Clamp(
					a_velocity.y,
					m_wallrunProps.Startup.minInitialVerticalSpeed,
					m_wallrunProps.Startup.maxInitialVerticalSpeed
				);
			}

			//push player into the wall
			a_velocity += -m_wallNormal * m_wallrunProps.During.stickToWallStrength;

			StoreNewWallRun(a_objectBeingWallRunOn, a_surfaceNormal, Time.time);

			m_lastTimeWallRunStarted        = Time.time;
			m_touchedGroundSinceLastWallRun = false;
		}

		private float GetInfluenceOnNewVelocity(Vector3 a_oldVelocity, Vector3 a_newMoveDirection)
		{
			Vector3 velocityDirectionXZ     = Vector3.Normalize(a_oldVelocity);
			float   velocityDotNewDirection = Vector3.Dot(velocityDirectionXZ, a_newMoveDirection);

			//influence that the player's current velocity will have on the new (wall run) velocity, based on direction
			float wallRunVelocityInfluence = Mathf.Clamp(velocityDotNewDirection * 2f, 0f, 1f);

			return wallRunVelocityInfluence;
		}

		private void StoreNewWallRun(GameObject a_wallObject, Vector3 a_normal, float a_time)
		{
			m_lastWallRuns.AddLast((a_wallObject, a_time));
			if (m_lastWallRuns.Count > NUM_PREVIOUS_WALL_OBJECTS_TO_STORE)
			{
				m_lastWallRuns.RemoveFirst();
			}
			m_lastWallNormal     = a_normal;
			m_lastWallInterupted = false;
		}

		public void UpdateWallRunning(
			ref Vector3 a_velocity,
			float       a_gravityStrength,
			bool        a_isGrounded,
			Vector2     a_movementAxes
		)
		{
			Vector3 lastFrameWallNormal = m_wallNormal;

			//update last wall run time
			var (obj, time)           = m_lastWallRuns.Last.Value;
			m_lastWallRuns.Last.Value = (obj, Time.time);

			//check if the player pushed the movement key away from the wall
			const float hTolerance = 0.5f;
			const float vTolerance = 0.2f;
			if ((m_wallSide    == Side.Left  && a_movementAxes.x > hTolerance)
			    || (m_wallSide == Side.Right && a_movementAxes.x < -hTolerance))
			{
				PerformWallKickoff(ref a_velocity);
				return;
			} else if (a_isGrounded || a_movementAxes.y < -vTolerance)
			{
				StopWallRun();
				return;
			}

			//check for no raycast hit to the side, which means we don't have a wall to run on anymore
			if (!Physics.Raycast(
				    m_moveTarget.position,
				    -m_wallNormal,
				    out RaycastHit sideWallRay,
				    m_wallrunProps.During.maxContinualDistanceFromWall,
				    m_wallrunProps.Other.wallLayers
			    ))
			{
				if (!CheckToDiscardLastWallRun() && a_velocity.y > 0)
				{
					PerformWallVerticalBoost(ref a_velocity);
				} else
				{
					StopWallRun();
				}

				return;
			}

			//check if we transitioned onto a new wall, in which case we need to store it
			if (sideWallRay.transform.gameObject != LastObjectWallRunOn)
			{
				//check if the wall is just a trigger volume
				if (IsWallTrigger(sideWallRay.collider))
				{
					StopWallRun();
					return;
				}
				
				StoreNewWallRun(sideWallRay.transform.gameObject, sideWallRay.normal, Time.time);
			}

			m_wallNormal     = sideWallRay.normal;
			m_lastWallNormal = sideWallRay.normal;

			if (m_wallNormal != lastFrameWallNormal)
			{
				Vector3 oldMoveDirection = m_moveDirection;

				if (m_wallSide == Side.Left)
				{
					m_moveDirection.x = -m_wallNormal.z;
					m_moveDirection.z = m_wallNormal.x;
				} else //right side
				{
					m_moveDirection.x = m_wallNormal.z;
					m_moveDirection.z = -m_wallNormal.x;
				}

				//calculate new horizontal velocity by accounting for current velocity and new direction (which is based on wall normal)
				Vector3 velocityXZ = new Vector3(a_velocity.x, 0f, a_velocity.z);

				float oldVelocityInfluence = GetInfluenceOnNewVelocity(velocityXZ, oldMoveDirection);

				Vector3 newVelocityXZ =
					m_moveDirection * velocityXZ.magnitude * oldVelocityInfluence;

				a_velocity = new Vector3(newVelocityXZ.x, a_velocity.y, newVelocityXZ.z);

				a_velocity += -m_wallNormal * m_wallrunProps.During.stickToWallStrength;
			}

			//check for raycast hit in front, which means something is blocking us from continuing the wall run
			if (Physics.Raycast(
				m_mainCamera.transform.position,
				m_moveDirection,
				out RaycastHit frontBlockerRay,
				m_wallrunProps.During.distanceForwardToCheckForBlocker,
				m_wallrunProps.Other.wallLayers
			))
			{
				StopWallRun();
				return;
			}

			Vector3 clampedVelocityXZ = a_velocity;
			clampedVelocityXZ.y = 0f;
			clampedVelocityXZ = Vector3.ClampMagnitude(
				clampedVelocityXZ,
				clampedVelocityXZ.magnitude - (m_wallrunProps.During.slowDownRate * Time.deltaTime)
			);

			a_velocity.x = clampedVelocityXZ.x;
			a_velocity.z = clampedVelocityXZ.z;

			a_velocity.y -= a_gravityStrength * m_wallrunProps.During.gravityMultiplier * Time.deltaTime;
		}

		public void PerformWallVerticalBoost(ref Vector3 a_velocity)
		{
			a_velocity.y = m_wallJumpVelocityY * 0.85f;

			StopWallRun();
		}

		public void PerformWallKickoff(ref Vector3 a_velocity)
		{
			a_velocity *= m_wallrunProps.Jump.velocityKeptAfterWallJumpFactor; //take a portion of the current velocity
			a_velocity += m_wallNormal * m_wallrunProps.Jump.wallJumpHorizontalStrength;

			StopWallRun();
		}

		public void PerformWallJump(ref Vector3 a_velocity)
		{
			PerformWallKickoff(ref a_velocity);

			a_velocity.y = m_wallJumpVelocityY;

			StopWallRun();
		}

		private bool IsWallTrigger(Collider a_wallCollider)
		{
			if (a_wallCollider.isTrigger)
			{
				return true;
			}

			return false;
		}

		public void StopWallRun()
		{
			m_isWallRunning = false;

			m_cameraTiltTargetValue = 0f;
		}

		private bool CheckToDiscardLastWallRun()
		{
			float lastWallRunDuration = LastTimeWallRun - m_lastTimeWallRunStarted;
			if (lastWallRunDuration < MIN_DURATION_TO_COUNT_WALL_RUN)
			{
				m_lastWallRuns.RemoveLast();
				return true;
			}

			return false;
		}
	}
}
