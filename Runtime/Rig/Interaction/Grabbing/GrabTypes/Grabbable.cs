using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;

namespace KadenZombie8.BIMOS.Rig
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

        protected Rigidbody RigidBody;
        protected ArticulationBody ArticulationBody;
        protected Transform Body;

        [HideInInspector]
        public Collider Collider;

        private readonly float _maxGrabTime = 1f;
        private readonly float _maxPositionDifference = 0.1f;

        private void OnEnable()
        {
            Body = Utilities.GetBody(transform, out RigidBody, out ArticulationBody);
            if (!Body)
            {
                Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
                rigidbody.isKinematic = true;
                RigidBody = rigidbody;
                Body = RigidBody.transform;
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

            AlignHand(hand, out var position, out var rotation);
            StartCoroutine(CreateGrabJoint(hand, position, rotation));

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

            OnGrab?.Invoke();
        }

        public virtual void IgnoreCollision(Hand hand, bool ignore)
        {
            foreach (Collider collider in Body.GetComponentsInChildren<Collider>())
                Physics.IgnoreCollision(collider, hand.PhysicsHandCollider, ignore);
        }

        public virtual void AlignHand(Hand hand, out Vector3 position, out Quaternion rotation)
        {
            position = hand.PalmTransform.position;
            rotation = hand.PalmTransform.rotation;
        }

        private IEnumerator CreateGrabJoint(Hand hand, Vector3 position, Quaternion rotation)
        {
            var tempRotation = hand.PhysicsHandTransform.rotation;

            hand.PhysicsHandTransform.rotation = rotation;
            //hand.PhysicsHandTransform.position = position; // TODO: REMOVE
            var grabJoint = hand.PhysicsHandTransform.gameObject.AddComponent<ConfigurableJoint>();
            hand.PhysicsHand.Rigidbody.rotation = tempRotation;

            hand.GrabJoint = grabJoint;

            //grabJoint.enableCollision = true;
            //grabJoint.enablePreprocessing = false;
            //grabJoint.projectionMode = JointProjectionMode.PositionAndRotation;

            grabJoint.xMotion
               = grabJoint.yMotion
               = grabJoint.zMotion
               = ConfigurableJointMotion.Locked;
            grabJoint.rotationDriveMode = RotationDriveMode.Slerp;
            grabJoint.slerpDrive = new() { positionSpring = Mathf.Infinity, maximumForce = Mathf.Infinity };

            if (RigidBody)
                grabJoint.connectedBody = RigidBody;
            if (ArticulationBody)
                grabJoint.connectedArticulationBody = ArticulationBody;

            //grabJoint.autoConfigureConnectedAnchor = false;
            grabJoint.anchor = hand.PalmTransform.localPosition;

            hand.PalmTransform.GetPositionAndRotation(out var initialPosition, out var initialRotation);
            //var initialLocalPosition = Body.InverseTransformPoint(initialPosition);
            var initialLocalRotation = Quaternion.Inverse(Body.rotation) * initialRotation;
            //var targetLocalPosition = transform.InverseTransformPoint(position);
            var targetLocalRotation = Quaternion.Inverse(Body.rotation) * rotation;

            var elapsedTime = 0f;
            var positionDifference = Mathf.Min(
                Vector3.Distance(initialPosition, position), _maxPositionDifference)
                / _maxPositionDifference;
            var rotationDifference = (-Quaternion.Dot(initialRotation, rotation) + 1f) / 2f;
            var averageDifference = (positionDifference + rotationDifference) / 2f;
            var grabTime = 2f; // _maxGrabTime * averageDifference;
            while (elapsedTime < grabTime)
            {
                if (!grabJoint)
                    yield break;

                //var initialWorldPosition = transform.TransformPoint(initialLocalPosition);
                var initialWorldRotation = Body.rotation * initialLocalRotation;

                //var targetWorldPosition = transform.TransformPoint(targetLocalPosition);
                var targetWorldRotation = Body.rotation * targetLocalRotation;

                //var lerpedTargetPosition = Vector3.Lerp(initialWorldPosition, targetWorldPosition, elapsedTime / grabTime);
                var lerpedTargetRotation = Quaternion.Lerp(initialLocalRotation, Quaternion.identity, elapsedTime / grabTime);

                //grabJoint.connectedAnchor = transform.InverseTransformPoint(lerpedTargetPosition);
                grabJoint.targetRotation = lerpedTargetRotation;

                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            
            //var targetWorldPosition2 = transform.TransformPoint(targetLocalPosition);
            //var targetWorldRotation2 = transform.rotation * targetLocalRotation;

            //grabJoint.connectedAnchor = transform.localPosition;
            grabJoint.targetRotation = Quaternion.identity;

            yield return null; //TODO: Remove
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