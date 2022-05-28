using UnityEngine;

namespace UTS.Core
{
    public class Fare : MonoBehaviour
    {
        #region Properties and Fields

        [SerializeField] DestinationTrigger _destination;

        #endregion

        #region Unity Methods

        private void Start()
        {
            var destinations = FindObjectsOfType<DestinationTrigger>(true);
            _destination = destinations[Random.Range(0, destinations.Length)];
            GetComponent<FareTrigger>().SetDestination(_destination);
        }

        #endregion

        #region Public Methods

        public void ToggleDestination(bool enabled)
            => _destination.SetEnabled(enabled);

        public bool WithinRange(Vector3 position)
            => _destination.WithinRange(position);

        public DestinationTrigger GetDestination() => _destination;

        #endregion
    } 
}
