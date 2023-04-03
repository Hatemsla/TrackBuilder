using System;
using System.Collections;
using System.Collections.Generic;
using Builder;
using UnityEngine;

public class TrackObject : MonoBehaviour
{
    public ObjectsType objectType;
    public float upPointHeight;
    public int rotateStateIndex = 0;
    public int heightStateIndex = 0;
    public bool isActive;

    private Transform _upPointTransform;

    private void Awake()
    {
        _upPointTransform = transform.GetChild(0);
    }

    private void Update()
    {
        upPointHeight = MathF.Round(_upPointTransform.position.y, 2);
    }
}
