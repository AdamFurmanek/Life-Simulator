using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public DateTime globalTime = new DateTime(0);
    public static float globalSpeed = 1.0f;

    void Start()
    {
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / globalSpeed);
            globalTime = globalTime.AddMinutes(1);
            Debug.Log(globalTime);
            foreach (GameObject humanObject in City.humans)
            {
                humanObject.GetComponent<Human>().CheckTime(globalTime);
            }
        }
    }
}
