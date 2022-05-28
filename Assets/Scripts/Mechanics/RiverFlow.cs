using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UTS.Mechanics
{
    [RequireComponent(typeof(Collider))]
    public class RiverFlow : MonoBehaviour
    {
        #region Properties and Fields

        [SerializeField] float _strength = 1f;

        private Collider _collider;

        #endregion

        #region Unity Methods

        private void Start()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
        }

        private void OnTriggerStay(Collider other)
        {
            var rb = other.GetComponent<Rigidbody>();
            if (rb == null) return;
            rb.AddForce(transform.forward * _strength);
        }

        #endregion
    }
}
