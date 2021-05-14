using UnityEngine;

namespace TOJam.FLR
{
    public class CameraPoint : MonoBehaviour
    {
        [SerializeField] private bool _moveImmediately;
        public bool MoveImmediately => _moveImmediately;

        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;

        protected virtual void OnEnable()
        {
            CameraManager.Instance.RegisterPoint(this);
        }

        protected virtual void OnDisable()
        { 
            CameraManager.Instance.UnRegisterPoint(this);
        }
    }
}
