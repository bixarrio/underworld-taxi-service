using UnityEngine;

public class Ruler : MonoBehaviour
{
    [SerializeField] Transform _other;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, _other.position);
        Debug.Log(Vector3.Distance(transform.position, _other.position).ToString());
    }
}
