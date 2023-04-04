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
        public Vector3 mousePos;
        public GameObject[] objects;
        public List<GameObject> objectsPool;
        public Dictionary<float, bool> UpPointHeights;
        public Dictionary<GameObject, float> Grounds;
        
        private int _currentGroundIndex;
        private RaycastHit _hit;
        private float _heightGateOffset = 2f;
        private BuilderUI _builderUI;
        private Selection _selection;
            
        private void Start()
        {
            _selection = FindObjectOfType<Selection>();
            _builderUI = FindObjectOfType<BuilderUI>();
            UpPointHeights = new Dictionary<float, bool> { { 0f, true } };
            Grounds = new Dictionary<GameObject, float> { { groundPrefab, 0f } };
        }

        private void Update()
        {
            Ray ray = Camera.main!.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out _hit, 10000, layerMask))
            {
                mousePos = _hit.point;
                
                if (pendingObject == null) return;
                
                if (LayerMask.LayerToName(_hit.collider.gameObject.layer) == "FloorConnection" && currentObjectType.objectType == ObjectsType.Floor)
                {
                    // pendingObject.transform.position = _hit.collider.transform.position;
                    // pendingObject.transform.rotation = _hit.collider.transform.rotation;
                }
                else
                {
                    switch (currentObjectType.objectType)
                    {
                        case ObjectsType.Floor:
                            pendingObject.transform.position = _hit.point;
                            break;
                        case ObjectsType.Wall:
                            pendingObject.transform.position =
                                new Vector3(_hit.point.x, _hit.point.y + 1.25f, _hit.point.z);
                            break;
                        case ObjectsType.Slant:
                            pendingObject.transform.position =
                                new Vector3(_hit.point.x, _hit.point.y + 1.25f, _hit.point.z);
                            break;
                        default:
                            pendingObject.transform.position = _hit.point;
                            break;
                    }
                }
            }
            
            // switch (currentObjectType.objectType)
            // {
            //     case ObjectsType.Floor:
            //         pendingObject.transform.position = new Vector3
            //         (
            //             RoundToNearsGrid(_pos.x),
            //             _pos.y,
            //             RoundToNearsGrid(_pos.z)
            //         );
            //         break;
            //     case ObjectsType.Slant:
            //     {
            //         var yOffset = currentObjectType.rotateStateIndex switch
            //         {
            //             -1 => 1f,
            //             0 => 1.35f,
            //             _ => 1.65f
            //         };
            //
            //         var y = Grounds.ElementAt(_currentGroundIndex).Value + yOffset;
            //         pendingObject.transform.position = new Vector3
            //         (
            //             RoundToNearsGrid(_pos.x),
            //             y,
            //             RoundToNearsGrid(_pos.z)
            //         );
            //         break;
            //     }
            //     case ObjectsType.Gate:
            //     {
            //         _heightGateOffset = currentObjectType.heightStateIndex switch
            //         {
            //             -1 => 1.375f,
            //             0 => 2f,
            //             _ => 3f
            //         };
            //
            //         var y = Grounds.ElementAt(_currentGroundIndex).Value + _heightGateOffset;
            //         pendingObject.transform.position = new Vector3
            //         (
            //             RoundToNearsGrid(_pos.x),
            //             y,
            //             RoundToNearsGrid(_pos.z)
            //         );
            //         break;
            //     }
            //     default:
            //     {
            //         var y = Grounds.ElementAt(_currentGroundIndex).Value + 1.35f;
            //         pendingObject.transform.position = new Vector3
            //         (
            //             RoundToNearsGrid(_pos.x),
            //             y,
            //             RoundToNearsGrid(_pos.z)
            //         );
            //         break;
            //     }
            // }

            if (Input.GetMouseButtonDown(0) && canPlace)
            {
                PlaceObject();
            }
                
            if (Input.GetKeyDown(KeyCode.Q))
            {
                RotateObject(Vector3.up, currentObjectType.objectType == ObjectsType.Gate ? -45 : -90, Space.World);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                RotateObject(Vector3.up, currentObjectType.objectType == ObjectsType.Gate ? 45 : 90, Space.World);
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                switch (currentObjectType.objectType)
                {
                    case ObjectsType.Slant when currentObjectType.rotateStateIndex >= 0:
                        currentObjectType.rotateStateIndex--;
                        RotateObject(Vector3.forward, -20f, Space.Self);
                        break;
                    case ObjectsType.Gate when currentObjectType.heightStateIndex <= 0:
                        currentObjectType.heightStateIndex++;
                        break;
                }
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                switch (currentObjectType.objectType)
                {
                    case ObjectsType.Slant when currentObjectType.rotateStateIndex <= 0:
                        currentObjectType.rotateStateIndex++;
                        RotateObject(Vector3.forward, 20f, Space.Self);
                        break;
                    case ObjectsType.Gate when currentObjectType.heightStateIndex >= 0:
                        currentObjectType.heightStateIndex--;
                        break;
                }
            }
        }

        public void PlaceObject()
        {
            // if (currentObjectType.objectType == ObjectsType.Slant && !UpPointHeights.ContainsKey(MathF.Round(currentObjectType.upPointHeight, 2)))
            // {
            //     UpPointHeights.Add(MathF.Round(currentObjectType.upPointHeight, 2), false);
            //     UpPointHeights = new Dictionary<float, bool>(UpPointHeights.OrderBy(x => x.Key));
            // }

            try
            {
                ChangeLayerRecursively(pendingObject.transform, LayerMask.NameToLayer("TrackGround"));
                currentObjectType = null;
                pendingObject = null;
            }
            catch
            {
                // ignored
            }
        }
        
        public void ChangeLayerRecursively(Transform obj, int layer)
        {
            if (LayerMask.LayerToName(obj.gameObject.layer) != "FloorConnection" && LayerMask.LayerToName(obj.gameObject.layer) != "WallConnection")
            {
                obj.gameObject.layer = layer;
            }

            foreach (Transform child in obj)
            {
                ChangeLayerRecursively(child, layer);
            }
        }

        private void RotateObject(Vector3 axis, float rotateAmount, Space space)
        {
            pendingObject.transform.Rotate(axis, rotateAmount, space);
        }

        public void ConfirmGridSize()
        {
            gridSize = Convert.ToSingle(_builderUI.gridSizeInput.text.Replace('.', ','));
        }

        public void SelectObject(int index)
        {
            pendingObject = Instantiate(objects[index], mousePos, transform.rotation);
            objectsPool.Add(pendingObject);
            _selection.Deselect();
            _selection.Select(pendingObject);
            currentObjectType = pendingObject.GetComponent<TrackObject>();
            currentObjectType.isActive = true;
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