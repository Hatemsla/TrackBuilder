using System;
using UnityEngine;

namespace Builder
{
    public class Connection : MonoBehaviour
    {
        public bool isLinked;
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
                    isLinked = true;
                    other.GetComponent<Connection>().isLinked = true;
                    _trackObject.transform.position = other.transform.position;
                    _trackObject.transform.rotation = other.transform.rotation;
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
                        isLinked = false;
                        other.GetComponent<Connection>().isLinked = false;
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_trackObject.isActive)
            {
                if (_trackObject.objectType == ObjectsType.Floor &&
                    other.gameObject.layer == LayerMask.NameToLayer("FloorConnection"))
                {
                    isLinked = false;
                    other.GetComponent<Connection>().isLinked = false;
                }
            }
        }
    }
}