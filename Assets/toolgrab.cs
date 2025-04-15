using UnityEngine;

public class SimpleGrabber : MonoBehaviour
{
    private GameObject grabbedObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tool")) // Make sure the tool has this tag
        {
            grabbedObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tool"))
        {
            grabbedObject = null;
        }
    }

    void Update()
    {
        if (grabbedObject != null)
        {
            grabbedObject.transform.position = transform.position;
            grabbedObject.transform.rotation = transform.rotation;
        }
    }
}