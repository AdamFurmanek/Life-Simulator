using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocietyGenerator : MonoBehaviour
{
    public GameObject human;

    void Start()
    {
        for(int i = 0; i < 40; i++)
        {
            City.Humans.Add(Instantiate(human, Vector3.zero, Quaternion.Euler(0, 0, 0)));
        }
    }

    void Update()
    {
        
    }
}
