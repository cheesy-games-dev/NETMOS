using UnityEngine;

namespace BIMOS
{
    [AddComponentMenu("BIMOS/Grabs/Grab (Auto)")]
    public class AutoGrab : Grab
    {
        public override void AlignHand(Hand hand)
        {
            Vector3 handTargetPosition = GetComponent<Collider>().ClosestPoint(hand.PalmTransform.position);
            Vector3 handToTargetDirection = (handTargetPosition - hand.PalmTransform.position).normalized;

            Ray ray = new Ray(hand.PalmTransform.position, handToTargetDirection);
            RaycastHit hit;

            if (GetComponent<Collider>().Raycast(ray, out hit, 10f))
            {
                Vector3 projected = Vector3.ProjectOnPlane(-hand.PalmTransform.up, hit.normal).normalized;
                hand.PhysicsHandTransform.position = handTargetPosition;
                Vector3 crossed = Vector3.Cross(hit.normal, projected).normalized;
                hand.PhysicsHandTransform.rotation = Quaternion.LookRotation(-crossed, -projected);
                hand.PhysicsHandTransform.Rotate(180f, 90f, 180f);
                hand.PhysicsHandTransform.position += hit.normal * 0.02f; // Moves hand out of collider
            }

            hand.PhysicsHandTransform.position = hand.PhysicsHandTransform.TransformPoint(hand.PalmTransform.InverseTransformPoint(hand.PhysicsHandTransform.position));
            hand.PhysicsHandTransform.rotation = hand.PhysicsHandTransform.rotation * Quaternion.Inverse(hand.PalmTransform.rotation) * hand.PhysicsHandTransform.rotation;
        }

        public override void IgnoreCollision(Hand hand, bool ignore) { }
    }
}