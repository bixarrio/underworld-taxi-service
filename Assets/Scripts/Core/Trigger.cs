using System.Collections.Generic;
using UnityEngine;

namespace UTS.Core
{
    public abstract class Trigger : MonoBehaviour
    {
        #region Properties and Fields
        
        [SerializeField] protected float _detectionRadius = 3f;
        [SerializeField] protected int _segments = 36;
        [SerializeField] protected float _rotationSpeed = 0.01f;
        [SerializeField] protected LineRenderer _lineRenderer;
        [SerializeField] protected Gradient _distanceColorGradient;
        [SerializeField] protected LayerMask _mask;

        protected List<Vector3> _points = new List<Vector3>();

        #endregion

        #region Unity Methods

        protected void Update()
        {
            DrawDetectionCircle();
        }

        #endregion

        #region Public Methods

        public virtual bool WithinRange(Vector3 position)
        {
            var flatMe = new Vector3(transform.position.x, 0f, transform.position.z);
            var flatYou = new Vector3(position.x, 0f, position.z);
            return Vector3.Distance(flatYou, flatMe) <= _detectionRadius;
        }

        public virtual void SetEnabled(bool enabled)
        {
            this.enabled = enabled;
            _lineRenderer.enabled = enabled;
        }

        #endregion

        #region Private Methods

        protected virtual void CreatePoints()
        {
            if (_points.Count == _segments) return;
            _points.Clear();
            for (var i = 0; i < _segments; i++)
                _points.Add(transform.position);
        }

        protected virtual void DrawDetectionCircle()
        {
            CreatePoints();

            var angles = 6.283158f / _segments;
            var points = new Vector3[_segments + 1];
            for (var i = 0; i < _segments; i++)
            {
                var f = i * angles + Time.time * _rotationSpeed;
                var v3 = transform.position + new Vector3(Mathf.Sin(f) * _detectionRadius, 0f, Mathf.Cos(f) * _detectionRadius);
                var point = _points[i];
                if (Physics.Raycast(v3 + Vector3.up * 500f, Vector3.down, out RaycastHit hit, 1000f, _mask.value))
                    v3.y = hit.point.y;
                points[i] = v3;
            }
            points[_segments] = points[0];
            _lineRenderer.positionCount = _segments + 1;
            _lineRenderer.SetPositions(points);
        }

        #endregion
    }
}
