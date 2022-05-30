using System.Collections.Generic;
using UnityEngine;

namespace UTS.Core
{
    public class Fare : MonoBehaviour
    {
        #region Properties and Fields

        const float TAU = 6.283185f;

        [Header("Detection Ring")]
        [SerializeField] float _radius = 5f;
        [SerializeField] int _numberOfSegments = 36;
        [SerializeField] float _rotationSpeed = 0.1f;
        [SerializeField] Transform _ringParent;
        [SerializeField] GameObject _ringSegmentPrefab;
        [SerializeField] Material _material;
        [SerializeField] Gradient _distanceGradient;
        [SerializeField] float _alpha = 0.4f;
        [SerializeField] LayerMask _terrainMask;

        private Material _materialInst;
        private Destination _destination;
        private List<GameObject> _segments = new List<GameObject>();

        private Stack<System.Action> _state = new Stack<System.Action>();

        #endregion

        #region Unity Methods

        private void Start()
        {
            _materialInst = new Material(_material);
            _destination = Destinations.Get().PickDestination();
            if (_destination != null) SetRingColor();
            _state.Push(WaitForTaxiState);
        }

        private void Update()
        {
            _state.Peek().Invoke();
        }

        #endregion

        #region Public Methods

        public float GetDetectionRadius()
            => _radius;

        public Destination GetDestination()
            => _destination;

        public void Pickup()
        {
            // we do not pop, 'cos fare might fall out
            _state.Push(PassengerState);

            // mark all other fares as idle
            foreach (var fare in FindObjectsOfType<Fare>())
                if (fare != this)
                    fare.SetIdleState();
        }

        public void DropOff()
        {
            _state.Pop();
            _state.Push(DestinationReachedState);

            foreach (var fare in FindObjectsOfType<Fare>())
                if (fare != this)
                    fare.PopState();
        }

        #endregion

        #region Private Methods

        // This state is active when the fare is waiting for a ride
        private void WaitForTaxiState()
        {
            if (_destination == null)
            {
                _destination = Destinations.Get().PickDestination();
                if (_destination != null) SetRingColor();
            }

            _destination.SetActive(false);
            _ringParent.gameObject.SetActive(true);

            DrawDetectionRing();
        }

        // This state is active when the fare is waiting for a ride
        // but the taxi is not currently available
        private void IdleState()
        {
            _destination.SetActive(false);
            _ringParent.gameObject.SetActive(false);
        }

        // This state is active when the fare is currently in the taxi
        private void PassengerState()
        {
            _destination.SetActive(true);
            _ringParent.gameObject.SetActive(false);
        }

        // This state is active when the fare has reached the desired destination
        private void DestinationReachedState()
        {
            _destination.SetActive(false);
            _ringParent.gameObject.SetActive(false);
            Destroy(gameObject);
        }

        private void PopState()
            => _state.Pop();

        private void SetIdleState(bool pop = false)
        {
            if (pop) _state.Pop();
            _state.Push(IdleState);
        }

        private void SetRingColor()
        {
            var from = new Vector3(transform.position.x, 0f, transform.position.z);
            var to = new Vector3(_destination.transform.position.x, 0f, _destination.transform.position.z);
            var dist = Vector3.Distance(from, to);
            var color = _distanceGradient.Evaluate(Mathf.InverseLerp(0, Ruler.Get().WorldDistance(), dist));
            color.a = _alpha;
            _materialInst.color = color;
            var emissionColor = new Color(color.r * 3f, color.g * 3f, color.b * 3f, 1f);
            _materialInst.SetColor("_EmissionColor", emissionColor);
        }

        private void DrawDetectionRing()
        {
            CreateSegments();
            var angle = TAU / _segments.Count;
            for (var i = 0; i < _segments.Count; i++)
            {
                var offset = i * angle + Time.time * _rotationSpeed;
                var v3 = transform.position + new Vector3(Mathf.Sin(offset) * _radius, 0f, Mathf.Cos(offset) * _radius);
                if (Physics.Raycast(v3 + Vector3.up * 500f, Vector3.down, out RaycastHit hit, 1000f, _terrainMask))
                    v3.y = hit.point.y;
                _segments[i].transform.position = v3;
            }
            for (var i = 0; i < _segments.Count; i++)
            {
                var before = i == 0 ? _segments[_segments.Count - 1] : _segments[i - 1];
                var after = i == _segments.Count - 1 ? _segments[0] : _segments[i + 1];
                var normalized = (after.transform.position - before.transform.position).normalized;
                _segments[i].transform.rotation = Quaternion.LookRotation(normalized, Vector3.up);
            }
        }

        private void CreateSegments()
        {
            if (_segments.Count == _numberOfSegments) return;
            _segments.ForEach(s => Destroy(s));
            _segments.Clear();
            for (var i = 0; i < _numberOfSegments; i++)
            {
                var segment = Instantiate(_ringSegmentPrefab, transform.position, Quaternion.identity, _ringParent.transform);
                segment.GetComponent<DetectionRingSegment>().SetRendererMaterial(_materialInst);
                _segments.Add(segment);
            }
        }

        #endregion
    }
}
