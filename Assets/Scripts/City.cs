using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class City
{
    public static List<GameObject> houses = new List<GameObject>();
    public static List<GameObject> workplaces = new List<GameObject>();
    public static List<GameObject> publicSpaces = new List<GameObject>();
    public static List<GameObject> constructionSites = new List<GameObject>();
    public static List<GameObject> humans = new List<GameObject>();

    public static Building FindBuilding(BuildingType type)
    {
        List<GameObject> bouildings = null;
        if (type == BuildingType.House)
            bouildings = houses;
        else if (type == BuildingType.Workplace)
            bouildings = workplaces;
        else
            bouildings = publicSpaces;

        Building building = null;
        int tries = 0;
        do
        {
            building = bouildings[UnityEngine.Random.Range(0, bouildings.Count)].GetComponent<Building>();
            tries++;
        } while (building.owners.Count > 0 && tries < 100);
        return building;
    }
}

