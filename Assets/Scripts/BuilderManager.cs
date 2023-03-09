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
                        RoundToNearsGrid(_pos.y),
                        RoundToNearsGrid(_pos.z)
                    );
                }
                else
                {
                    pendingObject.transform.position = new Vector3
                    (
                        RoundToNearsGrid(_pos.x),
                        1.35f,
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
            pendingObject = Instantiate(objects[index], _pos, transform.rotation);
            currentObject = pendingObject.GetComponent<TrackObject>();
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