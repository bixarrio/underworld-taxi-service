using System.Collections.Generic;
using UnityEngine;

namespace UTS.Core
{
    public class Destination : MonoBehaviour
    {
        #region Properties and Fields

        const float TAU = 6.283185f;

        [Header("Detection Ring")]
        [SerializeField] float _radius = 5f;
        [SerializeField] int _numberOfSegments = 36;
        [SerializeField] float _rotationSpeed = 0.1f;
        [SerializeField] Transform _ringParent;
        [SerializeField] GameObject _ringSegmentPrefab;
        [SerializeField] LayerMask _terrainMask;
        [Header("Pointer")]
        [SerializeField] GameObject _pointer;
        [SerializeField] float _bobSpeed = 1f;
        [SerializeField] float _bobRange = 0.01f;

        private List<GameObject> _segments = new List<GameObject>();

        #endregion

        #region Unity Methods

        private void Start()
        {
            _pointer.SetActive(false);
            _ringParent.gameObject.SetActive(false);
        }

        private void Update()
        {
            DetectionRing();
            PointerBob();
        }

        #endregion

        #region Public Methods

        public void SetActive(bool active)
        {
            _pointer.SetActive(active);
            _ringParent.gameObject.SetActive(active);
        }

        public bool IsInRange(Vector3 position)
            => Vector3.Distance(position, transform.position) <= _radius;

        #endregion

        #region Private Methods

        private void PointerBob()
        {
            // bob
            var f = (TAU * Time.time * _bobSpeed) % TAU;
            var pos = _pointer.transform.position;
            pos.y += Mathf.Sin(f) * _bobRange;
            _pointer.transform.position = pos;

            // rotate
            var rot = _pointer.transform.eulerAngles;
            rot.y += 360f * Time.deltaTime;
            _pointer.transform.eulerAngles = rot;
        }

        private void DetectionRing()
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
                _segments.Add(Instantiate(_ringSegmentPrefab, transform.position, Quaternion.identity, _ringParent.transform));
        }

        #endregion
    }
}
