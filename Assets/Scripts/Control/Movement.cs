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

        #endregion

        #region Unity Methods

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _taxi = GetComponent<TaxiService>();
        }

        private void FixedUpdate()
        {
            if (ProcessMovement()) return;
        }

        #endregion

        #region Public Methods

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

            return true;
        }

        #endregion
    }
}
