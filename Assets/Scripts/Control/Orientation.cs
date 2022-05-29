using System.Collections;
using UnityEngine;
using UTS.Core;

namespace UTS.Control
{
    public class Orientation : MonoBehaviour
    {
        #region Properties and Fields

        [SerializeField] float _dumpOrientation = 0.2f;
        [SerializeField] float _resetDelay = 2f;
        [SerializeField] Transform _driver;
        [SerializeField] Transform _driverSeat;
        [SerializeField] Transform _passengerSeat;

        private bool _dumped = false;

        #endregion

        #region Unity Methods

        private void FixedUpdate()
        {
            if (_dumped) return;

            // Check if the boat is still upright
            var dot = Vector3.Dot(transform.up, Vector3.up);
            if (dot <= _dumpOrientation) DumpContents();
        }

        private void DumpContents()
        {
            _driver.GetComponent<Dump>().DumpMe();
            GetComponent<TaxiService>().DumpMe();
            var passenger = _passengerSeat.GetChild(0);
            passenger.GetComponent<Dump>().DumpMe();
            StartCoroutine(ResetRoutine(transform.position, passenger));
            _dumped = true;
        }

        private IEnumerator ResetRoutine(Vector3 resetLocation, Transform dumpedPassenger)
        {
            yield return new WaitForSeconds(_resetDelay);

            transform.GetComponent<Rigidbody>().isKinematic = true;
            yield return null;

            transform.position = resetLocation;
            transform.up = Vector3.up;
            _driver.GetComponent<Dump>().UndumpMe(_driverSeat);

            transform.GetComponent<Rigidbody>().isKinematic = false;
            yield return null;

            dumpedPassenger.GetComponent<Fare>().ResetMe();
            dumpedPassenger.GetComponent<Dump>().UndumpMe(null);

            _dumped = false;
        }

        #endregion
    }
}
