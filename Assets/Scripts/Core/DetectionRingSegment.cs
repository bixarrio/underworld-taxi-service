using UnityEngine;

public class DetectionRingSegment : MonoBehaviour
{
    #region Properties and Fields

    [SerializeField] Renderer _renderer;

    #endregion

    #region Public Methods

    public void SetRendererMaterial(Material material)
        => _renderer.material = material;

    public void SetRendererColor(Color color)
        => _renderer.material.color = color;

    #endregion
}
