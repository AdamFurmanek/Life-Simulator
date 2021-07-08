using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrackingInfo : MonoBehaviour
{
    private TextMeshProUGUI name, place;
    private GameObject trackedObject = null;

    private void Start()
    {
        name = transform.Find("Name").GetComponent<TextMeshProUGUI>();
        place = transform.Find("Place").GetComponent<TextMeshProUGUI>();
        this.gameObject.SetActive(false);
    }

    public void SetObject(GameObject trackedObject)
    {
        this.trackedObject = trackedObject;
        gameObject.SetActive(true);

    }

    public void Update()
    {
        if(trackedObject != null)
        {
            Debug.Log("elo");
            name.text = "Human " + trackedObject.GetComponent<Human>().id;
            if (trackedObject.GetComponent<Human>().destination != null)
                place.text = trackedObject.GetComponent<Human>().destination.GetComponent<Building>().type.ToString();
            else if (trackedObject.GetComponent<Human>().actualPlace != null)
                place.text = trackedObject.GetComponent<Human>().actualPlace.GetComponent<Building>().type.ToString();

            else
                place.text = ".";
        }
    }

    public void RemoveObject()
    {
        trackedObject = null;
        gameObject.SetActive(false);
    }

}
