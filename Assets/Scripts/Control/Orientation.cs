using UnityEngine;

namespace UTS.Control
{
    public class Orientation : MonoBehaviour
    {
        #region Properties and Fields

        [SerializeField] float _dumpOrientation = 0.2f;
        [SerializeField] Transform _driver;
        [SerializeField] Transform _passengerSeat;

        private bool _dumped = false;

        #endregion

        #region Unity Methods

        private void FixedUpdate()
        {
            if (_dumped) return;

            // Check if the boat is still upright
            var dot = Vector3.Dot(transform.up, Vector3.up);
            if (dot <= _dumpOrientation)
            {
                // dump your contents
                _driver.GetComponent<Dump>().DumpMe();
                foreach (Transform child in _passengerSeat)
                    child.GetComponent<Dump>().DumpMe();

                _dumped = true;
            }
        }

        #endregion
    } 
}
