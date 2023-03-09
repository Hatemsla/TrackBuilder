using System;
using System.Collections.Generic;
using UnityEngine;

namespace Builder
{
    public class BuilderManager : MonoBehaviour
    {
        public float gridSize;
        public bool canPlace;
        public LayerMask layerMask;
        public GameObject[] objects;
        public GameObject pendingObject;
        public TrackObject currentObject;
        public GameObject groundPrefab;
        public List<GameObject> grounds;

        private GameObject _currentGround;
        private int _currentGroundIndex;
        private int _currentObjectIndex;
        private Vector3 _pos;
        private RaycastHit _hit;

        private void Update()
        {
            if (pendingObject != null)
            {
                if (currentObject.objectType == ObjectsType.Floor)
                {
                    pendingObject.transform.position = new Vector3
                    (
                        RoundToNearsGrid(_pos.x),
                        _pos.y,
                        RoundToNearsGrid(_pos.z)
                    );
                }
                else
                {
                    var y = grounds[_currentGroundIndex].transform.position.y + 1.35f;
                    pendingObject.transform.position = new Vector3
                    (
                        RoundToNearsGrid(_pos.x),
                        y,
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

        private void PlaceObject()
        {
            pendingObject = null;
        }

        private void RotateObject(float rotateAmount)
        {
            pendingObject.transform.Rotate(Vector3.up, rotateAmount, Space.World);
        }

        private void FixedUpdate()
        {
            Ray ray = Camera.main!.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out _hit, 10000, layerMask))
            {
                _pos = _hit.point;
            }
        }

        public void SelectObject(int index)
        {
            _currentObjectIndex = index;
            pendingObject = Instantiate(objects[index], _pos, transform.rotation);
            currentObject = pendingObject.GetComponent<TrackObject>();
        }

        public void UpFloor()
        {
            if (_currentGroundIndex != grounds.Count - 1)
            {
                for (var i = 0; i < grounds.Count; i++)
                {
                    if(i != _currentGroundIndex + 1)
                        grounds[i].SetActive(false);
                    else
                    {
                        grounds[i].SetActive(true);
                        _currentGround = grounds[i];
                        _currentGroundIndex = i;
                        break;
                    }
                }
            }
            else
            {
                var newGround = Instantiate(groundPrefab,
                    new Vector3(0, grounds[^1].transform.position.y + 2.6f, 0), Quaternion.identity);

                grounds.Add(newGround);
                _currentGroundIndex = grounds.Count - 1;
                _currentGround = newGround;

                foreach (var ground in grounds)
                {
                    ground.SetActive(ground == newGround);
                }
            }
        }

        public void DownFloor()
        {
            var prevGroundIndex = grounds.IndexOf(_currentGround) - 1;
            
            if(prevGroundIndex < 0)
                return;

            _currentGroundIndex = prevGroundIndex;
            
            for (var i = 0; i < grounds.Count; i++)
            {
                if(i != prevGroundIndex)
                    grounds[i].SetActive(false);
                else
                {
                    grounds[i].SetActive(true);
                    _currentGround = grounds[i];
                }
            }
        }

        private float RoundToNearsGrid(float pos)
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