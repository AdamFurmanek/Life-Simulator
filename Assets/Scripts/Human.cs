using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Human : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject human, car;
    public Material blue, pink;
    private bool haveCar;
    private bool woman;
    Building house;
    GameObject destination = null;
    GameObject actualPlace = null;

    struct Task
    {
        public System.DateTime startDate, endDate;
        public GameObject place;
    }

    List<Task> schedule = new List<Task>();

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        human = transform.Find("Body").gameObject;
        car = transform.Find("Car").gameObject;
        SetCar(Random.Range(0,2) > 0);
        SetGender(Random.Range(0, 2) > 0);
        FindEmptyHouse();

        //Testing tasks.
        float offset = Random.Range(0, 15);
        schedule.Add(new Task() {startDate = new System.DateTime(0).AddHours(8).AddMinutes(0 + offset), endDate = new System.DateTime(0).AddHours(8).AddMinutes(200 + offset),place = house.gameObject });
        //schedule.Add(new Task() { startDate = new System.DateTime(0).AddMinutes(20 + offset), endDate = new System.DateTime(0).AddMinutes(40 + offset), place = City.houses[Random.Range(0, City.houses.Count)] });
        //schedule.Add(new Task() { startDate = new System.DateTime(0).AddMinutes(40 + offset), endDate = new System.DateTime(0).AddMinutes(60 + offset), place = City.houses[Random.Range(0, City.houses.Count)] });
    }

    void Update()
    {

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(transform.position, out navHit, 1f, NavMesh.AllAreas))
        {
            int mask1 = navHit.mask;
            int index = 0;

            while ((mask1 >>= 1) > 0)
            {
                index++;
            }
            float areaCost = NavMesh.GetAreaCost(index);
            agent.speed = 5 / areaCost * TimeController.globalSpeed;

            if (index == 4)
            {
                human.SetActive(true);
                car.SetActive(false);
            }
            else if(index == 3 && haveCar)
            {
                human.SetActive(false);
                car.SetActive(true);
            }

        }

    }

    void SetCar(bool haveCar)
    {
        agent.areaMask = 0 | (1 << 4) | (1 << 5);
        this.haveCar = haveCar;
        if (haveCar)
        {
            agent.areaMask = agent.areaMask | (1 << 3);
        }
    }

    void SetGender(bool woman)
    {
        this.woman = woman;
        if (woman)
        {
            
            var renderers = human.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material = pink;
            }
            renderers = car.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material = pink;
            }
            
        }
    }

    void FindEmptyHouse()
    {
        Building house = null;
        int tries = 0;
        do
        {
            house = City.houses[Random.Range(0, City.houses.Count)].GetComponent<Building>();
            tries++;
        } while (house.owners.Count > 0 && tries < 2000);
        if (tries < 2000)
            SetHouse(house);
    }

    void SetHouse(Building house)
    {
        this.house = house;
        house.owners.Add(this);
    }

    public void CheckTime(System.DateTime globalTime)
    {
        if(schedule.Count > 0)
        {
            while(schedule[0].endDate <= globalTime)
            {
                Task task = schedule[0];
                task.startDate = task.startDate.AddDays(1);
                task.endDate = task.endDate.AddDays(1);
                schedule.Add(task);
                schedule.RemoveAt(0);
            }
            if(schedule[0].startDate <= globalTime)
            {
                if (schedule[0].place != destination)
                {
                    destination = schedule[0].place;
                    agent.SetDestination(schedule[0].place.transform.position);
                    if (actualPlace != null)
                    {
                        actualPlace.GetComponent<Building>().visitors.Remove(this);
                    }
                    //Debug.Log("Ustalam trasê");
                }
                else if(actualPlace != destination)
                {
                    if(Vector3.Distance(destination.transform.position, transform.position) < 1)
                    {
                        actualPlace = destination;
                        actualPlace.GetComponent<Building>().visitors.Add(this);
                        //Debug.Log("Wszed³em do budynku");
                    }
                }
                else
                {

                    //Debug.Log("Siedzê w budynku");
                }
            }
            else
            {
                //Debug.Log("Nic nie robiê");
                if(actualPlace!= null)
                {
                    actualPlace.GetComponent<Building>().visitors.Remove(this);
                }
                destination = null;
                actualPlace = null;
                agent.SetDestination(transform.position);
            }
        }
    }

}
