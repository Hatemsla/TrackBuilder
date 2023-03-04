using System;
using UnityEngine;

public class CameraMove  : MonoBehaviour
{
    public Camera mainCamera;
    
    public Vector3 _origin;
    public Vector3 _difference;
    public bool _isDrag;

    private void LateUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            _difference = (mainCamera.ScreenToWorldPoint(Input.mousePosition)) - mainCamera.transform.position;
            if (_isDrag == false)
            {
                _isDrag = true;
                _origin = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            _isDrag = false;
        }

        if (_isDrag)
        {
            mainCamera.transform.position = _origin - _difference;
        }
    }
}