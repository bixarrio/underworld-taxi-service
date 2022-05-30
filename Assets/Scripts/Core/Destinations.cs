using System.Collections.Generic;
using UnityEngine;

namespace UTS.Core
{
    public class Destinations : Spawner
    {
        #region Properties and Fields

        private List<Destination> _destinations = new List<Destination>();

        #endregion

        #region Unity Methods

        private void Start()
        {
            SpawnObjects();
            LoadDestinations();
        }

        #endregion

        #region Public Methods

        public static Destinations Get()
            => FindObjectOfType<Destinations>();

        public Destination PickDestination()
        {
            if (_destinations.Count == 0) return null;
            return _destinations[Random.Range(0, _destinations.Count)];
        }

        #endregion

        #region Private Methods

        private void LoadDestinations()
        {
            foreach (Transform child in transform)
            {
                var dest = child.GetComponent<Destination>();
                if (dest == null) continue;
                _destinations.Add(dest);
            }
        }

        #endregion
    }
}
