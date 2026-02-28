using UnityEngine;

public class BillboardWaterDropletIcon : MonoBehaviour
{
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
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
=======
=======
>>>>>>> 4e4296b (Apply PR review feedback: null safety, operator precedence, billboard class name, seed count init)
=======
>>>>>>> 4e4296b (Apply PR review feedback: null safety, operator precedence, billboard class name, seed count init)
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
<<<<<<< HEAD
<<<<<<< HEAD
>>>>>>> 44c1d0a (Address PR review comments: fix bugs in Plant, FarmTile, BillboardWaterDropletIcon, HotbarUI)
=======
=======
>>>>>>> 4e4296b (Apply PR review feedback: null safety, operator precedence, billboard class name, seed count init)
=======
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
>>>>>>> 7545cdd (Apply PR review feedback: null safety, operator precedence, billboard class name, seed count init)
<<<<<<< HEAD
>>>>>>> 4e4296b (Apply PR review feedback: null safety, operator precedence, billboard class name, seed count init)
=======
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
>>>>>>> 44c1d0a (Address PR review comments: fix bugs in Plant, FarmTile, BillboardWaterDropletIcon, HotbarUI)
=======
>>>>>>> 4e4296b (Apply PR review feedback: null safety, operator precedence, billboard class name, seed count init)
        }
    }
}