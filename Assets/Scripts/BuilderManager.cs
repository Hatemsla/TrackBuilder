using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Builder
{
    public class BuilderManager : MonoBehaviour
    {
        public float gridSize;
        public bool canPlace;
        public LayerMask layerMask;
        public GameObject[] objects;
        public GameObject pendingObject;
        public ObjectsType objectsType;

        public Vector3 _pos;
        private RaycastHit _hit;
        private int _currentIndex;

        private void Update()
        {
            if (pendingObject != null)
            {
                if (_currentIndex == 0)
                {
                    pendingObject.transform.position = new Vector3
                    (
                        RoundToNearsGrid(_pos.x),
                        RoundToNearsGrid(_pos.y),
                        RoundToNearsGrid(_pos.z)
                    );
                }
                else
                {
                    pendingObject.transform.position = new Vector3
                    (
                        RoundToNearsGrid(_pos.x),
                        RoundToNearsGrid(_pos.y),
                        RoundToNearsGrid(_pos.z)
                    );
                }

                if (Input.GetMouseButtonDown(0) && canPlace)
                {
                    PlaceObject();
                }

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    RotateObject(-90);
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    RotateObject(90);
                }
            }
        }

        public void PlaceObject()
        {
            pendingObject = null;
        }

        public void RotateObject(float rotateAmount)
        {
            pendingObject.transform.Rotate(Vector3.up, rotateAmount);
        }

        private void FixedUpdate()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out _hit, 10000, layerMask))
            {
                _pos = _hit.point;
                
            }
        }

        public void SelectObject(int index)
        {
            _currentIndex = index;
            pendingObject = Instantiate(objects[index], _pos, transform.rotation);
        }

        public float RoundToNearsGrid(float pos)
        {
            float xDiff = pos % gridSize;
            pos -= xDiff;

            if (xDiff > (gridSize / 2))
            {
                pos += gridSize;
            }

            return pos;
        }
    }
}