using UnityEngine;

namespace BIMOS
{
    [AddComponentMenu("BIMOS/Grabs/Grab (Snap)")]
    public class SnapGrab : Grab
    {
        public override float CalculateRank(Transform handTransform)
        {
            return base.CalculateRank(handTransform) * 3f;
        }

        public override void AlignHand(Hand hand)
        {
            hand.PhysicsHandTransform.SetPositionAndRotation(
                transform.TransformPoint(hand.PalmTransform.InverseTransformPoint(hand.PhysicsHandTransform.position)),
                transform.rotation * Quaternion.Inverse(hand.PalmTransform.rotation) * hand.PhysicsHandTransform.rotation
            );
        }
    }
}