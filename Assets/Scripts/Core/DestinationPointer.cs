using System.Collections.Generic;
using UnityEngine;

namespace UTS.Core
{
    public class DestinationPointer : MonoBehaviour
    {
        #region Properties and Fields

        [SerializeField] GameObject _arrowObject;
        [SerializeField] Gradient _distanceGradient;
        [SerializeField] float _alpha = 0.4f;
        [SerializeField] float _rotationSpeed = 2f;
        [SerializeField] TaxiService _taxi;
        [Header("Map Stuff")]
        [SerializeField] Vector2 _worldOffset;
        [SerializeField] Vector2 _worldDimensions;

        private Vector3 _velocity = Vector3.zero;
        private Material _material;

        #endregion

        #region Unity Methods

        private void Start()
        {
            _material = new Material(_arrowObject.GetComponent<Renderer>().sharedMaterial);
            _arrowObject.GetComponent<Renderer>().material = _material;
        }

        private void LateUpdate()
        {
            PointToDestination();
        }

        #endregion

        #region Private Methods

        private void SetColor(Vector3 from, Vector3 to)
        {
            var dist = Vector3.Distance(from, to);
            var color = _distanceGradient.Evaluate(Mathf.InverseLerp(0, Ruler.Get().WorldDistance(), dist));
            color.a = _alpha;
            _material.color = color;
            var emissionColor = new Color(color.r * 3f, color.g * 3f, color.b * 3f, 1f);
            _material.SetColor("_EmissionColor", emissionColor);
        }

        private void PointToDestination()
        {
            var destination = _taxi.GetDestination();

            _arrowObject.SetActive(destination != null);
            if (destination == null) return;

            var from = new Vector3(transform.position.x, 0f, transform.position.z);
            var to = new Vector3(destination.transform.position.x, 0f, destination.transform.position.z);
            SetColor(from, to);

            var desiredRot = Quaternion.LookRotation(to - from);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, Time.deltaTime * _rotationSpeed);
        }

        #endregion
    }
}
