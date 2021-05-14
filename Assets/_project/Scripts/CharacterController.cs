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

		[Header("Camera")] 
		[SerializeField] private Camera _camera;
		[SerializeField] private Transform _cameraTarget;
		[SerializeField] private float _distanceBeforeFollow;
		
		[Header("GroundMovement")]
		[SerializeField] private float _moveSpeed = 5;
		[SerializeField] private float _orientSpeed = 20;
		
		[Header("AirMovement")]
		[SerializeField] private Vector3 _gravity = new Vector3 {y = -30};
		[SerializeField] private float _airSpeed;
		[SerializeField] private float _airAcceleration;
		
		[Header("Jump")]
		[SerializeField] private float _jumpPower;
		[SerializeField] private float _extraForwardJumpPower = 5;
		[SerializeField] private float _wallJumpPower;
		[SerializeField] private float _postJumpGracePeriod;

		[Header("Sliding")] 
		[SerializeField, Range(0, 90)] private float _slidingSlopeAngle = 15;
		[SerializeField] private float _slidingMoveSpeedModifier;
		
		[SerializeField] private InputActionProperty _moveAction;
		[SerializeField] private InputActionProperty _jumpAction;
		[SerializeField] private InputActionProperty _slideAction;
		[SerializeField] private InputActionProperty _specialAction;

		public bool IsGrounded => _motor.GroundingStatus.FoundAnyGround;

		//Movement
		private Vector2 _currentMoveInput;
		private Vector3 _currentMoveVector;
		private Vector3 _cameraLookVector;
		private Quaternion _cameraRotation;
		private Vector3 _moveDirectionVector;
		private Vector3 _lookDirectionVector;
		
		//Jump
		private bool _isJumpPressed;
		private bool _wasJumpPressed;
		private bool _hasJumpedThisFrame;
		private bool _isJumpRequested;
		private bool _hasJumped;
		private bool _hasDoubleJumped;
		private float _timeSinceLastAbleToJump;
		private bool _setJumpInAnimator;
		private bool _canWallJump;
		private Vector3 _wallHitNormal;
		private Vector3 _wallHitPoint;
		
		//Sliding
		private bool _isSliding;
		
		//Special
		private bool _isSpecialPressed;

		#region MonoBehaviour

		private void OnEnable()
		{
			_motor.CharacterController = this;
			KinematicMotorManager.Instance.RegisterMotor(_motor);
		}

		private void OnDisable()
		{
			KinematicMotorManager.Instance.UnRegisterMotor(_motor);
		}

		private void Update()
		{
			ProcessInput();
			UpdateAnimation();

			var distance = Vector3.Distance(_motor.TransientPosition, _cameraTarget.position);
			if (distance >= _distanceBeforeFollow && _motor.TransientPosition.z >= _cameraTarget.position.z)
			{
				var target = _motor.TransientPosition;
				target.x = 0;
				_cameraTarget.position = Vector3.MoveTowards(_cameraTarget.position, target, Time.deltaTime * 15);
			}
		}
		
		#endregion MonoBehaviouor

		#region Input

		private void ProcessInput()
		{
			ProcessMoveInput();
			ProcessJumpInput();
			ProcessSlideInput();
			ProcessSpecialInput();
		}

		private void ProcessMoveInput()
		{
			_currentMoveInput = _moveAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
			_currentMoveVector = Vector3.ClampMagnitude(new Vector3(_currentMoveInput.x, 0, _currentMoveInput.y), 1);

			//Calculate camera orientation
			_cameraLookVector = Vector3.ProjectOnPlane(_camera.transform.rotation * Vector3.forward, _motor.CharacterUp)
				.normalized;
			if (_cameraLookVector.sqrMagnitude == 0f)
			{
				_cameraLookVector = Vector3.ProjectOnPlane(_camera.transform.rotation * Vector3.up, _motor.CharacterUp)
					.normalized;
			}

			_cameraRotation = Quaternion.LookRotation(_cameraLookVector, _motor.CharacterUp);

			//Calculate move direction using input and camera orientation
			_moveDirectionVector = _cameraRotation * _currentMoveVector;
			_lookDirectionVector = _moveDirectionVector.normalized;
		}

		private void ProcessJumpInput()
		{
			_isJumpPressed = IsActionDown(in _jumpAction);
			if (_isJumpPressed && !_wasJumpPressed && (!_hasJumped || !_hasDoubleJumped))
			{
				_isJumpRequested = true;
			}

			_wasJumpPressed = _isJumpPressed;
		}

		private void ProcessSlideInput()
		{
			if (IsActionDown(in _slideAction))
			{
				//ToDo check if can slide

				if (!IsOnDownwardSlope()) return;
				
				_isSliding = true;
			}
			else
			{
				_isSliding = false;
			}
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
			if (_lookDirectionVector.sqrMagnitude > 0f && _orientSpeed > 0f)
			{
				// Smoothly interpolate from current to target look direction
				var smoothedLookInputDirection = Vector3.Slerp(_motor.CharacterForward, _lookDirectionVector,
					1 - Mathf.Exp(-_orientSpeed * deltaTime)).normalized;

				// Set the current rotation (which will be used by the KinematicCharacterMotor)
				currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, _motor.CharacterUp);
			}

			var currentUp = currentRotation * Vector3.up;
			Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -_gravity.normalized,
				1 - Mathf.Exp(-_orientSpeed * deltaTime));
			currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
		}

		public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
		{
			Vector3 targetMovementVelocity = Vector3.zero;
            if (_motor.GroundingStatus.IsStableOnGround)
            {
                // Reorient velocity on slope
                currentVelocity = _motor.GetDirectionTangentToSurface(currentVelocity, _motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;

                // Calculate target velocity
                Vector3 inputRight = Vector3.Cross(_moveDirectionVector, _motor.CharacterUp);
                Vector3 reorientedInput = Vector3.Cross(_motor.GroundingStatus.GroundNormal, inputRight).normalized * _moveDirectionVector.magnitude;
                targetMovementVelocity = reorientedInput * GetMoveSpeed();

                // Smooth movement Velocity
                currentVelocity = targetMovementVelocity;
            }
            else
            {
                // Add move input
                if (_moveDirectionVector.sqrMagnitude > 0f)
                {
                    targetMovementVelocity = _moveDirectionVector * _airSpeed;

                    // Prevent climbing on un-stable slopes with air movement
                    if (_motor.GroundingStatus.FoundAnyGround)
                    {
                        Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(_motor.CharacterUp, _motor.GroundingStatus.GroundNormal), _motor.CharacterUp).normalized;
                        targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity, perpenticularObstructionNormal);
                    }

                    Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, _gravity);
                    currentVelocity += velocityDiff * (_airAcceleration * deltaTime);
                }

                // Gravity
                currentVelocity += _gravity * deltaTime;
            }

            // Handle jumping
            _hasJumpedThisFrame = false;
            _timeSinceLastAbleToJump += deltaTime;
            if (_isJumpRequested)
            {
	            // See if we actually are allowed to jump
	            if (_canWallJump || (!_hasJumped && _motor.GroundingStatus.FoundAnyGround || _timeSinceLastAbleToJump <= _postJumpGracePeriod))
	            {
		            // Calculate jump direction before ungrounding
		            Vector3 jumpDirection = _motor.CharacterUp;
		            float jumpPower = _jumpPower;
		            if (_canWallJump)
		            {
			            jumpDirection = _wallHitNormal;
			            jumpPower = _wallJumpPower;
		            }
		            else if (_motor.GroundingStatus.FoundAnyGround && !_motor.GroundingStatus.IsStableOnGround)
		            {
			            jumpDirection = _motor.GroundingStatus.GroundNormal;
		            }

		            // Makes the character skip ground probing/snapping on its next update. 
		            // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
		            _motor.ForceUnground(0.1f);

		            // Add to the return velocity and reset jump state
		            currentVelocity += jumpDirection * jumpPower - Vector3.Project(currentVelocity, _motor.CharacterUp);
		            currentVelocity += _moveDirectionVector * _extraForwardJumpPower;
		            _isJumpRequested = false;
		            _hasJumped = true;
		            _setJumpInAnimator = true;
		            _hasJumpedThisFrame = true;
	            }
            }

            _canWallJump = false;
		}

		public void BeforeCharacterUpdate(float deltaTime)
		{
		}

		public void PostGroundingUpdate(float deltaTime)
		{
		}

		public void AfterCharacterUpdate(float deltaTime)
		{
			// Handle jumping pre-ground grace period
			_isJumpRequested = false;

			if (IsGrounded)
			{
				// If we're on a ground surface, reset jumping values
				if (!_hasJumpedThisFrame)
				{
					_hasDoubleJumped = false;
					_hasJumped = false;
				}
				_timeSinceLastAbleToJump = 0f;
			}
			else
			{
				// Keep track of time since we were last able to jump (for grace period)
				_timeSinceLastAbleToJump += deltaTime;
			}

			if (_isSliding && !IsOnDownwardSlope()) _isSliding = false;
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
			if (!_motor.GroundingStatus.IsStableOnGround && !hitStabilityReport.IsStable)
			{
				_canWallJump = true;
				_wallHitNormal = Vector3.Lerp(hitNormal, _motor.CharacterUp, 0.5f);
			}
		}

		public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition,
			Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
		{
		}

		public void OnDiscreteCollisionDetected(Collider hitCollider)
		{
		}
		
		#endregion ICharacterController

		private void UpdateAnimation()
		{
			_animator.SetFloat(AnimationHash.Forward, _moveDirectionVector.magnitude);
			_animator.SetBool(AnimationHash.IsGrounded, IsGrounded);

			if (_setJumpInAnimator)
			{
				_animator.SetTrigger(AnimationHash.Jump);
				_setJumpInAnimator = false;
			}

			_animator.SetBool(AnimationHash.IsSliding, _isSliding);
		}

		private float GetMoveSpeed()
		{
			var speed = _moveSpeed;
			if (_isSliding && _slidingMoveSpeedModifier >= 1) speed *= _slidingMoveSpeedModifier;
			return speed;
		}
		
		private bool IsOnDownwardSlope()
		{
			var angle = Vector3.Angle(_motor.CharacterUp, _motor.GroundingStatus.GroundNormal);

			//Check if were going down a slope
			var cross = Vector3.Cross(_motor.GroundingStatus.GroundNormal, Vector3.Cross(_moveDirectionVector, _motor.CharacterUp));
			var angle2 = Vector3.Angle(_motor.CharacterUp, cross) - 90;
			
			return angle >= _slidingSlopeAngle && angle2 > 0;
		}

		private void OnDrawGizmos()
		{
			var cross = Vector3.Cross(_motor.GroundingStatus.GroundNormal, Vector3.Cross(_moveDirectionVector, _motor.CharacterUp));
			
			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(_motor.TransientPosition, _motor.TransientPosition + cross * 1);
			
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(_motor.TransientPosition, _motor.TransientPosition + _wallHitNormal * 1);
		}
	}
}
