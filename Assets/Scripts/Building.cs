using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BuildingType
{
    House,
    Workplace,
    PublicSpace,
    ConstructionSite
};

public class Building : MonoBehaviour
{
    public BuildingType type;
    public List<Human> owners;
    public List<Human> visitors;

    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
