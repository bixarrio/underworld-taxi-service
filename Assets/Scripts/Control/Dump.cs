using UnityEngine;

namespace UTS.Control
{
    public class Dump : MonoBehaviour
    {
        #region Public Methods

        public void DumpMe()
        {
            transform.SetParent(null);
            gameObject.AddComponent<Rigidbody>();
        }

        #endregion
    }
}
