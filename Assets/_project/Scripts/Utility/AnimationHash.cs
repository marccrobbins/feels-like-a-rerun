using UnityEngine;

namespace TOJam.FLR
{
    public static class AnimationHash
    {
        private const string FORWARD = "Forward";
        private const string JUMP = "Jump";
        private const string IS_GROUNDED = "IsGrounded";
        private const string IS_SLIDING = "IsSliding";

        private static int _forward = -1;
        public static int Forward => GetHash(ref _forward, FORWARD);
        
        private static int _jump = -1;
        public static int Jump => GetHash(ref _jump, JUMP);
        
        private static int _isGrounded = -1;
        public static int IsGrounded => GetHash(ref _isGrounded, IS_GROUNDED);
        
        private static int _isSliding = -1;
        public static int IsSliding => GetHash(ref _isSliding, IS_SLIDING);
        
        private static int GetHash(ref int value, string name)
        {
            if (value > 0) return value;
            return value = Animator.StringToHash(name);
        }
    }
}
