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

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                var worldRect = new Rect(_worldOffset, _worldDimensions);
                var quadTree = new QuadTree(worldRect, 5);

                var dest = _taxi.GetDestination();
                if (dest != null)
                {
                    quadTree.AddPoints(new[] { dest.transform.position });
                    var lookAt = quadTree.FindPointToLookAt(transform.position, dest.transform.position);
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(transform.position, lookAt);
                }

                quadTree.RootNode.Draw();
            }
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

            // This is too precise. Makes it hard with all the windyness
            //var desiredRot = Quaternion.LookRotation(to - from);
            //transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, Time.deltaTime * _rotationSpeed);

            // This is a little less precise. At least at the start
            // it points in the general direction and fine tunes as it gets closer to the target
            to = GetSomethingToPointAt(destination);
            to.y = 0f;
            var desiredRot = Quaternion.LookRotation(to - from);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, Time.deltaTime * _rotationSpeed);
        }

        private Vector3 GetSomethingToPointAt(Destination destination)
        {
            var worldRect = new Rect(_worldOffset, _worldDimensions);
            var quadTree = new QuadTree(worldRect, 5);
            return quadTree.FindPointToLookAt(transform.position, destination.transform.position);
        }

        #endregion

        #region Classes and Structs

        class QuadTree
        {
            #region Properties and Fields

            public QuadTreeNode RootNode { get; set; }

            #endregion

            #region ctor

            public QuadTree(Rect worldDimensions, int maxDepth)
                => RootNode = new QuadTreeNode(worldDimensions, maxDepth);

            #endregion

            #region Public Methods

            public void AddPoints(IEnumerable<Vector3> points)
            {
                foreach (var point in points)
                    RootNode.AddPoint(point);
            }

            public Vector3 FindPointToLookAt(Vector3 from, Vector3 final)
            {
                return RootNode.FindPointToLookAt(from, final);
            }

            #endregion
        }

        class QuadTreeNode
        {
            #region Properties and Fields

            private Rect _nodeRect;
            private int _maxDepth;

            private Rect[] _childRects;
            private QuadTreeNode[] _children;

            #endregion

            #region ctor

            public QuadTreeNode(Rect rect, int maxDepth)
            {
                _nodeRect = rect;
                _maxDepth = maxDepth;

                Subdivide();
            }

            #endregion

            #region Public Methods

            public void Draw()
            {
                Gizmos.color = new Color(1f, 1f, 0f, 0.25f);
                Gizmos.DrawWireCube(
                    new Vector3(_nodeRect.center.x, 0f, _nodeRect.center.y),
                    new Vector3(_nodeRect.size.x, 1f, _nodeRect.size.y));

                if (_children == null) return;

                foreach (var child in _children)
                    if (child != null)
                        child.Draw();
            }

            public void AddPoint(Vector3 point)
                => AddPointAndDivide(point);

            public Vector3 FindPointToLookAt(Vector3 from, Vector3 final)
            {
                // if from and final are in the same quadrant, subdivide
                // until we've reached maxdepth, or they are no longer
                // sharing a quadrant.
                return FindPointOrDivide(from, final);
            }

            #endregion

            #region Private Methods

            private void AddPointAndDivide(Vector3 point, int depth = 0)
            {
                if (depth >= _maxDepth) return;
                if (_children == null) _children = new QuadTreeNode[4];

                var pointOnPlane = new Vector2(point.x, point.z);
                var divided = false;
                for (var i = 0; i < _children.Length; i++)
                {
                    if (_children[i] == null) _children[i] = new QuadTreeNode(_childRects[i], _maxDepth);
                    if (_childRects[i].Contains(pointOnPlane))
                    {
                        divided = true;
                        _children[i].AddPointAndDivide(point, depth + 1);
                    }
                }
                if (!divided) _children = null;
            }

            private Vector3 FindPointOrDivide(Vector3 from, Vector3 final, int depth = 0)
            {
                if (depth >= _maxDepth) return final;
                if (_children == null) _children = new QuadTreeNode[4];

                var fromOnPlane = new Vector2(from.x, from.z);
                var finalOnPlane = new Vector2(final.x, final.z);

                for (int i = 0; i < _childRects.Length; i++)
                {
                    if (_children[i] == null) _children[i] = new QuadTreeNode(_childRects[i], _maxDepth);
                    if (_childRects[i].Contains(fromOnPlane) && _childRects[i].Contains(finalOnPlane))
                        return _children[i].FindPointOrDivide(from, final, depth + 1);
                }

                var center = QuadrantCenter(finalOnPlane);
                return new Vector3(center.x, from.y, center.y);

                Vector2 QuadrantCenter(Vector2 point)
                {
                    for (int i = 0; i < _childRects.Length; i++)
                        if (_childRects[i].Contains(point))
                            return _childRects[i].center;
                    return Vector2.zero;
                }
            }

            private void Subdivide()
            {
                var halfX = _nodeRect.size.x * 0.5f;
                var halfY = _nodeRect.size.y * 0.5f;
                var childSize = new Vector2(halfX, halfY);

                _childRects = new Rect[4];
                var idx = 0;
                foreach (var y in new[] { 0, 1 })
                    foreach (var x in new[] { 0, 1 })
                        _childRects[idx++] = new Rect(_nodeRect.position + new Vector2(halfX * x, halfY * y), childSize);
            }

            #endregion
        }

        #endregion
    }
}
