using UnityEngine;

namespace BIMOS
{
    public class AnimationRig : MonoBehaviour
    {
        public static AnimationRig Instance { get; private set;}
        public Feet Feet;

        [Header("Transforms")]
        public Transform CharacterTransform;
        public Transform ArmatureTransform;
        public Transform HipsTransform;
        public Transform LeftFootAnchorTransform;
        public Transform LeftFootTargetTransform;
        public Transform RightFootAnchorTransform;
        public Transform RightFootTargetTransform;
        public Transform HipIKTransform;

        void Start()
        {
            Instance = this;
        }
    }
}
