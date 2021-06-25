using UnityEngine;
using UnityEngine.AI;

public class Human : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject human, car;
    public Material blue, pink;
    private bool haveCar;
    private bool woman;

    bool done = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        human = transform.Find("Body").gameObject;
        car = transform.Find("Car").gameObject;
        SetCar(true);
        SetGender(true);
    }

    private void Update()
    {
        if (!done)
        {
            agent.SetDestination(City.Houses[Random.Range(0, City.Houses.Count)].transform.position);
            done = !done;
        }

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
            agent.speed = 5 / areaCost;

            if(index == 4)
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

    private void SetCar(bool haveCar)
    {
        agent.areaMask = 0 | (1 << 4) | (1 << 5);
        this.haveCar = haveCar;
        if (haveCar)
        {
            agent.areaMask = agent.areaMask | (1 << 3);
        }
    }

    private void SetGender(bool woman)
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

}
