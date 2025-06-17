using UnityEngine;

namespace BIMOS {
    public class SlideGrab : LineGrab {
        public float slideLimit = 0.2f;

        public ConfigurableJointMotion xMotion, yMotion, zMotion = ConfigurableJointMotion.Locked;

        private ConfigurableJoint leftHandCJ, rightHandCJ;


        public override void OnGrab(Hand hand) {
            // SM NOTE: Since the grab base class is built with just Fixed Joints in mind,
            // We have to reimplement a few functions in order to accommodate them.
            // This one is basically the same as onGrab, but with a custom slide joint
            // Setup class on it.

            hand.CurrentGrab = this;

            if (hand.IsLeftHand)
                LeftHand = hand;
            else
                RightHand = hand;

            hand.GrabHandler.ApplyGrabPose(HandPose); //Use the hand pose attached

            AlignHand(hand);

            if (hand.IsLeftHand)
                SetupSlideJoint(hand, ref leftHandCJ);
            else
                SetupSlideJoint(hand, ref rightHandCJ);

            IgnoreCollision(hand, true);

            foreach (Grab grab in EnableGrabs) {
                if (grab)
                    grab.enabled = true;
            }
            foreach (Grab grab in DisableGrabs) {
                if (grab)
                    grab.enabled = false;
            }

            GetComponent<Interactable>()?.OnGrab();
        }

        private void FixedUpdate() {
            if (LeftHand)
                UpdateJointMotions(LeftHand, ref leftHandCJ);
            if (RightHand)
                UpdateJointMotions(RightHand, ref rightHandCJ);
        }

        private void UpdateJointMotions(Hand hand, ref ConfigurableJoint joint) {
            if (!hand || !joint)
                return;

            bool triggerDown = hand.HandInputReader.Trigger > 0.5f;

            if (xMotion == ConfigurableJointMotion.Limited)
                joint.xMotion = triggerDown ?
                ConfigurableJointMotion.Locked : ConfigurableJointMotion.Limited;

            if (yMotion == ConfigurableJointMotion.Limited)
                joint.yMotion = triggerDown ?
                ConfigurableJointMotion.Locked : ConfigurableJointMotion.Limited;

            if (zMotion == ConfigurableJointMotion.Limited)
                joint.zMotion = triggerDown ?
                ConfigurableJointMotion.Locked : ConfigurableJointMotion.Limited;
        }

        private void SetupSlideJoint(Hand hand, ref ConfigurableJoint joint) {
            // SM NOTE: All the setup of the required Config Joint happens here.
            // The user can define which axis they want the joint to move on.
            // PLEASE KEEP IN MIND THAT THIS DOES NOT ACCOUNT FOR WHERE THE HAND STARTED GRABBING.

            joint = hand.PhysicsHandTransform.gameObject.AddComponent<ConfigurableJoint>();
            joint.enableCollision = true;

            var rb = transform.GetComponentInParent<Rigidbody>();
            if (rb)
                joint.connectedBody = rb;

            joint.xMotion = xMotion;
            joint.yMotion = yMotion;
            joint.zMotion = zMotion;

            joint.angularXMotion =
                joint.angularYMotion =
                joint.angularZMotion = ConfigurableJointMotion.Locked;

            joint.linearLimit = new() {
                limit = slideLimit
            };
        }

        public override void DestroyGrabJoint(Hand hand) {
            if (!hand)
                return;

            if (hand.IsLeftHand)
                Destroy(leftHandCJ);
            else
                Destroy(rightHandCJ);

            IgnoreCollision(hand, false);

            if (!gameObject.activeSelf)
                return;
        }
    }
}