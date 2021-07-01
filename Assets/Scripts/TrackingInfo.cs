using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrackingInfo : MonoBehaviour
{
    private TextMeshProUGUI name, place;

    private void Start()
    {
        name = transform.Find("Name").GetComponent<TextMeshProUGUI>();
        place = transform.Find("Place").GetComponent<TextMeshProUGUI>();
        this.gameObject.SetActive(false);
    }

    public void SetObject(GameObject trackedObject)
    {
        gameObject.SetActive(true);
        name.text = "name";
        place.text = "place";
    }

    public void RemoveObject()
    {
        gameObject.SetActive(false);
    }

}
