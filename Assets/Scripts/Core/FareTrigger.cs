using UnityEngine;

namespace UTS.Core
{
    public class FareTrigger : Trigger
    {
        #region Properties and Fields

        [SerializeField] Transform _worldPointA;
        [SerializeField] Transform _worldPointB;

        private float _far = 0f;
        private DestinationTrigger _destination;

        #endregion

        #region Unity Methods

        private void Start()
        {
            _far = Vector3.Distance(_worldPointA.position, _worldPointB.position);
        }

        #endregion

        #region Public Methods

        public static void ToggleTrigger(bool enabled)
        {
            foreach (var trigger in FindObjectsOfType<FareTrigger>(true))
                trigger.SetEnabled(enabled);
        }

        public void SetDestination(DestinationTrigger destination)
        {
            _destination = destination;
            var dist = Vector3.Distance(transform.position, destination.transform.position);
            var r = Mathf.InverseLerp(0f, _far, dist);
            Debug.Log($"dist: {dist}, far: {_far}, ilerp: {r}");
            _lineRenderer.material.color = _distanceColorGradient.Evaluate(r);
        }

        #endregion
    }
}
