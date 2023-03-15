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
        public Dictionary<float, bool> UpPointHeights;
        public Dictionary<GameObject, float> Grounds;
        
        private int _currentGroundIndex;
        private Vector3 _pos;
        private RaycastHit _hit;

        private void Start()
        {
            UpPointHeights = new Dictionary<float, bool> { { 0f, true } };
            Grounds = new Dictionary<GameObject, float> { { groundPrefab, 0f } };
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
            else if (currentObjectType.objectType == ObjectsType.Slant)
            {
                var yOffset = currentObjectType.rotateStateIndex switch
                {
                    -1 => 1f,
                    0 => 1.35f,
                    _ => 1.65f
                };

                var y = Grounds.ElementAt(_currentGroundIndex).Value + yOffset;
                pendingObject.transform.position = new Vector3
                (
                    RoundToNearsGrid(_pos.x),
                    y,
                    RoundToNearsGrid(_pos.z)
                );
            }
            else
            {
                var y = Grounds.ElementAt(_currentGroundIndex).Value + 1.35f;
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
                RotateObject(Vector3.up, -90, Space.World);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                RotateObject(Vector3.up, 90, Space.World);
            }

            if (currentObjectType.objectType == ObjectsType.Slant)
            {
                if (Input.GetKeyDown(KeyCode.Z) && currentObjectType.rotateStateIndex >= 0)
                {
                    currentObjectType.rotateStateIndex--;
                    RotateObject(Vector3.forward, -20f, Space.Self);
                }
                else if (Input.GetKeyDown(KeyCode.X) && currentObjectType.rotateStateIndex <= 0)
                {
                    currentObjectType.rotateStateIndex++;
                    RotateObject(Vector3.forward, 20f, Space.Self);
                }
            }
        }

        private void PlaceObject()
        {
            if (currentObjectType.objectType == ObjectsType.Slant && !UpPointHeights.ContainsKey(MathF.Round(currentObjectType.upPointHeight, 2)))
            {
                UpPointHeights.Add(MathF.Round(currentObjectType.upPointHeight, 2), false);
                UpPointHeights = new Dictionary<float, bool>(UpPointHeights.OrderBy(x => x.Key));
            }
            
            pendingObject = null;
        }

        private void RotateObject(Vector3 axis, float rotateAmount, Space space)
        {
            pendingObject.transform.Rotate(axis, rotateAmount, space);
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
                for (var i = 0; i < Grounds.Count; i++)
                {
                    if(i != _currentGroundIndex)
                        Grounds.ElementAt(i).Key.SetActive(false);
                    else
                    {
                        Grounds.ElementAt(i).Key.SetActive(true);
                        _currentGroundIndex = i;
                        break;
                    }
                }
            }
            else
            {
                var newGround = Instantiate(groundPrefab,
                    new Vector3(0, UpPointHeights.ElementAt(_currentGroundIndex).Key, 0), Quaternion.identity);

                Grounds.Add(newGround, UpPointHeights.ElementAt(_currentGroundIndex).Key);
                Grounds = new Dictionary<GameObject, float>(Grounds.OrderBy(x => x.Value));
                UpPointHeights[UpPointHeights.ElementAt(_currentGroundIndex).Key] = true;

                foreach (var ground in Grounds)
                {
                    ground.Key.SetActive(ground.Key == newGround);

                    if (ground.Key == newGround)
                        _currentGroundIndex = Grounds.Keys.ToList().IndexOf(ground.Key);
                }
            }
        }

        public void DownFloor()
        {
            var isGroundExist = Grounds.ContainsKey(Grounds.ElementAt(_currentGroundIndex - 1).Key);
            
            if(!isGroundExist)
                return;

            _currentGroundIndex--;
            
            for (var i = 0; i < Grounds.Count; i++)
            {
                if(i != _currentGroundIndex)
                    Grounds.ElementAt(i).Key.SetActive(false);
                else
                {
                    Grounds.ElementAt(i).Key.SetActive(true);
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