using UnityEngine;

public class SimpleDrag : MonoBehaviour
{
    public GameObject objectToDrag;
    public Camera mainCamera;

    private bool isDragging = false;
    private Vector3 offset;
    private float objectZ;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.gameObject == objectToDrag)
                {
                    isDragging = true;
                    objectZ = mainCamera.WorldToScreenPoint(objectToDrag.transform.position).z;
                    Vector3 worldPoint = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, objectZ));
                    offset = objectToDrag.transform.position - worldPoint;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, objectZ);
            Vector3 newWorldPosition = mainCamera.ScreenToWorldPoint(screenPoint) + offset;
            objectToDrag.transform.position = newWorldPosition;
        }
    }
}
