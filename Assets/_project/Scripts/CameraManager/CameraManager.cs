using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WobblyGamerStudios;

namespace TOJam.FLR
{
    public class CameraManager : Manager<CameraManager>
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private bool _retroactivelyUpdatePoint;

        public Camera MainCamera => _mainCamera ? _mainCamera : Camera.main;
        public GameObject CameraObject => MainCamera.gameObject;
        public Transform CameraTransform => MainCamera.transform;

        private List<CameraPoint> _cameraPoints;
        private CameraPoint _activePoint;
        
        protected override IEnumerator InitializeManager()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            
            _cameraPoints = new List<CameraPoint>();
            
            return base.InitializeManager();
        }

        public void RegisterPoint(CameraPoint point)
        {
            if (_cameraPoints.Contains(point)) return;
            _cameraPoints.Add(point);

            if (point.MoveImmediately)
            {
                MoveToPoint(point);
            }
        }

        public void UnRegisterPoint(CameraPoint point)
        {
            if (!_cameraPoints.Contains(point)) return;
            _cameraPoints.Remove(point);

            if (!_retroactivelyUpdatePoint) return;
            if (_cameraPoints.Count == 0) return;
            if (point == _activePoint)
            {
                MoveToPoint(_cameraPoints[0]);
            }
        }

        public void MoveToPoint(int index)
        {
        }

        public void MoveToPoint(CameraPoint point)
        {
            CameraTransform.SetPositionAndRotation(point.Position, point.Rotation);
            _activePoint = point;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var current = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (!scene.IsValid() || scene == current) return;
            _cameraPoints?.Clear();
        }
    }
}
