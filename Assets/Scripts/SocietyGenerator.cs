using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocietyGenerator : MonoBehaviour
{
    public GameObject human;

    void Start()
    {
        for(int i = 0; i < 200; i++)
        {
            Building house = City.FindBuilding(BuildingType.House);
            GameObject humanObject = Instantiate(human, house.transform.position, Quaternion.Euler(0, 0, 0));
            humanObject.GetComponent<Human>().SetHouse(house);
            humanObject.GetComponent<Human>().id = i;
            City.humans.Add(humanObject);
        }
    }

}
