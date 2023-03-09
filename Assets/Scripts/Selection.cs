using System;
using UnityEngine;

namespace Builder
{
    public class Selection : MonoBehaviour
    {
        public GameObject selectedObject;
        private BuilderManager _builderManager;

        private void Start()
        {
            _builderManager = FindObjectOfType<BuilderManager>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main!.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 10000))
                    if (hit.collider.gameObject.CompareTag("Track"))
                        Select(hit.collider.gameObject);
            }

            if (Input.GetMouseButtonDown(1) && selectedObject != null)
            {
                Deselect();
            }
        }

        public void Move()
        {
            _builderManager.pendingObject = selectedObject.transform.root.gameObject;
            _builderManager.currentObject = selectedObject.GetComponentInParent<TrackObject>();
        }

        public void Delete()
        {
            var obj = selectedObject;
            Deselect();
            Destroy(obj);
        }

        private void Select(GameObject obj)
        {
            if (obj == selectedObject)
                return;
            
            if(selectedObject != null)
                Deselect();

            var outline = obj.GetComponent<Outline>();
            if (outline == null)
                obj.AddComponent<Outline>();
            else
                outline.enabled = true;
            selectedObject = obj;
        }

        private void Deselect()
        {
            selectedObject.GetComponent<Outline>().enabled = false;
            selectedObject = null;
        }
    }
}