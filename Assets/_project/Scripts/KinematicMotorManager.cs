using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController;
using UnityEngine;
using WobblyGamerStudios;

namespace TOJam.FLR
{
    public delegate void CharacterMotorDelegate(KinematicCharacterMotor motor);
    public delegate void PhysicsMoverDelegate(PhysicsMover mover);
    
    public class KinematicMotorManager : Manager<KinematicMotorManager>
    {
        public List<KinematicCharacterMotor> CharacterMotors { get; private set; } = new List<KinematicCharacterMotor>();
        public  List<PhysicsMover> PhysicsMovers { get; private set; } = new List<PhysicsMover>();
        
        private KCCSettings _settings;
        private static float _lastCustomInterpolationStartTime = -1f;
        private static float _lastCustomInterpolationDeltaTime = -1f;
        
        protected override IEnumerator InitializeManager()
        {
            _settings = ScriptableObject.CreateInstance<KCCSettings>();
            
            return base.InitializeManager();
        }

        public KinematicCharacterMotor SpawnMotor(KinematicCharacterMotor motorPrefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            var motor = Instantiate(motorPrefab, position, rotation, parent);
            CharacterMotors.Add(motor);
            return motor;
        }
        
        #region Registration

        public void RegisterMotor(KinematicCharacterMotor motor)
        {
            if (CharacterMotors.Contains(motor)) return;
            CharacterMotors.Add(motor);
            OnMotorRegistered?.Invoke(motor);
        }
        
        public void UnRegisterMotor(KinematicCharacterMotor motor)
        {
            if (!CharacterMotors.Contains(motor)) return;
            CharacterMotors.Remove(motor);
            OnMotorUnRegistered?.Invoke(motor);
        }
        
        public void RegisterMover(PhysicsMover mover)
        {
            if (PhysicsMovers.Contains(mover)) return;
            PhysicsMovers.Add(mover); 
            OnPhysicsMoverRegistered?.Invoke(mover);
        }
        
        public void UnRegisterMover(PhysicsMover mover)
        {
            if (!PhysicsMovers.Contains(mover)) return;
            PhysicsMovers.Remove(mover);
            OnPhysicsMoverUnRegistered?.Invoke(mover);
        }
        
        #endregion Registration
        
        private void FixedUpdate()
        {
            if (!IsInitialized) return;
            
            if (_settings.AutoSimulation)
            {
                float deltaTime = Time.deltaTime;

                if (_settings.Interpolate)
                {
                    PreSimulationInterpolationUpdate(deltaTime);
                }

                Simulate(deltaTime, CharacterMotors, PhysicsMovers);

                if (_settings.Interpolate)
                {
                    PostSimulationInterpolationUpdate(deltaTime);
                }
            }
        }

        private void LateUpdate()
        {
            if (!IsInitialized) return;
            
            if (_settings.Interpolate)
            {
                CustomInterpolationUpdate();
            }
        }

        /// <summary>
        /// Remembers the point to interpolate from for KinematicCharacterMotors and PhysicsMovers
        /// </summary>
        public void PreSimulationInterpolationUpdate(float deltaTime)
        {
            // Save pre-simulation poses and place transform at transient pose
            for (int i = 0; i < CharacterMotors.Count; i++)
            {
                KinematicCharacterMotor motor = CharacterMotors[i];

                motor.InitialTickPosition = motor.TransientPosition;
                motor.InitialTickRotation = motor.TransientRotation;

                motor.Transform.SetPositionAndRotation(motor.TransientPosition, motor.TransientRotation);
            }

            // for (int i = 0; i < PhysicsMovers.Count; i++)
            // {
            //     PhysicsMover mover = PhysicsMovers[i];
            //
            //     mover.InitialTickPosition = mover.TransientPosition;
            //     mover.InitialTickRotation = mover.TransientRotation;
            //
            //     mover.Transform.SetPositionAndRotation(mover.TransientPosition, mover.TransientRotation);
            //     mover.Rigidbody.position = mover.TransientPosition;
            //     mover.Rigidbody.rotation = mover.TransientRotation;
            // }
        }

        /// <summary>
        /// Ticks characters and/or movers
        /// </summary>
        public void Simulate(float deltaTime, List<KinematicCharacterMotor> motors, List<PhysicsMover> movers)
        {
            int characterMotorsCount = motors.Count;
            int physicsMoversCount = movers.Count;

#pragma warning disable 0162
            // Update PhysicsMover velocities
            // for (int i = 0; i < physicsMoversCount; i++)
            // {
            //     movers[i].VelocityUpdate(deltaTime);
            // }

            // Character controller update phase 1
            for (int i = 0; i < characterMotorsCount; i++)
            {
                motors[i].UpdatePhase1(deltaTime);
            }

            // Simulate PhysicsMover displacement
            // for (int i = 0; i < physicsMoversCount; i++)
            // {
            //     PhysicsMover mover = movers[i];
            //
            //     mover.Transform.SetPositionAndRotation(mover.TransientPosition, mover.TransientRotation);
            //     mover.Rigidbody.position = mover.TransientPosition;
            //     mover.Rigidbody.rotation = mover.TransientRotation;
            // }

            // Character controller update phase 2 and move
            for (int i = 0; i < characterMotorsCount; i++)
            {
                KinematicCharacterMotor motor = motors[i];

                motor.UpdatePhase2(deltaTime);

                motor.Transform.SetPositionAndRotation(motor.TransientPosition, motor.TransientRotation);
            }
#pragma warning restore 0162
        }

        /// <summary>
        /// Initiates the interpolation for KinematicCharacterMotors and PhysicsMovers
        /// </summary>
        public void PostSimulationInterpolationUpdate(float deltaTime)
        {
            _lastCustomInterpolationStartTime = Time.time;
            _lastCustomInterpolationDeltaTime = deltaTime;

            // Return interpolated roots to their initial poses
            for (int i = 0; i < CharacterMotors.Count; i++)
            {
                KinematicCharacterMotor motor = CharacterMotors[i];

                motor.Transform.SetPositionAndRotation(motor.InitialTickPosition, motor.InitialTickRotation);
            }

            // for (int i = 0; i < PhysicsMovers.Count; i++)
            // {
            //     PhysicsMover mover = PhysicsMovers[i];
            //
            //     if (mover.MoveWithPhysics)
            //     {
            //         mover.Rigidbody.position = mover.InitialTickPosition;
            //         mover.Rigidbody.rotation = mover.InitialTickRotation;
            //
            //         mover.Rigidbody.MovePosition(mover.TransientPosition);
            //         mover.Rigidbody.MoveRotation(mover.TransientRotation);
            //     }
            //     else
            //     {
            //         mover.Rigidbody.position = (mover.TransientPosition);
            //         mover.Rigidbody.rotation = (mover.TransientRotation);
            //     }
            //}
        }

        /// <summary>
        /// Handles per-frame interpolation
        /// </summary>
        private void CustomInterpolationUpdate()
        {
            float interpolationFactor = Mathf.Clamp01((Time.time - _lastCustomInterpolationStartTime) / _lastCustomInterpolationDeltaTime);

            // Handle characters interpolation
            for (int i = 0; i < CharacterMotors.Count; i++)
            {
                KinematicCharacterMotor motor = CharacterMotors[i];

                motor.Transform.SetPositionAndRotation(
                    Vector3.Lerp(motor.InitialTickPosition, motor.TransientPosition, interpolationFactor),
                    Quaternion.Slerp(motor.InitialTickRotation, motor.TransientRotation, interpolationFactor));
            }

            // Handle PhysicsMovers interpolation
            // for (int i = 0; i < PhysicsMovers.Count; i++)
            // {
            //     PhysicsMover mover = PhysicsMovers[i];
            //     
            //     mover.Transform.SetPositionAndRotation(
            //         Vector3.Lerp(mover.InitialTickPosition, mover.TransientPosition, interpolationFactor),
            //         Quaternion.Slerp(mover.InitialTickRotation, mover.TransientRotation, interpolationFactor));
            //
            //     Vector3 newPos = mover.Transform.position;
            //     Quaternion newRot = mover.Transform.rotation;
            //     mover.PositionDeltaFromInterpolation = newPos - mover.LatestInterpolationPosition;
            //     mover.RotationDeltaFromInterpolation = Quaternion.Inverse(mover.LatestInterpolationRotation) * newRot;
            //     mover.LatestInterpolationPosition = newPos;
            //     mover.LatestInterpolationRotation = newRot;
            // }
        }

        #region Events

        public CharacterMotorDelegate OnMotorRegistered;
        public CharacterMotorDelegate OnMotorUnRegistered;

        public PhysicsMoverDelegate OnPhysicsMoverRegistered;
        public PhysicsMoverDelegate OnPhysicsMoverUnRegistered;

        #endregion
    }
}
