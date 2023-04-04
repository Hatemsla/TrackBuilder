using System;
using UnityEngine;

namespace Builder
{
    public class Connection : MonoBehaviour
    {
        private TrackObject _trackObject;
        private BuilderManager _builderManager;
        private Selection _selection;

        private void Start()
        {
            _trackObject = GetComponentInParent<TrackObject>();
            _builderManager = FindObjectOfType<BuilderManager>();
            _selection = FindObjectOfType<Selection>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_trackObject.isActive)
            {
                if (_trackObject.objectType == ObjectsType.Floor &&
                    other.gameObject.layer == LayerMask.NameToLayer("FloorConnection"))
                {
                    _builderManager.PlaceObject();
                    _trackObject.transform.position = other.transform.position;
                }
                else if (_trackObject.objectType == ObjectsType.Wall &&
                         other.gameObject.layer == LayerMask.NameToLayer("WallConnection"))
                {
                    _builderManager.PlaceObject();
                    _trackObject.transform.position = other.transform.position;
                }
                else if (_trackObject.objectType == ObjectsType.Slant &&
                         other.gameObject.layer == LayerMask.NameToLayer("WallConnection"))
                {
                    _builderManager.PlaceObject();
                    _trackObject.transform.position = other.transform.position;
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (_trackObject.isActive)
            {
                if (_trackObject.objectType == ObjectsType.Floor &&
                    other.gameObject.layer == LayerMask.NameToLayer("FloorConnection"))
                {
                    if (Vector3.Distance(other.transform.position, _builderManager.mousePos) > 3)
                    {
                        _selection.Move();
                    }
                }
                else if (_trackObject.objectType == ObjectsType.Wall &&
                         other.gameObject.layer == LayerMask.NameToLayer("WallConnection"))
                {
                    if (Vector3.Distance(other.transform.position, _builderManager.mousePos) > 3)
                    {
                        _selection.Move();
                    }
                }
                else if (_trackObject.objectType == ObjectsType.Slant &&
                         other.gameObject.layer == LayerMask.NameToLayer("WallConnection"))
                {
                    if (Vector3.Distance(other.transform.position, _builderManager.mousePos) > 4)
                    {
                        _selection.Move();
                    }
                }
            }
        }
    }
}