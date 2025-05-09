using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;

namespace BIMOS
{
    public class Socket : MonoBehaviour
    {
        public string[] Tags;

        public Transform AttachPoint, DetachPoint;

        [SerializeField]
        private float _insertTime = 0.1f;

        [HideInInspector]
        public Attacher Attacher;

        [HideInInspector]
        public FixedJoint AttachJoint;

        [SerializeField]
        private AudioVarianceData _attachSounds, _detachSounds;

        [SerializeField]
        private UnityEvent AttachEvent, DetachEvent; 

        private bool _onCooldown;
        private float _cooldownTime;
        private bool _waitingForDetach;
        private AudioSource _audioSource;

        private Rigidbody _rigidBody;
        private ArticulationBody _articulationBody;
        private Transform _body;

        private void Awake()
        {
            _body = Utilities.GetBody(transform, out _rigidBody, out _articulationBody);
            _audioSource = GetComponent<AudioSource>();
        }

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

            if (_audioSource && _attachSounds)
                _audioSource.PlayOneShot(_attachSounds.GetRandomClip());

            StartCoroutine(AttachCoroutine());
            AttachEvent.Invoke();
            Attacher.Attach();
        }

        private IEnumerator AttachCoroutine()
        {
            float elapsedTime = 0f;
            var attacher = Attacher.transform;
            Attacher.Rigidbody.isKinematic = true;

            while (elapsedTime < _insertTime)
            {
                var targetPosition = Vector3.Lerp(DetachPoint.position, AttachPoint.position, elapsedTime / _insertTime);
                var targetRotation = Quaternion.Lerp(DetachPoint.rotation, AttachPoint.rotation, elapsedTime / _insertTime);

                Attacher.Rigidbody.Move(
                    targetPosition - targetRotation * attacher.localPosition,
                    targetRotation * Quaternion.Inverse(attacher.localRotation)
                );

                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            Attacher.Rigidbody.transform.SetPositionAndRotation(
                AttachPoint.position - AttachPoint.rotation * attacher.localPosition,
                AttachPoint.rotation * Quaternion.Inverse(attacher.localRotation)
            );

            AttachJoint = Attacher.Rigidbody.gameObject.AddComponent<FixedJoint>();
            if (_rigidBody)
                AttachJoint.connectedBody = _rigidBody;
            if (_articulationBody)
                AttachJoint.connectedArticulationBody = _articulationBody;

            Attacher.Rigidbody.isKinematic = false;

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

            if (_audioSource && _detachSounds)
                _audioSource.PlayOneShot(_detachSounds.GetRandomClip());

            Destroy(AttachJoint);

            StartCoroutine(DetachCoroutine());
            DetachEvent.Invoke();
            Attacher.Detach();
        }

        private IEnumerator DetachCoroutine()
        {
            float elapsedTime = 0f;
            var attacher = Attacher.transform;
            Attacher.Rigidbody.isKinematic = true;

            while (elapsedTime < _insertTime)
            {
                var targetPosition = Vector3.Lerp(AttachPoint.position, DetachPoint.position, elapsedTime / _insertTime);
                var targetRotation = Quaternion.Lerp(AttachPoint.rotation, DetachPoint.rotation, elapsedTime / _insertTime);

                Attacher.Rigidbody.Move(
                    targetPosition - targetRotation * attacher.localPosition,
                    targetRotation * Quaternion.Inverse(attacher.localRotation)
                );

                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

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

            Attacher.Rigidbody.isKinematic = false;
            Attacher.Rigidbody.linearVelocity += (DetachPoint.position - AttachPoint.position) / _insertTime;

            Attacher.Socket = null;
            Attacher = null;

            yield return new WaitForSeconds(_cooldownTime);

            _onCooldown = false;
        }
    }
}
