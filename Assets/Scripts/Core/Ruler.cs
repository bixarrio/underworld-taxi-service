using System.Collections.Generic;
using UnityEngine;

public class Ruler : MonoBehaviour
{
    #region Properties and Fields

    [SerializeField] List<Transform> _worldPoints;

    #endregion

    #region Unity Methods

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        for (var i = 1; i < _worldPoints.Count; i++)
            Gizmos.DrawLine(_worldPoints[i - 1].position, _worldPoints[i].position);
    }

    #endregion

    #region Public Methods

    public static Ruler Get()
        => FindObjectOfType<Ruler>();

    public float WorldDistance()
    {
        var dist = 0f;
        for (var i = 1; i < _worldPoints.Count; i++)
            dist += Vector3.Distance(_worldPoints[i - 1].position, _worldPoints[i].position);
        return dist;
    }

    #endregion
}
