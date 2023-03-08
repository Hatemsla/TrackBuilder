using UnityEngine;

namespace Builder
{
    public class Selection : MonoBehaviour
    {
        public GameObject selectedObject;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 10000))
                    if (hit.collider.gameObject.CompareTag("Track"))
                        Select(hit.collider.gameObject);
            }

            if (Input.GetMouseButtonDown(1))
            {
                Deselect();
            }
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