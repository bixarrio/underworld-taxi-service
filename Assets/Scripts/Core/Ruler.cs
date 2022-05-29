using UnityEngine;

public class Ruler : MonoBehaviour
{
    #region Properties and Fields

    [SerializeField] Transform _worldPointA;
    [SerializeField] Transform _worldPointB;

    #endregion

    #region Unity Methods

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_worldPointA.position, _worldPointB.position);
    }

    #endregion

    #region Public Methods

    public static Ruler Get()
        => FindObjectOfType<Ruler>();

    public float WorldDistance()
        => Vector3.Distance(_worldPointA.position, _worldPointB.position);

    #endregion
}
