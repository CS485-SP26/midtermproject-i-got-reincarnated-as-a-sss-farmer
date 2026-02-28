using UnityEngine;

public class BillboardWaterDropletIcon : MonoBehaviour
public class BillboardWaterDropletIcon : MonoBehaviour
{
    private Transform _cameraTransform;

    private void Awake()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            _cameraTransform = mainCamera.transform;
        }
    }

    void LateUpdate()
    {
        if (_cameraTransform != null)
        {
            transform.forward = _cameraTransform.forward;
        }
    }
}