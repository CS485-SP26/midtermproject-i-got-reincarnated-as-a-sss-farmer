using UnityEngine;

public class BillboardWaterDropletIcon : MonoBehaviour
{
<<<<<<< HEAD
<<<<<<< HEAD
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
=======
=======
>>>>>>> 4241b9e09a5b44f57f7961716adeddcec728b790
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void LateUpdate()
    {
        if (_mainCamera != null)
        {
            transform.forward = _mainCamera.transform.forward;
<<<<<<< HEAD
>>>>>>> 7545cdd (Apply PR review feedback: null safety, operator precedence, billboard class name, seed count init)
=======
>>>>>>> 4241b9e09a5b44f57f7961716adeddcec728b790
        }
    }
}