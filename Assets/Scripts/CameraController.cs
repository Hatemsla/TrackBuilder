using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool isDrag;
    public EditableObject currentEditableObject;

    private void Update()
    {
        if (Cursor.visible)
            isDrag = false;
        else
            isDrag = true;
    }
}