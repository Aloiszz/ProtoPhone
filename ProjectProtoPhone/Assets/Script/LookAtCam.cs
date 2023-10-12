using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera _camera;
    private Transform _camTransform;

    private void Start()
    {
        if (Camera.main != null) _camera = Camera.main;
        _camTransform = _camera.transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + _camTransform.forward);
    }
}
