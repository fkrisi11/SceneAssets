#if UNITY_EDITOR
using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public class MirrorCamera : MonoBehaviour
{
    [Header("Flip Options")]
    [SerializeField] private bool flipHorizontal = true;
    [SerializeField] private bool flipVertical = false;

    private Camera _cam;

    private static int _invertCullingRefCount = 0;

    private void OnEnable()
    {
        _cam = GetComponent<Camera>();
        _cam.ResetProjectionMatrix();
    }

    private void OnDisable()
    {
        if (_cam != null)
            _cam.ResetProjectionMatrix();

        _invertCullingRefCount = 0;
        GL.invertCulling = false;
    }

    private void OnPreCull()
    {
        if (_cam == null) _cam = GetComponent<Camera>();

        _cam.ResetProjectionMatrix();

        if (!flipHorizontal && !flipVertical)
            return;

        float sx = flipHorizontal ? -1f : 1f;
        float sy = flipVertical ? -1f : 1f;

        _cam.projectionMatrix = _cam.projectionMatrix * Matrix4x4.Scale(new Vector3(sx, sy, 1f));
    }

    private void OnPreRender()
    {
        bool oddFlip = flipHorizontal ^ flipVertical;
        if (!oddFlip) return;

        _invertCullingRefCount++;
        GL.invertCulling = true;
    }

    private void OnPostRender()
    {
        if (_cam == null) _cam = GetComponent<Camera>();

        _cam.ResetProjectionMatrix();

        bool oddFlip = flipHorizontal ^ flipVertical;
        if (!oddFlip) return;

        _invertCullingRefCount = Mathf.Max(0, _invertCullingRefCount - 1);
        if (_invertCullingRefCount == 0)
            GL.invertCulling = false;
    }
}
#endif