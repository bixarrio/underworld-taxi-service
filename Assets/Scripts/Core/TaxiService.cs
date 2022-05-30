using System.Collections.Generic;
using UnityEngine;
using UTS.Control;

namespace UTS.Core
{
    public class TaxiService : MonoBehaviour
    {
        #region Properties and Fields

        public enum Difficulty { Easy, Normal, Hard }

        [SerializeField] Transform _passengerSeat;
        [SerializeField] DestinationPointer _pointer;
        [SerializeField] LayerMask _fareMask;
        [Header("Lantern")]
        [SerializeField] Renderer _lightEmitter;
        [SerializeField] Material _onMaterial;
        [SerializeField] Material _offMaterial;
        [SerializeField] Light _light;
        [Header("Timer")]
        [SerializeField] float _maxAllowedTime;
        [SerializeField] float[] _difficultyMultipliers;
        [SerializeField] Difficulty _currentDifficulty;
        [SerializeField] FareTimer _fareTimer;

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

        #endregion

        #region Private Methods

        private void FindAFareState()
        {
            _lightEmitter.material = _onMaterial;
            _light.enabled = true;
            foreach(var fare in FindObjectsOfType<Fare>())
            {
                var direction = (fare.transform.position - transform.position).normalized;
                if (Physics.SphereCast(transform.position, 1f, direction, out RaycastHit hit, fare.GetDetectionRadius(), _fareMask))
                {
                    // TODO: Show On-Screen 'E' when in range
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        // Set the fare
                        _currentFare = fare;
                        _currentFare.Pickup();
                        _currentFare.transform.SetParent(_passengerSeat);
                        _currentFare.transform.position = _passengerSeat.position;
                        _currentFare.transform.rotation = _passengerSeat.rotation;

                        // Add some time to the game timer for the drop-off
                        _currentDestination = fare.GetDestination();
                        GameTimer.Get().AddTime(CalculateTime(_currentFare, _currentDestination));
                        _fareTimer.SetTimerDuration(CalculateTime(_currentFare, _currentDestination));
                        _fareTimer.SetActive(true);

                        // Change our state
                        _state.Pop();
                        _state.Push(DeliverAFareState);
                    }

                    return;
                }
            }
        }

        private float CalculateTime(Fare currentFare, Destination currentDestination)
        {
            var dist = Vector3.Distance(currentFare.transform.position, currentDestination.transform.position);
            var factor = Mathf.InverseLerp(0f, Ruler.Get().WorldDistance(), dist);
            return GetAllowedTimeFoDifficulty() * factor;
        }

        private float GetAllowedTimeFoDifficulty()
            => _maxAllowedTime * _difficultyMultipliers[(int)_currentDifficulty];

        private void DeliverAFareState()
        {
            _lightEmitter.material = _offMaterial;
            _light.enabled = false;
            if (!_currentDestination.IsInRange(transform.position)) return;

            // Good to deliver
            _currentDestination = null;

            _currentFare.DropOff();
            _currentFare = null;

            _fareTimer.SetActive(false);
            GameTimer.Get().AddTime(_fareTimer.GetCurrentTime());

            _state.Pop();
            _state.Push(FindAFareState);
        }

        #endregion
    }
}
