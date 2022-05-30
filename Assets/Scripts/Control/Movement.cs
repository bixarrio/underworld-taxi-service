using UnityEngine;
using UTS.Core;

namespace UTS.Control
{
    [RequireComponent(typeof(Rigidbody))]
    public class Movement : MonoBehaviour
    {
        #region Properties and Fields

        [SerializeField] float _moveSpeed = 10f;
        [SerializeField] float _rotationalSpeed = 10f;

        private Rigidbody _rb;
        private TaxiService _taxi;

        private bool _active = true;

        #endregion

        #region Unity Methods

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _taxi = GetComponent<TaxiService>();
        }

        private void FixedUpdate()
        {
            if (!_active) return;
            if (ProcessMovement()) return;
        }

        #endregion

        #region Public Methods

        public void SetActive(bool enabled)
        {
            _active = enabled;
            TogglePhysics(enabled);
        }

        public void TogglePhysics(bool enabled)
            => _rb.isKinematic = !enabled;

        #endregion

        #region Private Methods

        private bool ProcessMovement()
        {
            var turning = Input.GetAxis("Horizontal");
            _rb.AddTorque(transform.up * turning * _rotationalSpeed * Time.deltaTime);

            // This can make us go backwards which is not ideal, but it's a jam and ok for now
            var moving = Input.GetAxis("Vertical");
            _rb.AddForce(transform.forward * moving * _moveSpeed * Time.deltaTime);

            // counter force if we are moving sideways
            var currentVelocity = _rb.velocity;
            var counterFactor = Vector3.Dot(currentVelocity.normalized, transform.forward);
            var counterVelocity = (currentVelocity * (1f - Mathf.Abs(counterFactor))) * -1;
            _rb.AddForce(counterVelocity);

            Debug.DrawLine(transform.position, transform.position + counterVelocity, Color.blue);
            Debug.DrawLine(transform.position, transform.position + _rb.velocity, Color.green);

            return true;
        }

        #endregion
    }
}
