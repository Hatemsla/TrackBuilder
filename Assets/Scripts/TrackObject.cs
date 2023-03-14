using System;
using System.Collections;
using System.Collections.Generic;
using Builder;
using UnityEngine;

public class TrackObject : MonoBehaviour
{
    public ObjectsType objectType;
    public float upPointHeight;

    private Transform _upPointTransform;

    private void Awake()
    {
        _upPointTransform = transform.GetChild(0);
    }

    private void Update()
    {
        upPointHeight = _upPointTransform.position.y;
    }
}
