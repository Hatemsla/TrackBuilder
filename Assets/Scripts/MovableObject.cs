using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : MonoBehaviour
{
    public Camera mainCamera;
    public LayerMask mouseColliderLayerMask = new LayerMask();

    private bool _isSelect;

    private void Update()
    {
        if (_isSelect)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 1000f, mouseColliderLayerMask))
            {
                transform.position = raycastHit.point + new Vector3(0, 0.5f, 0);
            }
        }
    }

    private void OnMouseDown()
    {
        _isSelect = !_isSelect;

        if (_isSelect)
            gameObject.layer = LayerMask.NameToLayer("Default");
        else
            gameObject.layer = LayerMask.NameToLayer("Track");
    }
}
