using System;
using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController;
using UnityEngine;

namespace TOJam.FLR
{
    public class LevelController : MonoBehaviour
    {
        // [SerializeField] private KinematicCharacterMotor _characterMotorPrefab;
        // [SerializeField] private KinematicCharacterMotor _ghostMotorPrefab;

        [SerializeField] private Transform _levelTransform;
        [SerializeField] private KinematicCharacterMotor _characterMotor;
        
        private void Start()
        {
            //_characterMotor = KinematicMotorManager.Instance.SpawnMotor(_characterMotorPrefab, transform.position, transform.rotation, _levelTransform);
            GameManager.Instance.BeginPreGame();
            StartRun();
        }

        private void Update()
        {
            
        }

        public void StartRun()
        {
            Debug.Log("Start Run");
            GameManager.Instance.BeginGame();
        }

        public void FinishRun()
        {
            Debug.Log("Finish Run");
            GameManager.Instance.BeginPostGame();
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            
            Gizmos.color = Color.blue;

            var start = transform.position;
            start.y += 2.5f;

            var end = _characterMotor.TransientPosition;
            end.y += 2.5f;
            
            Gizmos.DrawLine(start, end);
        }
    }
}
