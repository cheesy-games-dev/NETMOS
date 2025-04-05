using UnityEngine;

namespace BIMOS
{
    public class HandAnimator : MonoBehaviour
    {
        public bool IsLeftHand;

        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private HandInputReader _handInputReader;

        [SerializeField]
        private Transform _handTarget;

        private Transform _hand;

        public HandPose DefaultHandPose, HandPose;

        public Transform[]
            Thumb,
            Index,
            Middle,
            Ring,
            Little;

        public thumbSubPoses ThumbPose = thumbSubPoses.Idle;
        public enum thumbSubPoses
        {
            Idle,
            ThumbrestTouched,
            PrimaryTouched,
            PrimaryButton,
            SecondaryTouched,
            SecondaryButton,
            ThumbstickTouched
        };
        public bool IsIndexOnTrigger;
        public float
            IndexCurl,
            MiddleCurl,
            RingCurl,
            LittleCurl;

        private void Awake()
        {
            if (IsLeftHand)
            {
                _hand = _animator.GetBoneTransform(HumanBodyBones.LeftHand);
                Thumb[0] = _animator.GetBoneTransform(HumanBodyBones.LeftThumbProximal);
                Thumb[1] = _animator.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate);
                Thumb[2] = _animator.GetBoneTransform(HumanBodyBones.LeftThumbDistal);
                Index[0] = _animator.GetBoneTransform(HumanBodyBones.LeftIndexProximal);
                Index[1] = _animator.GetBoneTransform(HumanBodyBones.LeftIndexIntermediate);
                Index[2] = _animator.GetBoneTransform(HumanBodyBones.LeftIndexDistal);
                Middle[0] = _animator.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);
                Middle[1] = _animator.GetBoneTransform(HumanBodyBones.LeftMiddleIntermediate);
                Middle[2] = _animator.GetBoneTransform(HumanBodyBones.LeftMiddleDistal);
                Ring[0] = _animator.GetBoneTransform(HumanBodyBones.LeftRingProximal);
                Ring[1] = _animator.GetBoneTransform(HumanBodyBones.LeftRingIntermediate);
                Ring[2] = _animator.GetBoneTransform(HumanBodyBones.LeftRingDistal);
                Little[0] = _animator.GetBoneTransform(HumanBodyBones.LeftLittleProximal);
                Little[1] = _animator.GetBoneTransform(HumanBodyBones.LeftLittleIntermediate);
                Little[2] = _animator.GetBoneTransform(HumanBodyBones.LeftLittleDistal);
            }
            else
            {
                _hand = _animator.GetBoneTransform(HumanBodyBones.RightHand);
                Thumb[0] = _animator.GetBoneTransform(HumanBodyBones.RightThumbProximal);
                Thumb[1] = _animator.GetBoneTransform(HumanBodyBones.RightThumbIntermediate);
                Thumb[2] = _animator.GetBoneTransform(HumanBodyBones.RightThumbDistal);
                Index[0] = _animator.GetBoneTransform(HumanBodyBones.RightIndexProximal);
                Index[1] = _animator.GetBoneTransform(HumanBodyBones.RightIndexIntermediate);
                Index[2] = _animator.GetBoneTransform(HumanBodyBones.RightIndexDistal);
                Middle[0] = _animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
                Middle[1] = _animator.GetBoneTransform(HumanBodyBones.RightMiddleIntermediate);
                Middle[2] = _animator.GetBoneTransform(HumanBodyBones.RightMiddleDistal);
                Ring[0] = _animator.GetBoneTransform(HumanBodyBones.RightRingProximal);
                Ring[1] = _animator.GetBoneTransform(HumanBodyBones.RightRingIntermediate);
                Ring[2] = _animator.GetBoneTransform(HumanBodyBones.RightRingDistal);
                Little[0] = _animator.GetBoneTransform(HumanBodyBones.RightLittleProximal);
                Little[1] = _animator.GetBoneTransform(HumanBodyBones.RightLittleIntermediate);
                Little[2] = _animator.GetBoneTransform(HumanBodyBones.RightLittleDistal);
            }
        }

        private void Update()
        {
            UpdateCurls();
            UpdateHand();
        }

        private void LateUpdate()
        {
            _hand.SetPositionAndRotation(_handTarget.position, _handTarget.rotation);
        }

        private void UpdateCurls()
        {
            if (_handInputReader.SecondaryButton)
            {
                ThumbPose = thumbSubPoses.SecondaryButton;
            }
            else if (_handInputReader.PrimaryButton)
            {
                ThumbPose = thumbSubPoses.PrimaryButton;
            }
            else if (_handInputReader.SecondaryTouched)
            {
                ThumbPose = thumbSubPoses.SecondaryTouched;
            }
            else if (_handInputReader.PrimaryTouched)
            {
                ThumbPose = thumbSubPoses.PrimaryTouched;
            }
            else if (_handInputReader.ThumbstickTouched)
            {
                ThumbPose = thumbSubPoses.ThumbstickTouched;
            }
            else if (_handInputReader.ThumbrestTouched)
            {
                ThumbPose = thumbSubPoses.ThumbrestTouched;
            }
            else
            {
                ThumbPose = thumbSubPoses.Idle;
            }

            IndexCurl = _handInputReader.Trigger;
            IsIndexOnTrigger = _handInputReader.TriggerTouched;
            MiddleCurl = _handInputReader.Grip;
            RingCurl = _handInputReader.Grip;
            LittleCurl = _handInputReader.Grip;
        }

        private void UpdateHand()
        {
            switch (ThumbPose)
            {
                case thumbSubPoses.Idle:
                    UpdateFinger(Thumb, 1, HandPose.Thumb.Idle, HandPose.Thumb.Idle);
                    break;
                case thumbSubPoses.ThumbrestTouched:
                    UpdateFinger(Thumb, 1, HandPose.Thumb.ThumbrestTouched, HandPose.Thumb.ThumbrestTouched);
                    break;
                case thumbSubPoses.PrimaryTouched:
                    UpdateFinger(Thumb, 1, HandPose.Thumb.PrimaryTouched, HandPose.Thumb.PrimaryTouched);
                    break;
                case thumbSubPoses.PrimaryButton:
                    UpdateFinger(Thumb, 1, HandPose.Thumb.PrimaryButton, HandPose.Thumb.PrimaryButton);
                    break;
                case thumbSubPoses.SecondaryTouched:
                    UpdateFinger(Thumb, 1, HandPose.Thumb.SecondaryTouched, HandPose.Thumb.SecondaryTouched);
                    break;
                case thumbSubPoses.SecondaryButton:
                    UpdateFinger(Thumb, 1, HandPose.Thumb.SecondaryButton, HandPose.Thumb.SecondaryButton);
                    break;
                case thumbSubPoses.ThumbstickTouched:
                    UpdateFinger(Thumb, 1, HandPose.Thumb.ThumbstickTouched, HandPose.Thumb.ThumbstickTouched);
                    break;
            }

            if (IsIndexOnTrigger)
            {
                UpdateFinger(Index, IndexCurl, HandPose.Index.TriggerTouched, HandPose.Index.Closed);
            }
            else
            {
                UpdateFinger(Index, IndexCurl, HandPose.Index.Open, HandPose.Index.Closed);
            }

            UpdateFinger(Middle, MiddleCurl, HandPose.Middle.Open, HandPose.Middle.Closed);
            UpdateFinger(Ring, RingCurl, HandPose.Ring.Open, HandPose.Ring.Closed);
            UpdateFinger(Little, LittleCurl, HandPose.Little.Open, HandPose.Little.Closed);
        }

        private void UpdateFinger(Transform[] finger, float value, FingerPose open, FingerPose closed)
        {
            Transform root = finger[0];
            Transform middle = finger[1];
            Transform tip = finger[2];

            if (IsLeftHand)
            {
                open = open.Mirrored();
                closed = closed.Mirrored();
            }

            //Sets dummy finger bone rotations to those in the fingerPose
            root.localRotation = Quaternion.Slerp(root.localRotation, Quaternion.Slerp(open.RootBone, closed.RootBone, value), Time.deltaTime * 25f);
            middle.localRotation = Quaternion.Slerp(middle.localRotation, Quaternion.Slerp(open.MiddleBone, closed.MiddleBone, value), Time.deltaTime * 25f);
            tip.localRotation = Quaternion.Slerp(tip.localRotation, Quaternion.Slerp(open.TipBone, closed.TipBone, value), Time.deltaTime * 25f);
        }
    }
}