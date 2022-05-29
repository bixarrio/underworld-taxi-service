using System.Collections.Generic;
using UnityEngine;
using UTS.Control;

namespace UTS.Core
{
    public class TaxiService : MonoBehaviour
    {
        #region Properties and Fields

        [SerializeField] Transform _passengerSeat;
        [SerializeField] DestinationPointer _pointer;
        [SerializeField] LayerMask _fareMask;

        private Movement _movement;

        private Fare _currentFare = default;
        private Destination _currentDestination = default;

        private Stack<System.Action> _state = new Stack<System.Action>();

        #endregion

        #region Unity Methods

        private void Start()
        {
            _movement = GetComponent<Movement>();
            _state.Push(FindAFareState);
        }

        private void Update()
        {
            _state.Peek().Invoke();
        }

        #endregion

        #region Public Methods

        public Destination GetDestination()
            => _currentDestination;

        public void DumpMe()
        {
            _currentFare = null;
            _currentDestination = null;

            _state.Pop();
            _state.Push(FindAFareState);
        }

        #endregion

        #region Private Methods

        private void FindAFareState()
        {
            foreach(var fare in FindObjectsOfType<Fare>())
            {
                var direction = (fare.transform.position - transform.position).normalized;
                if (Physics.SphereCast(transform.position, 1f, direction, out RaycastHit hit, fare.GetDetectionRadius(), _fareMask))
                {
                    // TODO: Show On-Screen 'E' when in range
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        _currentFare = fare;
                        _currentFare.Pickup();
                        _currentFare.transform.SetParent(_passengerSeat);
                        _currentFare.transform.position = _passengerSeat.position;
                        _currentFare.transform.rotation = _passengerSeat.rotation;

                        _currentDestination = fare.GetDestination();

                        _state.Pop();
                        _state.Push(DeliverAFareState);
                    }

                    return;
                }
            }
        }

        private void DeliverAFareState()
        {
            if (!_currentDestination.IsInRange(transform.position)) return;

            // Good to deliver
            _currentDestination = null;

            _currentFare.DropOff();
            _currentFare = null;

            _state.Pop();
            _state.Push(FindAFareState);
        }

        #endregion
    }
}
