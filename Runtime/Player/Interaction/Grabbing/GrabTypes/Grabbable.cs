using System;
using UnityEngine;

namespace BIMOS
{
    [AddComponentMenu("BIMOS/Grabbables/Grabbable (Basic)")]
    public class Grabbable : MonoBehaviour
    {
        public event Action OnGrab;
        public event Action OnRelease;
        public HandPose HandPose;
        public bool IsLeftHanded = true, IsRightHanded = true;
        public Grabbable[] EnableGrabs, DisableGrabs;

        [HideInInspector]
        public Hand LeftHand, RightHand;

        private Rigidbody _rigidBody;
        private ArticulationBody _articulationBody;
        private Transform _body;

        [HideInInspector]
        public Collider Collider;

        private void OnEnable()
        {
            _body = Utilities.GetBody(transform, out _rigidBody, out _articulationBody);
            if (!_body)
            {
                Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
                rigidbody.isKinematic = true;
                _rigidBody = rigidbody;
                _body = _rigidBody.transform;
            }

            Collider = GetComponent<Collider>();
            if (Collider)
                return;

            CreateCollider();
        }

        public virtual void CreateCollider()
        {
            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 0.01f;
            Collider = collider;
        }

        public virtual float CalculateRank(Transform handTransform) //Returned when in player grab range
        {
            if (Collider is MeshCollider)
                return 1f/1000f;

            return 1f / Vector3.Distance(handTransform.position, Collider.ClosestPoint(handTransform.position)); //Reciprocal of distance from hand to grab
        }

        public virtual void Grab(Hand hand) //Triggered when player grabs the grab
        {
            hand.CurrentGrab = this;

            if (hand.IsLeftHand)
                LeftHand = hand;
            else
                RightHand = hand;

            hand.GrabHandler.ApplyGrabPose(HandPose); //Use the hand pose attached

            AlignHand(hand);
            CreateGrabJoint(hand);

            IgnoreCollision(hand, true);

            foreach (Grabbable grab in EnableGrabs)
            {
                if (grab)
                    grab.enabled = true;
            }
            foreach (Grabbable grab in DisableGrabs)
            {
                if (grab)
                    grab.enabled = false;
            }

            if (TryGetComponent<Interactable>(out var interactable))
                interactable.OnRelease();

            OnGrab?.Invoke();
        }

        public virtual void IgnoreCollision(Hand hand, bool ignore)
        {
            foreach (Collider collider in _body.GetComponentsInChildren<Collider>())
                Physics.IgnoreCollision(collider, hand.PhysicsHandCollider, ignore);
        }

        public virtual void AlignHand(Hand hand) { }

        private void CreateGrabJoint(Hand hand)
        {
            var grabJoint = hand.PhysicsHandTransform.gameObject.AddComponent<ConfigurableJoint>();

            grabJoint.enableCollision = true;
            grabJoint.enablePreprocessing = false;
            if (_rigidBody)
                grabJoint.connectedBody = _rigidBody;
            if (_articulationBody)
                grabJoint.connectedArticulationBody = _articulationBody;

            grabJoint.xMotion
                = grabJoint.yMotion
                = grabJoint.zMotion
                = grabJoint.xMotion
                = grabJoint.angularXMotion
                = grabJoint.angularYMotion
                = grabJoint.angularZMotion
                = ConfigurableJointMotion.Locked;

            grabJoint.projectionMode = JointProjectionMode.PositionAndRotation;

            hand.GrabJoint = grabJoint;
        }

        public void Release(Hand hand, bool toggleGrabs) //Triggered when player releases the grab
        {
            if (!hand)
                return;

            DestroyGrabJoint(hand);

            if (toggleGrabs)
            {
                foreach (Grabbable grab in EnableGrabs)
                {
                    if (grab)
                        grab.enabled = false;
                }
                foreach (Grabbable grab in DisableGrabs)
                {
                    if (grab)
                        grab.enabled = true;
                }
            }

            if (TryGetComponent<Interactable>(out var interactable))
                interactable.OnRelease();

            hand.CurrentGrab = null;

            if (hand.IsLeftHand)
                LeftHand = null;
            else
                RightHand = null;

            OnRelease?.Invoke();
        }

        public virtual void DestroyGrabJoint(Hand hand)
        {
            if (!hand)
                return;

            if (hand.GrabJoint)
                Destroy(hand.GrabJoint); //Deletes the joint, letting it go

            IgnoreCollision(hand, false);

            if (!gameObject.activeSelf)
                return;
        }

        private void OnDisable()
        {
            Release(LeftHand, false);
            Release(RightHand, false);
        }
    }
}