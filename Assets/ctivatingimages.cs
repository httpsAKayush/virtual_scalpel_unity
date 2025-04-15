using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlaneActivator : MonoBehaviour
{
    public GameObject[] planes;        // Drag your 6 planes here
    public Text popupText;             // Assign your UI Text here
    public float popupDuration = 2f;   // How long the message stays visible

    private void Start()
    {
        // Start by hiding all planes
        foreach (GameObject plane in planes)
        {
            plane.SetActive(false);
        }

        // Show popup message
        StartCoroutine(ShowPopupMessage());

        // Start activating planes
        StartCoroutine(ActivatePlanesOneByOne());
    }

    IEnumerator ShowPopupMessage()
    {
        popupText.text = "Cut where marked";
        popupText.gameObject.SetActive(true);

        yield return new WaitForSeconds(popupDuration);

        popupText.gameObject.SetActive(false);
    }

    IEnumerator ActivatePlanesOneByOne()
    {
        for (int i = 0; i < planes.Length; i++)
        {
            yield return new WaitForSeconds(0.7f);
            planes[i].SetActive(true);
        }
    }
}
