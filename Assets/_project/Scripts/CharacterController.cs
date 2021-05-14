using System;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TOJam.FLR
{
	public class CharacterController : MonoBehaviour, ICharacterController
	{
		[SerializeField] private KinematicCharacterMotor _motor;
		[SerializeField] private Animator _animator;

		[SerializeField] private InputActionProperty _moveAction;
		[SerializeField] private InputActionProperty _jumpAction;
		[SerializeField] private InputActionProperty _slideAction;
		[SerializeField] private InputActionProperty _specialAction;

		public Vector2 _currentInput;
		public bool _isJumpPressed;
		public bool _isSlidePressed;
		public bool _isSpecialPressed;

		#region MonoBehaviour

		private void Awake()
		{
			_motor.CharacterController = this;
		}

		private void Update()
		{
			ProcessInput();
		}
		
		#endregion MonoBehaviouor

		#region Input

		private void ProcessInput()
		{
			_currentInput = _moveAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
		}

		private void ProcessJumpInput()
		{
		}

		private void ProcessSlideInput()
		{
			
		}

		private void ProcessSpecialInput()
		{
			
		}

		private bool IsActionDown(in InputActionProperty property)
		{
			return property.action?.ReadValue<float>() > InputSystem.settings.defaultButtonPressPoint;
		}

		#endregion Input

		#region ICharacterController

		public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
		{
		}

		public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
		{
		}

		public void BeforeCharacterUpdate(float deltaTime)
		{
		}

		public void PostGroundingUpdate(float deltaTime)
		{
		}

		public void AfterCharacterUpdate(float deltaTime)
		{
		}

		public bool IsColliderValidForCollisions(Collider coll)
		{
			return true;
		}

		public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
		{
		}

		public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
			ref HitStabilityReport hitStabilityReport)
		{
		}

		public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition,
			Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
		{
		}

		public void OnDiscreteCollisionDetected(Collider hitCollider)
		{
		}
		
		#endregion ICharacterController

	}
}
