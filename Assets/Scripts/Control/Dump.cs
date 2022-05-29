using UnityEngine;

namespace UTS.Control
{
    public class Dump : MonoBehaviour
    {
        #region Public Methods

        public void DumpMe()
        {
            transform.SetParent(null);
            gameObject.AddComponent<Rigidbody>().mass = 0.1f;
        }

        public void UndumpMe(Transform parent)
        {
            if (parent != null)
            {
                transform.position = parent.position;
                transform.rotation = parent.rotation;
                transform.SetParent(parent);
            }
            Destroy(GetComponent<Rigidbody>());
        }

        #endregion
    }
}
