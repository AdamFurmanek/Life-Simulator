using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public DateTime globalTime = new DateTime(0);
    public static float globalSpeed = 1.0f;
    public GameObject clock;
    public GameObject light;

    void Start()
    {
        globalTime = globalTime.AddHours(8);
        StartCoroutine(Timer());
    }

    private void Update()
    {
        globalTime = globalTime.AddSeconds(Time.deltaTime * 60 * globalSpeed);
        light.transform.eulerAngles = Vector3.right * (((float)globalTime.Second + globalTime.Minute * 60 + globalTime.Hour * 3600) * 360 / 86400 + 270);
        clock.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = (globalTime.Hour < 10 ? "0" : "") + globalTime.Hour + ":" + (globalTime.Minute < 10 ? "0" : "") + globalTime.Minute;
    }

    IEnumerator Timer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            foreach (GameObject humanObject in City.humans)
            {
                humanObject.GetComponent<Human>().CheckTime(globalTime);
            }
        }
    }
}
