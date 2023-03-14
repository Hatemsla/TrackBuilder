using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Builder
{
    public class BuilderManager : MonoBehaviour
    {
        public float gridSize;
        public bool canPlace;
        public LayerMask layerMask;
        public GameObject pendingObject;
        public TrackObject currentObjectType;
        public GameObject groundPrefab;
        public GameObject[] objects;
        public List<GameObject> grounds;
        // public List<float> upPointHeights;
        public Dictionary<float, bool> UpPointHeights;

        private GameObject _currentGround;
        private int _currentGroundIndex;
        private Vector3 _pos;
        private RaycastHit _hit;

        private void Start()
        {
            UpPointHeights = new Dictionary<float, bool> { { 0f, true } };
        }

        private void Update()
        {
            if (pendingObject == null) return;
            if (currentObjectType.objectType == ObjectsType.Floor)
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
                RotateObject(Vector3.up, -90);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                RotateObject(Vector3.up, 90);
            }

            if (currentObjectType.objectType == ObjectsType.Slant)
            {
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    if (Mathf.Floor(currentObjectType.transform.localEulerAngles.z) <= 0)
                    {
                        RotateObject(Vector3.forward, 20);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.X))
                {
                    if (Mathf.Floor(360f - currentObjectType.transform.localEulerAngles.z) >= 0)
                    {
                        RotateObject(Vector3.forward, -20);
                    }
                }
            }
        }

        private void PlaceObject()
        {
            if (currentObjectType.objectType == ObjectsType.Slant && !UpPointHeights.ContainsKey(currentObjectType.upPointHeight))
            {
                UpPointHeights.Add(MathF.Round(currentObjectType.upPointHeight, 2), false);
                UpPointHeights = new Dictionary<float, bool>(UpPointHeights.OrderBy(x => x.Key));
            }
            
            pendingObject = null;
        }

        private void RotateObject(Vector3 axis, float rotateAmount)
        {
            pendingObject.transform.Rotate(axis, rotateAmount, Space.World);
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
            pendingObject = Instantiate(objects[index], _pos, transform.rotation);
            currentObjectType = pendingObject.GetComponent<TrackObject>();
        }

        public void UpFloor()
        {
            if(_currentGroundIndex == UpPointHeights.Count - 1) return;

            _currentGroundIndex++;
            if (UpPointHeights.ElementAt(_currentGroundIndex).Value)
            {
                for (var i = 0; i < grounds.Count; i++)
                {
                    if(i != _currentGroundIndex)
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
                    new Vector3(0, UpPointHeights.ElementAt(_currentGroundIndex).Key, 0), Quaternion.identity);

                grounds.Add(newGround);
                _currentGroundIndex = grounds.Count - 1;
                UpPointHeights[UpPointHeights.ElementAt(_currentGroundIndex).Key] = true;
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