using UnityEngine;
using UnityEngine.AI;

public class Human : MonoBehaviour
{
    public NavMeshAgent agent;
    bool done = false;
    public GameObject human, car;

    private void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();

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
            else if(index == 3)
            {
                human.SetActive(false);
                car.SetActive(true);
            }

        }
    }

}
