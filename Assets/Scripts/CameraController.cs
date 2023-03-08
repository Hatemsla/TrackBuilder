using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool isDrag;
    
    private void Update()
    {
        if (Cursor.visible)
            isDrag = false;
        else
            isDrag = true;
        
    }
}