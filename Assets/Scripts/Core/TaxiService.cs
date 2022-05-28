using UnityEngine;
using UTS.Control;

namespace UTS.Core
{
    public class TaxiService : MonoBehaviour
    {
        #region Properties and Fields

        [SerializeField] Transform _passengerSeat;

        private Movement _movement;

        #endregion

        #region Unity Methods

        private void Start()
        {
            _movement = GetComponent<Movement>();
        }

        #endregion

        #region Public Methods

        public bool CanTakeFare()
            => _passengerSeat.childCount == 0;

        public void Pickup(Fare fare)
        {
            fare.transform.SetParent(_passengerSeat, true);

            fare.transform.position = _passengerSeat.position;
            fare.transform.up = _passengerSeat.up;
            fare.transform.forward = _passengerSeat.forward;

            FareTrigger.ToggleTrigger(false);
            _movement.TogglePhysics(true);

            fare.ToggleDestination(true);
        }

        public void DropOff()
        {
            var fare = _passengerSeat.GetChild(0).GetComponent<Fare>();
            var dest = fare.GetDestination();

            fare.transform.SetParent(dest.transform, true);

            fare.transform.position = dest.transform.position;
            //fare.transform.up = dest.transform.up;
            //fare.transform.forward = dest.transform.forward;

            FareTrigger.ToggleTrigger(true);
            fare.ToggleDestination(false);
        }

        public bool CanDropOff()
        {
            if (_passengerSeat.childCount == 0) return false;
            return _passengerSeat.GetChild(0).GetComponent<Fare>().WithinRange(transform.position);
        }

        #endregion
    } 
}
