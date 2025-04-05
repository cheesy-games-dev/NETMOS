using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

namespace BIMOS
{
    public class AnimationRig : MonoBehaviour
    {
        public Feet Feet;

        public AnimationRigTransforms Transforms;

        [Header("Transforms")]

        [SerializeField]
        private AnimationRigConstraints _constraints;

        private Animator _animator;
        private RigBuilder _rigBuilder;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            _rigBuilder = GetComponentInChildren<RigBuilder>();
            UpdateConstraints();
            _rigBuilder.Build();
        }

        private void UpdateConstraints()
        {
            //Transforms.Armature = _animator.avatarRoot;
            // Shoulders
            //_constraints.LeftShoulder.data.constrainedObject = _animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
            //_constraints.RightShoulder.data.constrainedObject = _animator.GetBoneTransform(HumanBodyBones.RightShoulder);

            // Arms
            _constraints.LeftArm.data.root = _animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            _constraints.LeftArm.data.mid = _animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            _constraints.LeftArm.data.tip = _animator.GetBoneTransform(HumanBodyBones.LeftHand);

            _constraints.RightArm.data.root = _animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
            _constraints.RightArm.data.mid = _animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
            _constraints.RightArm.data.tip = _animator.GetBoneTransform(HumanBodyBones.RightHand);

            // Legs
            _constraints.LeftLeg.data.root = _animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
            _constraints.LeftLeg.data.mid = _animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
            _constraints.LeftLeg.data.tip = _animator.GetBoneTransform(HumanBodyBones.LeftFoot);

            _constraints.RightLeg.data.root = _animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
            _constraints.RightLeg.data.mid = _animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
            _constraints.RightLeg.data.tip = _animator.GetBoneTransform(HumanBodyBones.RightFoot);

            // Feet
            //Transforms.LeftFootTarget.localPosition = Vector3.Scale(
            //    avatarPrefabScript.Animator.GetBoneTransform(HumanBodyBones.LeftFoot).position,
            //    new Vector3(0f, 1f, 1f));
            //Transforms.LeftFootTarget.localRotation = _animator.GetBoneTransform(HumanBodyBones.LeftFoot).rotation;

            //Transforms.RightFootTarget.localPosition = Vector3.Scale(
            //    avatarPrefabScript.Animator.GetBoneTransform(HumanBodyBones.RightFoot).position,
            //    new Vector3(0f, 1f, 1f));
            //Transforms.RightFootTarget.localRotation = _animator.GetBoneTransform(HumanBodyBones.RightFoot).rotation;

            //Feet.LeftUpperLeg = _animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
            //Feet.RightUpperLeg = _animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);

            //_constraints.LeftFoot.data.constrainedObject = _animator.GetBoneTransform(HumanBodyBones.LeftToes);
            //_constraints.RightFoot.data.constrainedObject = _animator.GetBoneTransform(HumanBodyBones.RightToes);

            // Head
            var headBone = _animator.GetBoneTransform(HumanBodyBones.Head);
            var neckBone = _animator.GetBoneTransform(HumanBodyBones.Neck);
            if (neckBone)
                headBone = neckBone;

            Transforms.Head = headBone;

            //var oldCameraOffsetPosition = Transforms.HeadCameraOffset.position;
            //Transforms.HeadCameraOffset.localPosition = avatarPrefabScript.EyeCenter.InverseTransformPoint(Transforms.Head.position);
            //Transforms.HeadCameraOffset.localPosition /= _trackingRig.RoomscaleOffset.localScale.x;
            //_trackingRig.RoomscaleOffset.position -= Transforms.HeadCameraOffset.position - oldCameraOffsetPosition;

            //_constraints.Head.data.constrainedObject = Transforms.Head;

            //// Torso
            Transforms.Hips = _animator.GetBoneTransform(HumanBodyBones.Hips);
            _constraints.Hip.data.constrainedObject = Transforms.Hips;

            _constraints.Chest.data.root = Transforms.Hips;
            _constraints.Chest.data.tip = Transforms.Head;
        }
    }

    [Serializable]
    public struct AnimationRigTransforms
    {
        public Transform Character;
        public Transform LeftFootTarget;
        public Transform RightFootTarget;
        public Transform LeftFootAnchor;
        public Transform RightFootAnchor;
        public Transform Hips;
        public Transform HipsIK;
        public Transform Head;
    }

    [Serializable]
    public struct AnimationRigConstraints
    {
        public TwoBoneIKConstraint LeftArm;
        public TwoBoneIKConstraint RightArm;
        public TwoBoneIKConstraint LeftLeg;
        public TwoBoneIKConstraint RightLeg;
        public OverrideTransform Hip;
        public ChainIKConstraint Chest;
        public MultiParentConstraint Head;
    }
}
