using KadenZombie8.BIMOS.Rig;
using System;
using System.Collections;
using UnityEngine;

namespace KadenZombie8.BIMOS.Sockets
{
    public class Socket : MonoBehaviour
    {
        public event Action
            OnAttach,
            OnDetach;

        public string[] Tags;

        public Transform AttachPoint, DetachPoint;

        [SerializeField]
        private float _insertTime = 0.1f;

        [HideInInspector]
        public Attacher Attacher;

        [HideInInspector]
        public ConfigurableJoint AttachJoint;

        private bool _onCooldown;
        private readonly float _cooldownTime = 0.1f;
        private bool _waitingForDetach;
        private readonly float _maxAlignTime = 0.25f;
        private readonly float _maxPositionDifference = 0.1f;

        private Rigidbody _rigidBody;
        private ArticulationBody _articulationBody;
        private Transform _body;

        private void Awake() => _body = Utilities.GetBody(transform, out _rigidBody, out _articulationBody);

        private bool HasMatchingTag(Attacher attacher)
        {
            foreach (string socketTag in Tags)
                foreach (string attacherTag in attacher.Tags)
                    if (socketTag == attacherTag)
                        return true;

            return false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Attacher)
                return;

            Attacher attacher = other.GetComponent<Attacher>();

            if (!attacher)
                return;

            if (!HasMatchingTag(attacher))
                return;

            if (attacher.Socket)
                return;

            Attach(attacher);
        }

        public void Attach(Attacher attacher)
        {
            if (_onCooldown)
                return;

            if (!attacher.IsGrabbed())
                return;

            _onCooldown = true;

            Attacher = attacher;

            Attacher.Socket = this;

            foreach (Collider attacherCollider in Attacher.Rigidbody.GetComponentsInChildren<Collider>())
                foreach (Collider socketCollider in _body.GetComponentsInChildren<Collider>())
                    Physics.IgnoreCollision(attacherCollider, socketCollider, true);

            foreach (Grabbable grab in Attacher.EnableGrabs)
                if (grab)
                    grab.enabled = true;

            foreach (Grabbable grab in Attacher.DisableGrabs)
                if (grab)
                    grab.enabled = false;

            StartCoroutine(AttachCoroutine());
        }

        private IEnumerator AttachCoroutine()
        {
            var attacher = Attacher.transform;

            var rotation = Attacher.Rigidbody.transform.rotation;
            Attacher.Rigidbody.transform.rotation = DetachPoint.rotation * Quaternion.Inverse(attacher.localRotation);

            AttachJoint = Attacher.Rigidbody.gameObject.AddComponent<ConfigurableJoint>();
            AttachJoint.xMotion
               = AttachJoint.yMotion
               = AttachJoint.zMotion
               = ConfigurableJointMotion.Locked;
            AttachJoint.rotationDriveMode = RotationDriveMode.Slerp;
            AttachJoint.slerpDrive = new() { positionSpring = Mathf.Infinity, maximumForce = Mathf.Infinity };
            if (_rigidBody)
                AttachJoint.connectedBody = _rigidBody;
            if (_articulationBody)
                AttachJoint.connectedArticulationBody = _articulationBody;

            AttachJoint.autoConfigureConnectedAnchor = false;
            AttachJoint.anchor = attacher.localPosition;

            Attacher.Rigidbody.transform.rotation = rotation;

            attacher.GetPositionAndRotation(out var initialPosition, out var initialRotation);
            var initialLocalPosition = _body.InverseTransformPoint(initialPosition);
            var initialLocalRotation = Quaternion.Inverse(_body.rotation) * initialRotation;

            var elapsedTime = 0f;
            var positionDifference = Mathf.Min(
                Vector3.Distance(initialPosition, DetachPoint.position),_maxPositionDifference)
                / _maxPositionDifference;
            var rotationDifference = (-Quaternion.Dot(initialRotation, DetachPoint.rotation) + 1f) / 2f;
            var averageDifference = (positionDifference + rotationDifference) / 2f;
            var alignTime = _maxAlignTime * averageDifference;
            while (elapsedTime < alignTime)
            {
                var initialWorldPosition = _body.TransformPoint(initialLocalPosition);
                var initialWorldRotation = _body.rotation * initialLocalRotation;
                var targetPosition = Vector3.Lerp(initialWorldPosition, DetachPoint.position, elapsedTime / alignTime);
                var targetRotation = Quaternion.Lerp(initialWorldRotation, DetachPoint.rotation, elapsedTime / alignTime);

                AttachJoint.connectedAnchor = _body.InverseTransformPoint(targetPosition);
                AttachJoint.targetRotation = Quaternion.Inverse(targetRotation) * DetachPoint.rotation;

                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            OnAttach?.Invoke();
            Attacher.Attach();
            elapsedTime = 0f;
            while (elapsedTime < _insertTime)
            {
                var targetPosition = Vector3.Lerp(DetachPoint.position, AttachPoint.position, elapsedTime / _insertTime);
                var targetRotation = Quaternion.Lerp(DetachPoint.rotation, AttachPoint.rotation, elapsedTime / _insertTime);

                AttachJoint.connectedAnchor = _body.InverseTransformPoint(targetPosition);
                AttachJoint.targetRotation = Quaternion.Inverse(targetRotation) * DetachPoint.rotation;

                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            AttachJoint.connectedAnchor = _body.InverseTransformPoint(AttachPoint.position);
            AttachJoint.targetRotation = Quaternion.Inverse(AttachPoint.rotation) * DetachPoint.rotation;

            _onCooldown = false;

            if (_waitingForDetach)
                Detach();
        }

        public void Detach()
        {
            if (!Attacher)
                return;

            _waitingForDetach = true;

            if (_onCooldown)
                return;

            _waitingForDetach = false;

            _onCooldown = true;

            StartCoroutine(DetachCoroutine());
            OnDetach?.Invoke();
            Attacher.Detach();
        }

        private IEnumerator DetachCoroutine()
        {
            float elapsedTime = 0f;
            var attacher = Attacher.transform;

            while (elapsedTime < _insertTime)
            {
                var targetPosition = Vector3.Lerp(AttachPoint.position, DetachPoint.position, elapsedTime / _insertTime);
                var targetRotation = Quaternion.Lerp(AttachPoint.rotation, DetachPoint.rotation, elapsedTime / _insertTime);

                AttachJoint.connectedAnchor = _body.InverseTransformPoint(targetPosition);
                AttachJoint.targetRotation = Quaternion.Inverse(targetRotation) * DetachPoint.rotation;

                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            AttachJoint.connectedAnchor = _body.InverseTransformPoint(DetachPoint.position);
            AttachJoint.targetRotation = Quaternion.Inverse(DetachPoint.rotation) * DetachPoint.rotation;

            Destroy(AttachJoint);

            foreach (Collider attacherCollider in Attacher.Rigidbody.GetComponentsInChildren<Collider>())
                foreach (Collider socketCollider in _body.GetComponentsInChildren<Collider>())
                    Physics.IgnoreCollision(attacherCollider, socketCollider, false);

            foreach (Grabbable grab in Attacher.EnableGrabs)
                if (grab)
                    grab.enabled = false;

            foreach (Grabbable grab in Attacher.DisableGrabs)
                if (grab)
                    grab.enabled = true;

            Attacher.Rigidbody.transform.SetPositionAndRotation(
                DetachPoint.position - DetachPoint.rotation * attacher.localPosition,
                DetachPoint.rotation * Quaternion.Inverse(attacher.localRotation)
            );

            Attacher.Rigidbody.linearVelocity += (DetachPoint.position - AttachPoint.position) / _insertTime;

            Attacher.Socket = null;
            Attacher = null;

            yield return new WaitForSeconds(_cooldownTime);

            _onCooldown = false;
        }
    }
}
