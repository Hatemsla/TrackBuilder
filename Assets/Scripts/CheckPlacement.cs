using System;
using System.Collections;
using System.Collections.Generic;
using Builder;
using UnityEngine;

public class CheckPlacement : MonoBehaviour
{
    private BuilderManager _builderManager;
    
    private void Start()
    {
        _builderManager = FindObjectOfType<BuilderManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Track"))
        {
            _builderManager.canPlace = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Track"))
        {
            _builderManager.canPlace = true;
        }
    }
}
