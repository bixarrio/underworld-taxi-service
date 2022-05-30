using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UTS.Core
{
    public class Spawner : MonoBehaviour
    {
        #region Properties and Fields

        [SerializeField] protected GameObject[] _objectsToSpawn;
        [SerializeField] protected int _maxObjects = 100;
        [Header("Scan Area")]
        [SerializeField] protected Vector3 _fromPosition;
        [SerializeField] protected Vector3 _toPosition;
        [SerializeField, Min(0.1f)] protected float _stepSize = 10f;
        [SerializeField] protected float _detectHeightMin = 8f;
        [SerializeField] protected float _detectHeightMax = 10f;
        [Header("Randomness")]
        [SerializeField] protected bool _randomRotation = false;
        [SerializeField] protected Vector3 _rotationAxis = Vector3.zero;
        [SerializeField] protected float _minRotation = 0f;
        [SerializeField] protected float _maxRotation = 0f;

        #endregion

        #region Unity Methods

        private void Start()
        {
            SpawnObjects();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;

            var hits = 0;
            for (var z = _fromPosition.z; z < _toPosition.z; z += _stepSize)
            {
                for (var x = _fromPosition.x; x < _toPosition.x; x += _stepSize)
                {
                    var ray = new Ray(new Vector3(x, 100f, z), Vector3.down);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                        if (hit.point.y >= _detectHeightMin && hit.point.y <= _detectHeightMax)
                        {
                            Gizmos.DrawSphere(hit.point, _stepSize / 3f);
                            hits++;
                        }
                }
            }
        }

        #endregion

        #region Private Methods

        protected void SpawnObjects()
        {
            // get a list of valid spawn points
            var validPoints = ScanForValidPoints();
            validPoints = validPoints.OrderBy(_ => Random.value).ToList();
            var chosen = validPoints;
            if (_maxObjects >= 0) chosen = validPoints.Take(_maxObjects).ToList();
            foreach (var point in chosen)
            {
                var rotation = Quaternion.identity;
                if (_randomRotation)
                {
                    rotation = Quaternion.Euler(
                        Random.Range(_minRotation, _maxRotation) * _rotationAxis.x,
                        Random.Range(_minRotation, _maxRotation) * _rotationAxis.y,
                        Random.Range(_minRotation, _maxRotation) * _rotationAxis.z);
                }
                Instantiate(_objectsToSpawn[Random.Range(0, _objectsToSpawn.Length)], point, rotation, transform);
            }
        }

        protected List<Vector3> ScanForValidPoints()
        {
            var validPoints = new List<Vector3>();
            for (var z = _fromPosition.z; z < _toPosition.z; z += _stepSize)
            {
                for (var x = _fromPosition.x; x < _toPosition.x; x += _stepSize)
                {
                    var ray = new Ray(new Vector3(x, 100f, z), Vector3.down);
                    if (!Physics.Raycast(ray, out RaycastHit hit))
                        continue;

                    if (hit.point.y >= _detectHeightMin && hit.point.y <= _detectHeightMax)
                        validPoints.Add(hit.point);
                }
            }
            return validPoints;
        }

        #endregion
    }
}
