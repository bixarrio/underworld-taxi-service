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
        [SerializeField] KeyCode _pickupKey = KeyCode.E;

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
            if (ProcessPickup()) return;
            if (ProcessDropOff()) return;
            if (ProcessMovement()) return;
        }

        #endregion

        #region Public Methods

        public void TogglePhysics(bool enabled)
            => _rb.isKinematic = !enabled;

        #endregion

        #region Private Methods

        private bool ProcessPickup()
        {
            if (!_taxi.CanTakeFare()) return false;

            // find all the fares
            var fares = FindObjectsOfType<FareTrigger>();
            foreach (var fare in fares)
            {
                if (!fare.WithinRange(transform.position)) continue;
                if (Input.GetKeyDown(_pickupKey))
                {
                    TogglePhysics(false);
                    _taxi.Pickup(fare.GetComponent<Fare>());
                    return true;
                }
            }
            return false;
        }

        private bool ProcessDropOff()
        {
            if (!_taxi.CanDropOff()) return false;

            _taxi.DropOff();
            return true;
        }

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
