using System;
using UnityEngine;

public class EditableObject : MonoBehaviour
{
    public bool isSelect;
    public Camera mainCamera;
    public LayerMask mouseColliderLayerMask;
    public EditableType editableType;
    public float rotationSpeed;

    private CameraController _cameraController;

    private void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
        _cameraController = mainCamera.GetComponent<CameraController>();
    }

    private void Update()
    {
        if (_cameraController.isDrag)
            isSelect = false;
        
        if (isSelect)
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var raycastHit, 1000f, mouseColliderLayerMask))
            {
                if (editableType == EditableType.Floor)
                {
                    transform.position = raycastHit.point + new Vector3(0, transform.localScale.y / 2, 0);
                }
                else if (editableType == EditableType.Slant)
                {
                    transform.position = raycastHit.point + new Vector3(0, transform.localScale.z / 4, 0);
                }
                else if (editableType == EditableType.Wall)
                {
                    transform.position = raycastHit.point + new Vector3(0, transform.localScale.y / 2, 0);
                }
            }
            
            Rotate();
        }
    }

    private void Rotate()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            if (editableType == EditableType.Floor)
            {
                transform.Rotate(Vector3.up, -Time.deltaTime * rotationSpeed, Space.World);
            }
            else if(editableType == EditableType.Slant)
            {
                transform.Rotate(Vector3.up, -Time.deltaTime * rotationSpeed, Space.World);
            }
            else if (editableType == EditableType.Wall)
            {
                transform.Rotate(Vector3.up, -Time.deltaTime * rotationSpeed, Space.World);
            }
        }
        else if (Input.GetKey(KeyCode.E))
        {
            if (editableType == EditableType.Floor)
            {
                transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed, Space.World);
            }
            else if(editableType == EditableType.Slant)
            {
                transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed, Space.World);
            }
            else if (editableType == EditableType.Wall)
            {
                transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed, Space.World);
            }
        }
        else if (Input.GetKey(KeyCode.Z))
        {
            if(editableType == EditableType.Slant)
            {
                transform.Rotate(Vector3.right, Time.deltaTime * rotationSpeed);
            }
        }
        else if (Input.GetKey(KeyCode.X))
        {
            if(editableType == EditableType.Slant)
            {
                transform.Rotate(Vector3.right, -Time.deltaTime * rotationSpeed);
            }
        }
    }

    private void OnMouseDown()
    {
        isSelect = !isSelect;
        _cameraController.currentEditableObject = this;

        UpdateLayerMask();
    }

    public void UpdateLayerMask()
    {
        if(isSelect)
            gameObject.layer = LayerMask.NameToLayer("Default");
        else
            gameObject.layer = LayerMask.NameToLayer("Track");
    }
}