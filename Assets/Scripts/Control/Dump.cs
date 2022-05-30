using UnityEngine;

namespace UTS.Control
{
    public class Dump : MonoBehaviour
    {
        #region Properties and Fields

        [SerializeField] GameObject _stiffie;
        [SerializeField] GameObject _ragdoll;

        #endregion

        #region Public Methods

        public void DumpMe()
        {
            transform.SetParent(null, true);
            if (_ragdoll != null)
            {
                _stiffie.SetActive(false);
                _ragdoll.SetActive(true);
            }
            else
            {
                gameObject.AddComponent<Rigidbody>().mass = 0.1f;
            }
        }

        public void UndumpMe(Transform parent)
        {
            if (parent != null)
            {
                transform.position = parent.position;
                transform.rotation = parent.rotation;
                transform.SetParent(parent);
            }
            if (_ragdoll != null)
            {
                _stiffie.SetActive(true);
                _ragdoll.SetActive(false);
            }
            else
            {
                Destroy(GetComponent<Rigidbody>());
            }
        }

        #endregion
    }
}
